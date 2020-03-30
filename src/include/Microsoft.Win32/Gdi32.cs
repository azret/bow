namespace Microsoft.Win32.Gdi {
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Microsoft.Win32;

    public interface IGdi32Controller {
        void WM_WINMM(IntPtr hWnd, IntPtr wParam, IntPtr lParam);
        void WM_SHOWWINDOW(IntPtr hWnd, IntPtr wParam, IntPtr lParam);
        void WM_KEYDOWN(IntPtr hWnd, IntPtr wParam, IntPtr lParam);
        void WM_CLOSE(IntPtr hWnd, IntPtr wParam, IntPtr lParam);
    }

    public class Plot2D {
        public static readonly Font Font = new Font("Consolas", 14.5f);
    }

    public class Gdi32<T> : Plot2D, IDisposable
        where T: class {
        public delegate void DrawFrame(Graphics g, RectangleF fill, float t, T userState);
        public enum SystemIcons {
            IDI_APPLICATION = 32512,
            IDI_HAND = 32513,
            IDI_QUESTION = 32514,
            IDI_EXCLAMATION = 32515,
            IDI_ASTERISK = 32516,
            IDI_WINLOGO = 32517,
            IDI_WARNING = IDI_EXCLAMATION,
            IDI_ERROR = IDI_HAND,
            IDI_INFORMATION = IDI_ASTERISK,
        }
        public IntPtr hWnd;
        Timer hTimer;
        long _startTime = 0;
        public float GetLocalTime() {
            if (_startTime == 0) { _startTime = Environment.TickCount; }
            return (Environment.TickCount - _startTime) * 0.001f;
        }
        public readonly Func<T> _getFrame;
        DrawFrame _onDrawFrame;
        int DefWndProc(IntPtr hWnd, WM msg, IntPtr wParam, IntPtr lParam) {
            return User32.DefWindowProc(hWnd, msg, wParam, lParam);
        }
        int UserWndProc(IntPtr hWnd, WM msg, IntPtr wParam, IntPtr lParam) {
            if (hWnd != this.hWnd) {
                return User32.DefWindowProc(hWnd, (WM)msg, wParam, lParam);
            }
            switch ((WM)msg) {
                case WM.CREATE:
                    break;
                case WM.WINMM:
                    OnWinMM(hWnd, wParam, lParam);
                    break;
                case WM.KEYDOWN:
                    _controller?.WM_KEYDOWN(hWnd, wParam, lParam);
                    return 0;
                case WM.SIZE:
                case WM.SIZING:
                    User32.GetClientRect(hWnd, out RECT lprctw);
                    User32.InvalidateRect(hWnd, ref lprctw, false);
                    break;
                case WM.PAINT:
                    OnPaint(_onDrawFrame, _getFrame, 1, hWnd);
                    return 0;
                case WM.CLOSE:
                    _controller?.WM_CLOSE(hWnd, wParam, lParam);
                    break;
                case WM.SHOWWINDOW:
                    _controller?.WM_SHOWWINDOW(hWnd, wParam, lParam);
                    break;
                case WM.DESTROY:
                    Dispose();
                    User32.PostQuitMessage(0);
                    return 0;
            }
            return User32.DefWindowProc(hWnd, (WM)msg, wParam, lParam);
        }
        Color _bgColor;
        WndProc lpfnWndProcPtr;
        public class ClassTemplate {
            internal Icon hIcon;
            internal string szName = "WNDCLASSEX_PLOT2D_" + Guid.NewGuid().ToString("N");
            internal WNDCLASSEX _lpwcx;
            internal WndProc lpfnDefWndProcPtr = new WndProc(
                (IntPtr hWnd, WM msg, IntPtr wParam, IntPtr lParam) =>
                    User32.DefWindowProc(hWnd, msg, wParam, lParam));
        }
        IGdi32Controller _controller;
        ClassTemplate _ClassTemplate;
        public Gdi32(IGdi32Controller controller, string title, DrawFrame onDrawFrame, TimeSpan framesPerSecond,
            Func<T> getFrame, Color bgColor, System.Drawing.Icon hIcon, Size? sz) {
            _controller = controller;
            _getFrame = getFrame;
            _onDrawFrame = onDrawFrame;
            _bgColor = bgColor;
            if (_ClassTemplate == null) {
                _ClassTemplate = new ClassTemplate();
                _ClassTemplate._lpwcx = new WNDCLASSEX();
                _ClassTemplate._lpwcx.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
                _ClassTemplate._lpwcx.hInstance = User32.GetModuleHandle(null);
                _ClassTemplate.hIcon = hIcon != null
                    ? hIcon
                    : Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
                _ClassTemplate._lpwcx.style = (int)(ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw);
                _ClassTemplate._lpwcx.cbClsExtra = 0;
                _ClassTemplate._lpwcx.cbWndExtra = 0;
                _ClassTemplate._lpwcx.hCursor = User32.LoadCursor(IntPtr.Zero, (int)Constants.IDC_ARROW);
                _ClassTemplate._lpwcx.hbrBackground = User32.CreateSolidBrush(ColorTranslator.ToWin32(_bgColor));
                _ClassTemplate._lpwcx.lpszMenuName = null;
                _ClassTemplate._lpwcx.lpszClassName = _ClassTemplate.szName;
                _ClassTemplate._lpwcx.hIcon = _ClassTemplate.hIcon.Handle;
                _ClassTemplate._lpwcx.lpfnWndProc = _ClassTemplate.lpfnDefWndProcPtr;
                if (User32.RegisterClassEx(ref _ClassTemplate._lpwcx) == 0) {
                    if (1410 != Marshal.GetLastWin32Error()) {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
            }
            hWnd = User32.CreateWindowEx(
                    WindowStylesEx.WS_EX_APPWINDOW,
                    _ClassTemplate.szName,
                    title,
                    WindowStyles.WS_OVERLAPPED |
                    WindowStyles.WS_SYSMENU |
                    WindowStyles.WS_BORDER |
                    WindowStyles.WS_SIZEFRAME |
                    WindowStyles.WS_MINIMIZEBOX |
                    WindowStyles.WS_MAXIMIZEBOX,
                160,
                230,
                sz?.Width ?? 720,
                sz?.Height ?? (int)(720 * (3f / 5f)),
                IntPtr.Zero,
                IntPtr.Zero,
                _ClassTemplate._lpwcx.hInstance,
                IntPtr.Zero);
            if (hWnd == IntPtr.Zero) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            lpfnWndProcPtr = new WndProc(UserWndProc);
            User32.SetWindowLongPtr(hWnd,
                (int)User32.WindowLongFlags.GWL_WNDPROC,
                lpfnWndProcPtr);
            User32.SendMessage(hWnd, WM.SETICON, IntPtr.Zero, _ClassTemplate._lpwcx.hIcon);
            hTimer = new Timer((state) => {
                User32.GetClientRect(hWnd, out RECT lprctw);
                User32.InvalidateRect(hWnd, ref lprctw, false);
            }, null, 0, (int)framesPerSecond.TotalMilliseconds);
        }
        public void Dispose() {
            hTimer?.Dispose();
            hTimer = null;
            if (hWnd != IntPtr.Zero) {
                User32.ShowWindow(hWnd, ShowWindowCommands.Hide);
                User32.DestroyWindow(hWnd);
            }
            hWnd = IntPtr.Zero;
            lpfnWndProcPtr = null;
        }
        void OnPaint(DrawFrame onDrawFrame, Func<T> userState, float scale, IntPtr hWnd) {
            IntPtr hdc = User32.BeginPaint(hWnd, out PAINTSTRUCT ps);
            User32.GetClientRect(hWnd,
                out RECT lprct);
            RECT Membitrect = new RECT(0, 0, (int)((lprct.Right - lprct.Left) * scale),
                (int)((lprct.Bottom - lprct.Top) * scale));
            IntPtr hMemDC = User32.CreateCompatibleDC(hdc);
            IntPtr hMemBitmap = User32.CreateCompatibleBitmap(hdc,
                Membitrect.Right - Membitrect.Left,
                Membitrect.Bottom - Membitrect.Top);
            User32.SelectObject(hMemDC, hMemBitmap);
            Graphics _g = Graphics.FromHdc(hMemDC);
            var phase = GetLocalTime();
            //try {
                var bgBrush = new SolidBrush(_bgColor);
                _g.FillRectangle(bgBrush, 0, 0, Membitrect.Right - Membitrect.Left, Membitrect.Bottom - Membitrect.Top);
                bgBrush.Dispose();
                onDrawFrame?.Invoke(_g,
                    new RectangleF(0, 0, Membitrect.Right - Membitrect.Left, Membitrect.Bottom - Membitrect.Top),
                    phase, userState != null ?
                        userState.Invoke() : null);
            //} catch {
            //    throw;
            //}
            _g.Dispose();
            int margin = 0;
            User32.StretchBlt(hdc, margin, margin, lprct.Right - lprct.Left - margin - margin,
                lprct.Bottom - lprct.Top - margin - margin, hMemDC, 0, 0,
                Membitrect.Right - Membitrect.Left - margin - margin,
                Membitrect.Bottom - Membitrect.Top - margin - margin,
                User32.TernaryRasterOperations.SRCCOPY);
            User32.DeleteObject(hMemBitmap);
            User32.DeleteDC(hMemDC);
            User32.EndPaint(hWnd, ref ps);
        }
        private unsafe void OnWinMM(IntPtr hWnd, IntPtr wParam, IntPtr lParam) {
            User32.GetClientRect(hWnd, out RECT lprctw3);
            User32.InvalidateRect(hWnd, ref lprctw3, false);
            _controller?.WM_WINMM(hWnd, wParam, lParam);
        }
        public void Invalidate() {
            if (hWnd == IntPtr.Zero) {
                throw new ObjectDisposedException(GetType().Name);
            }
            User32.GetClientRect(hWnd, out RECT lprctw);
            User32.InvalidateRect(hWnd, ref lprctw, false);
        }
        public void Show() {
            if (hWnd == IntPtr.Zero) {
                throw new ObjectDisposedException(GetType().Name);
            }
            User32.ShowWindow(hWnd, ShowWindowCommands.Normal);
            User32.UpdateWindow(hWnd);
        }
    }
}