using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Gdi32;
using Microsoft.Win32;

namespace System.IO {
    public static class Tools {
        public static ISet<string> GetDirs(string path) => GetDirs(path, new HashSet<string>());
        static ISet<string> GetDirs(string path, ISet<string> dirs) {
            try {
                var info = new DirectoryInfo(path);
                var attr = info.Attributes;
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                    if ((attr & FileAttributes.Hidden) != FileAttributes.Hidden) {
                        dirs.Add(path);
                        foreach (var s in Directory.EnumerateDirectories(path)) {
                            GetDirs(s, dirs);
                        }
                    }
                }
            } catch (System.IO.FileNotFoundException e) {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
            return dirs;
        }
        public static ISet<string> GetFiles(string path, string searchPattern, SearchOption searchOption) {
            ISet<string> files = new HashSet<string>();
            var dirs = searchOption == SearchOption.TopDirectoryOnly
                    ? new HashSet<string>() { path }
                    : GetDirs(path);
            foreach (var s in dirs) {
                foreach (var f in Directory.EnumerateFiles(s, "*.*", SearchOption.TopDirectoryOnly)) {
                    string file = Path.GetFullPath(f).ToLowerInvariant();
                    if (searchPattern != null && searchPattern != "*.*") {
                        if (!searchPattern.Contains(Path.GetExtension(file))) {
                            continue;
                        }
                    }
                    var attr = File.GetAttributes(file);
                    if ((attr & FileAttributes.Hidden) != FileAttributes.Hidden) {
                        files.Add(file);
                    }
                }
            }
            return files;
        }
        public static void TouchFiles(string path, DateTime dt) {
            foreach (var s in Tools.GetDirs(path)) {
                try {
                    Directory.SetCreationTime(s, dt);
                    Directory.SetLastAccessTime(s, dt);
                    Directory.SetLastWriteTime(s, dt);
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
                foreach (var f in Tools.GetFiles(s, "*.*", SearchOption.TopDirectoryOnly)) {
                    try {
                        File.SetCreationTime(f, dt);
                        File.SetLastAccessTime(f, dt);
                        File.SetLastWriteTime(f, dt);
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}

unsafe partial class App {
    public string CurrentDirectory {
        get => Environment.CurrentDirectory.Trim('\\', '/');
        set => Environment.CurrentDirectory = value;
    }

    public System.Ai.ITrainer Session;

    static IDictionary<string, Func<App, string, Func<bool>, bool>> _handlers = new Dictionary<string, Func<App, string, Func<bool>, bool>>();

    static void InitCliHandlers() {
        _handlers["--fit"] = (
            App app,
            string cliScript,
            Func<bool> IsTerminated) => {
                cliScript = cliScript
                    .Replace("--fit", "").Replace("fit", "");
                var dir = cliScript.Trim();
                if (Directory.Exists(dir)) {
                    app.CurrentDirectory = dir;
                }
                return global::Exec.Fit(app, app.CurrentDirectory, IsTerminated);
            };
        _handlers["--touch"] = (
            App app,
            string cliScript,
            Func<bool> IsTerminated) => {
                cliScript = cliScript
                    .Replace("--touch", "").Replace("touch", "");
                var dir = cliScript.Trim();
                if (Directory.Exists(dir)) {
                    app.CurrentDirectory = dir;
                }
                Tools.TouchFiles(
                    app.CurrentDirectory,
                    DateTime.Now);
                return false;
            };
    }

    static void Main() {
        InitCliHandlers();
        RunCli(new App());
    }

    #region Cli

    public static void RunCli(App app) {
        var HasCtrlBreak = false;
        Console.CancelKeyPress += OnCancelKey;
        try {
            Console.InputEncoding
                = Console.OutputEncoding = Encoding.UTF8;
            var cliScript = Environment.CommandLine;
            string exe = Environment.GetCommandLineArgs().First();
            if (cliScript.StartsWith($"\"{exe}\"")) {
                cliScript = cliScript.Remove(0, $"\"{exe}\"".Length);
            } else if (cliScript.StartsWith(exe)) {
                cliScript = cliScript.Remove(0, exe.Length);
            }
            cliScript
                = cliScript.Trim();
            if (!string.IsNullOrWhiteSpace(cliScript)) {
                HasCtrlBreak = DispatchCliScript(
                    app,
                    cliScript,
                    () => HasCtrlBreak);
            }
            while (!HasCtrlBreak) {
                Console.Write($"\r\n{app.CurrentDirectory}>");
                Console.Title = Path.GetFileNameWithoutExtension(
                    typeof(App).Assembly.Location);
                cliScript = Console.ReadLine();
                if (cliScript == null) {
                    HasCtrlBreak = false;
                    continue;
                }
                try {
                    HasCtrlBreak = DispatchCliScript(
                        app,
                        cliScript,
                        () => HasCtrlBreak);
                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
            }
        } finally {
            Console.CancelKeyPress -= OnCancelKey;
        }
        void OnCancelKey(object sender, ConsoleCancelEventArgs e) {
            HasCtrlBreak = true;
            e.Cancel = true;
        }
    }

    static bool DispatchCliScript(
            App app,
            string cliScript,
            Func<bool> IsTerminated) {
        if (cliScript.StartsWith("cd", StringComparison.OrdinalIgnoreCase)) {
            var dir = cliScript.Remove(0, "cd".Length).Trim();
            if (Directory.Exists(dir)) {
                app.CurrentDirectory = dir;
            }
        } else if (cliScript.StartsWith("cls", StringComparison.OrdinalIgnoreCase)) {
            Console.Clear();
        } else {
            foreach (var h in _handlers) {
                if (cliScript.StartsWith(h.Key, StringComparison.OrdinalIgnoreCase)) {
                    return h.Value(app, cliScript, IsTerminated);
                }
            }
            return global::Exec.Search(app, cliScript, IsTerminated);
        }
        return false;
    }

    #endregion

    public static Thread StartWin32UI<T>(IGdi32Controller controller, Gdi32<T>.DrawFrame onDrawFrame, Func<T> onGetFrame, string title,
        Color bgColor, Icon hIcon = null, Size? size = null)
        where T : class {
        Gdi32<T> hWnd = null;
        var t = new Thread(() => {
            IntPtr handl = IntPtr.Zero;
            try {
                hWnd = new Gdi32<T>(controller, title,
                    onDrawFrame,
                    TimeSpan.FromMilliseconds(173),
                    onGetFrame, bgColor, hIcon, size);
                handl = hWnd.hWnd;
                hWnd.Show();
                while (User32.GetMessage(out MSG msg, hWnd.hWnd, 0, 0) != 0) {
                    User32.TranslateMessage(ref msg);
                    User32.DispatchMessage(ref msg);
                }
            } catch (Exception e) {
                Console.Error?.WriteLine(e);
            } finally {
                hWnd?.Dispose();
                hWnd = null;
            }
        });
        t.Start();
        return t;
    }
}