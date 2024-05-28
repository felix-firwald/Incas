using GalaSoft.MvvmLight.Command;
using Incas.Common;
using System.Windows;
using System.Windows.Input;

namespace Incas.Core.ViewModels
{
    internal class ContainerWindowViewModel : BaseViewModel
    {
        #region Private Member

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window mWindow;

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int mOuterMarginSize = 10;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int mWindowRadius = 10;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        #endregion

        #region Public Properties

        /// <summary>
        /// The smallest width the window can go to
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 400;

        /// <summary>
        /// The smallest height the window can go to
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 400;

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool Borderless => this.mWindow.WindowState == WindowState.Maximized || this.mDockPosition != WindowDockPosition.Undocked;

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder { get; set; } = 6;

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness => new Thickness(this.ResizeBorder + this.OuterMarginSize);

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding => new Thickness(this.ResizeBorder);

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            get =>
                // If it is maximized or docked, no border
                this.Borderless ? 0 : this.mOuterMarginSize;
            set => this.mOuterMarginSize = value;
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSizeThickness => new Thickness(this.OuterMarginSize);

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            get =>
                // If it is maximized or docked, no border
                this.Borderless ? 0 : this.mWindowRadius;
            set => this.mWindowRadius = value;
        }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius => new CornerRadius(this.WindowRadius);

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public int TitleHeight { get; set; } = 42;
        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength => new GridLength(this.TitleHeight + this.ResizeBorder);

        #endregion

        #region Commands

        /// <summary>
        /// The command to minimize the window
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// The command to maximize the window
        /// </summary>
        public ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// The command to close the window
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to show the system menu of the window
        /// </summary>
        public ICommand MenuCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContainerWindowViewModel(Window window)
        {
            this.mWindow = window;

            // Listen out for the window resizing
            this.mWindow.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                this.WindowResized();
            };

            // Create commands
            this.MinimizeCommand = new RelayCommand(() => this.mWindow.WindowState = WindowState.Minimized);
            this.MaximizeCommand = new RelayCommand(() => this.mWindow.WindowState ^= WindowState.Maximized);
            this.CloseCommand = new RelayCommand(() => this.mWindow.Close());
            this.MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(this.mWindow, this.GetMousePosition()));

            // Fix window resize issue
            WindowResizer resizer = new WindowResizer(this.mWindow);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                this.mDockPosition = dock;

                // Fire off resize events
                this.WindowResized();
            };
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            Point position = Mouse.GetPosition(this.mWindow);

            // Add the window position so its a "ToScreen"
            return new Point(position.X + this.mWindow.Left, position.Y + this.mWindow.Top);
        }

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            this.OnPropertyChanged(nameof(this.Borderless));
            this.OnPropertyChanged(nameof(this.ResizeBorderThickness));
            this.OnPropertyChanged(nameof(this.OuterMarginSize));
            this.OnPropertyChanged(nameof(this.OuterMarginSizeThickness));
            this.OnPropertyChanged(nameof(this.WindowRadius));
            this.OnPropertyChanged(nameof(this.WindowCornerRadius));
        }


        #endregion
    }
}
