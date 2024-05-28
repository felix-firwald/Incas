using Incas.Core.Models;
using Incas.Core.ViewModels;

namespace Incubator_2.ViewModels.VMAdmin
{
    internal class VM_SectorElement : BaseViewModel
    {
        public Sector sector;
        public VM_SectorElement(Sector s)
        {
            this.sector = s;
        }
        public string SectorName
        {
            get
            {
                return this.sector.name;
            }
            set { this.sector.name = value; }
        }
        public bool CanDelete
        {
            get
            {
                if (this.sector.slug == "data")
                {
                    return false;
                }
                return true;
            }
        }
        public void Save()
        {
            this.sector.SaveSector();
        }
    }
}
