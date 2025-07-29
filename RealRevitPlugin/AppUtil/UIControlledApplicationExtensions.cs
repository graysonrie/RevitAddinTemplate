using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RealRevitPlugin.AppUtil {
    public static class UIControlledApplicationExtensions {
        public static RibbonPanel CreatePanel(this UIControlledApplication application, string tabName, string panelName) {
            // Create custom tab
            application.CreateRibbonTab(tabName);

            // Create a ribbon panel
            return application.CreateRibbonPanel(tabName, panelName);
        }
    }
}
