using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Miniservices.UserStatistics
{
    public static class StatisticsManager
    {
        private static StatisticsInfo info { get; set; }
        public static StatisticsInfo Info
        {
            get
            {
                if (info is null)
                {
                    info = StatisticsInfo.Load();
                }
                return info;
            }
        }
        public static void SaveStatistics()
        {
            Info.Save();
        }
        public static Guid GetDefaultFieldForSearch(IClass cl)
        {
            return Info.GetSearchTargetFor(cl);
        }
        public static void AddWorkedObjects(List<IObject> objs)
        {
            Info.AddObjects(objs);
            Info.AddClassInteractionWrite(objs[0].Class);
        }
        public static void AddInteractionRead(IClass cl)
        {
            Info.AddClassInteractionRead(cl);
        }
        public static void AddInteractionSearch(IClass cl, Field field)
        {
            Info.AddClassInteractionSearch(cl, field);
        }
    }
}
