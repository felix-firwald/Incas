using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Components
{
    internal class ObjectiveScripting
    {
        internal const string IncasPragma = "# [INCAS PRAGMA]";
        internal const string PragmaStartRegion = " start region ";
        internal const string PragmaEndRegion = " end region ";
        internal static string GetPragmaStartMethodsRegion()
        {
            return IncasPragma + PragmaStartRegion + "methods";
        }
        internal static string GetPragmaEndMethodsRegion()
        {
            return IncasPragma + PragmaEndRegion + "methods";
        }
        internal static string GetPragmaStartEventsRegion()
        {
            return IncasPragma + PragmaStartRegion + "events";
        }
        internal static string GetPragmaEndEventsRegion()
        {
            return IncasPragma + PragmaEndRegion + "events";
        }
        internal static string GetPragmaStartActionsRegion()
        {
            return IncasPragma + PragmaStartRegion + "actions";
        }
        internal static string GetPragmaEndActionsRegion()
        {
            return IncasPragma + PragmaEndRegion + "actions";
        }
    }
}
