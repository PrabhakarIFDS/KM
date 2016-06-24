using Microsoft.SharePoint.Administration;
using System.Collections.Generic;


namespace IFDS.KMPortal.Helper
{
    public class Logger : SPDiagnosticsServiceBase
    {
        //fgdgdfg
        public static string DiagnosticAreaName = "Fishbone";
        private static Logger _Current;
        public static Logger Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new Logger();
                }
                return _Current;
            }
        }

        public Logger()
            : base("Fishbone Logging Service", SPFarm.Local)
        {

        }

        public enum Category
        {
            Unexpected,
            High,
            Medium,
            Information
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
        {
            new SPDiagnosticsArea(DiagnosticAreaName, new List<SPDiagnosticsCategory>
            {
                new SPDiagnosticsCategory("Unexpected", TraceSeverity.Unexpected, EventSeverity.Error),
                new SPDiagnosticsCategory("High", TraceSeverity.High, EventSeverity.Warning),
                new SPDiagnosticsCategory("Medium", TraceSeverity.Medium, EventSeverity.Information),
                new SPDiagnosticsCategory("Information", TraceSeverity.Verbose, EventSeverity.Information)
           })
        };

            return areas;
        }

        /// <summary>
        /// Method to write into sharepoint uls logs
        /// </summary>
        /// <param name="logEntry"></param>
        public static void WriteLog(Category categoryName, string source, string errorMessage)
        {
            SPDiagnosticsCategory category = Logger.Current.Areas[DiagnosticAreaName].Categories[categoryName.ToString()];
            Logger.Current.WriteTrace(0, category, category.TraceSeverity, string.Concat(source, ": ", errorMessage));
        }
        
    }
}
