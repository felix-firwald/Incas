using IncasEngine.ObjectiveEngine.Interfaces;
using System.Windows.Controls;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectBackReferences.xaml
    /// </summary>
    public partial class ObjectBackReferences : UserControl
    {
        public IClass Class { get; set; }
        public IObject Object { get; set; }
        public ObjectBackReferences(IClass @class, IObject @object)
        {
            this.InitializeComponent();
            this.Class = @class;
            this.Object = @object;
            this.PlaceCard();
        }
        public void PlaceCard()
        {
            ObjectCard card = new(this.Class, false);
            card.UpdateFor(this.Object);
            this.CardPlacer.Child = card;
        }
    }
}
