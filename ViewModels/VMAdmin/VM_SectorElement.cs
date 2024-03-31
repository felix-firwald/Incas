using Incubator_2.Models;

namespace Incubator_2.ViewModels.VMAdmin
{
    class VM_SectorElement : VM_Base
    {
        public Sector sector;
        public VM_SectorElement(Sector s)
        {
            sector = s;
        }
        public string SectorName
        {
            get
            {
                return sector.name;
            }
            set { sector.name = value; }
        }
        public bool CanDelete
        {
            get
            {
                if (sector.slug == "data")
                {
                    return false;
                }
                return true;
            }
        }
        public void Save()
        {
            sector.SaveSector();
        }
    }
}
