using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Incas.Common
{
    public enum WindowDockPosition
    {
        /// <summary>
        /// Not docked
        /// </summary>
        Undocked,
        /// <summary>
        /// Docked to the left of the screen
        /// </summary>
        Left,
        /// <summary>
        /// Docked to the right of the screen
        /// </summary>
        Right,
    }

    /// <summary>
    /// Fixes the issue with Windows of Style <see cref="WindowStyle.None"/> covering the taskbar
    /// </summary>
    public class WindowResizer
    {
        #region Private Members

        /// <summary>
        /// The window to handle the resizing for
        /// </summary>
        private Window mWindow;

        /// <summary>
        /// The last calculated available screen size
        /// </summary>
        private Rect mScreenSize = new Rect();

        /// <summary>
        /// How close to the edge the window has to be to be detected as at the edge of the screen
        /// </summary>
        private int mEdgeTolerance = 2;

        /// <summary>
        /// The transform matrix used to convert WPF sizes to screen pixels
        /// </summary>
        private Matrix mTransformToDevice;

        /// <summary>
        /// The last screen the window was on
        /// </summary>
        private nint mLastScreen;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mLastDock = WindowDockPosition.Undocked;

        #endregion

        #region Dll Imports

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(nint hMonitor, MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        #endregion

        #region Public Events

        /// <summary>
        /// Called when the window dock position changes
        /// </summary>
        public event Action<WindowDockPosition> WindowDockChanged = (dock) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window">The window to monitor and correctly maximize</param>
        /// <param name="adjustSize">The callback for the host to adjust the maximum available size if needed</param>
        public WindowResizer(Window window)
        {
            this.mWindow = window;

            // Create transform visual (for converting WPF size to pixel size)
            this.GetTransform();

            // Listen out for source initialized to setup
            this.mWindow.SourceInitialized += this.Window_SourceInitialized;

            // Monitor for edge docking
            this.mWindow.SizeChanged += this.Window_SizeChanged;
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Gets the transform object used to convert WPF sizes to screen pixels
        /// </summary>
        private void GetTransform()
        {
            // Get the visual source
            PresentationSource source = PresentationSource.FromVisual(this.mWindow);

            // Reset the transform to default
            this.mTransformToDevice = default;

            // If we cannot get the source, ignore
            if (source == null)
                return;

            // Otherwise, get the new transform object
            this.mTransformToDevice = source.CompositionTarget.TransformToDevice;
        }

        /// <summary>
        /// Initialize and hook into the windows message pump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            // Get the handle of this window
            nint handle = new WindowInteropHelper(this.mWindow).Handle;
            HwndSource handleSource = HwndSource.FromHwnd(handle);

            // If not found, end
            if (handleSource == null)
                return;

            // Hook into it's Windows messages
            handleSource.AddHook(this.WindowProc);
        }

        #endregion

        #region Edge Docking

        /// <summary>
        /// Monitors for size changes and detects if the window has been docked (Aero snap) to an edge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // We cannot find positioning until the window transform has been established
            if (this.mTransformToDevice == default)
                return;

            // Get the WPF size
            Size size = e.NewSize;

            // Get window rectangle
            double top = this.mWindow.Top;
            double left = this.mWindow.Left;
            double bottom = top + size.Height;
            double right = left + this.mWindow.Width;

            // Get window position/size in device pixels
            Point windowTopLeft = this.mTransformToDevice.Transform(new Point(left, top));
            Point windowBottomRight = this.mTransformToDevice.Transform(new Point(right, bottom));

            // Check for edges docked
            bool edgedTop = windowTopLeft.Y <= this.mScreenSize.Top + this.mEdgeTolerance;
            bool edgedLeft = windowTopLeft.X <= this.mScreenSize.Left + this.mEdgeTolerance;
            bool edgedBottom = windowBottomRight.Y >= this.mScreenSize.Bottom - this.mEdgeTolerance;
            bool edgedRight = windowBottomRight.X >= this.mScreenSize.Right - this.mEdgeTolerance;

            // Get docked position
            WindowDockPosition dock = WindowDockPosition.Undocked;

            // Left docking
            dock = edgedTop && edgedBottom && edgedLeft
                ? WindowDockPosition.Left
                : edgedTop && edgedBottom && edgedRight ? WindowDockPosition.Right : WindowDockPosition.Undocked;

            // If dock has changed
            if (dock != this.mLastDock)
                // Inform listeners
                WindowDockChanged(dock);

            // Save last dock position
            this.mLastDock = dock;
        }

        #endregion

        #region Windows Proc

        /// <summary>
        /// Listens out for all windows messages for this window
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private nint WindowProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            switch (msg)
            {
                // Handle the GetMinMaxInfo of the Window
                case 0x0024:/* WM_GETMINMAXINFO */
                    this.WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return 0;
        }

        #endregion

        /// <summary>
        /// Get the min/max window size for this window
        /// Correctly accounting for the taskbar size and position
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        private void WmGetMinMaxInfo(nint hwnd, nint lParam)
        {
            // Get the point position to determine what screen we are on
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            // Get the primary monitor at cursor position 0,0
            nint lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);

            // Try and get the primary screen information
            MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
                return;

            // Now get the current screen
            nint lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

            // If this has changed from the last one, update the transform
            if (lCurrentScreen != this.mLastScreen || this.mTransformToDevice == default)
                this.GetTransform();

            // Store last know screen
            this.mLastScreen = lCurrentScreen;

            // Get min/max structure to fill with information
            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // If it is the primary screen, use the rcWork variable
            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            // Otherwise it's the rcMonitor values
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }

            // Set min size
            Point minSize = this.mTransformToDevice.Transform(new Point(this.mWindow.MinWidth, this.mWindow.MinHeight));

            lMmi.ptMinTrackSize.X = (int)minSize.X;
            lMmi.ptMinTrackSize.Y = (int)minSize.Y;

            // Store new size
            this.mScreenSize = new Rect(lMmi.ptMaxPosition.X, lMmi.ptMaxPosition.Y, lMmi.ptMaxSize.X, lMmi.ptMaxSize.Y);

            // Now we have the max size, allow the host to tweak as needed
            Marshal.StructureToPtr(lMmi, lParam, true);
        }
    }

    #region Dll Helper Structures

    internal enum MonitorOptions : uint
    {
        MONITOR_DEFAULTTONULL = 0x00000000,
        MONITOR_DEFAULTTOPRIMARY = 0x00000001,
        MONITOR_DEFAULTTONEAREST = 0x00000002
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public Rectangle rcMonitor = new Rectangle();
        public Rectangle rcWork = new Rectangle();
        public int dwFlags = 0;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int Left, Top, Right, Bottom;

        public Rectangle(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// x coordinate of point.
        /// </summary>
        public int X;
        /// <summary>
        /// y coordinate of point.
        /// </summary>
        public int Y;

        /// <summary>
        /// Construct a point of coordinates (x,y).
        /// </summary>
        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    #endregion
}
