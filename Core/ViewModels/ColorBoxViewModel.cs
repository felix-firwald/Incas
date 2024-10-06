using System.Windows.Media;

namespace Incas.Core.ViewModels
{
    internal class ColorBoxViewModel : BaseViewModel
    {
        private byte r;
        private byte g;
        private byte b;
        public ColorBoxViewModel(byte R, byte G, byte B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }
        public void Apply(System.Windows.Media.Color color)
        {
            this.R = color.R;
            this.G = color.G;
            this.B = color.B;
        }
        public System.Windows.Media.Brush Color => new SolidColorBrush(System.Windows.Media.Color.FromRgb(this.r, this.g, this.b));
        public byte R
        {
            get => this.r;
            set
            {
                this.r = value;
                this.OnPropertyChanged(nameof(this.R));
                this.OnPropertyChanged(nameof(this.Color));
            }
        }
        public byte G
        {
            get => this.g;
            set
            {
                this.g = value;
                this.OnPropertyChanged(nameof(this.G));
                this.OnPropertyChanged(nameof(this.Color));
            }
        }
        public byte B
        {
            get => this.b;
            set
            {
                this.b = value;
                this.OnPropertyChanged(nameof(this.B));
                this.OnPropertyChanged(nameof(this.Color));
            }
        }
    }
}
