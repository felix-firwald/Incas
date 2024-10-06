using System.Windows;
using System.Windows.Controls;

namespace Incas.Utils.CustomControls
{
    /// <summary>
    /// Выполните шаги 1a или 1b, а затем 2, чтобы использовать этот пользовательский элемент управления в файле XAML.
    ///
    /// Шаг 1a. Использование пользовательского элемента управления в файле XAML, существующем в текущем проекте.
    /// Добавьте атрибут XmlNamespace в корневой элемент файла разметки, где он 
    /// будет использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Incas.Utils.CustomControls"
    ///
    ///
    /// Шаг 1б. Использование пользовательского элемента управления в файле XAML, существующем в другом проекте.
    /// Добавьте атрибут XmlNamespace в корневой элемент файла разметки, где он 
    /// будет использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Incas.Utils.CustomControls;assembly=Incas.Utils.CustomControls"
    ///
    /// Потребуется также добавить ссылку из проекта, в котором находится файл XAML,
    /// на данный проект и пересобрать во избежание ошибок компиляции:
    ///
    ///     Щелкните правой кнопкой мыши нужный проект в обозревателе решений и выберите
    ///     "Добавить ссылку"->"Проекты"->[Поиск и выбор проекта]
    ///
    ///
    /// Шаг 2)
    /// Теперь можно использовать элемент управления в файле XAML.
    ///
    ///     <MyNamespace:NumericBox/>
    ///
    /// </summary>
    public class NumericBox : Control
    {

        public int MinValue
        {
            get => (int)this.GetValue(MinValueProperty);
            set => this.SetValue(MinValueProperty, value);
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(int), typeof(NumericBox), new PropertyMetadata(0));

        public int MaxValue
        {
            get => (int)this.GetValue(MaxValueProperty);
            set => this.SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(NumericBox), new PropertyMetadata(int.MaxValue));

        public int Value
        {
            get => (int)this.GetValue(ValueProperty);
            set
            {
                int result = value;
                if (result < this.MinValue)
                {
                    result = this.MinValue;
                }
                if (result > this.MaxValue)
                {
                    result = this.MaxValue;
                }
                this.SetValue(ValueProperty, result);
            }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(NumericBox), new PropertyMetadata(0));

        static NumericBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericBox), new FrameworkPropertyMetadata(typeof(NumericBox)));
        }
    }
}
