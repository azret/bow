using System.Collections.Generic;

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
