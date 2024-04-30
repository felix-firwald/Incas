using Incubator_2.Models;

namespace Incubator_2.ViewModels.VMAdmin
{
    internal class VM_SectorElement : VM_Base
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
