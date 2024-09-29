using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Core.ViewModels
{
    class ColorBoxViewModel : BaseViewModel
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
        public System.Windows.Media.Brush Color
        {
            get
            {
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(this.r, this.g, this.b));
            }
        }
        public byte R
        {
            get
            {
                return this.r;
            }
            set
            {
                this.r = value;
                this.OnPropertyChanged(nameof(this.R));
                this.OnPropertyChanged(nameof(this.Color));
            }
        }
        public byte G
        {
            get
            {
                return this.g;
            }
            set
            {
                this.g = value;
                this.OnPropertyChanged(nameof(this.G));
                this.OnPropertyChanged(nameof(this.Color));
            }
        }
        public byte B
        {
            get
            {
                return this.b;
            }
            set
            {
                this.b = value;
                this.OnPropertyChanged(nameof(this.B));
                this.OnPropertyChanged(nameof(this.Color));
            }
        }
    }
}
