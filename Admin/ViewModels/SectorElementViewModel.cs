using Incas.Core.Models;
using Incas.Core.ViewModels;

namespace Incas.Admin.ViewModels
{
    internal class SectorElementViewModel : BaseViewModel
    {
        public Sector sector;
        public SectorElementViewModel(Sector s)
        {
            this.sector = s;
        }
        public string SectorName
        {
            get => this.sector.name;
            set => this.sector.name = value;
        }
        public bool CanDelete => this.sector.slug != "data";
        public void Save()
        {
            this.sector.SaveSector();
        }
    }
}
