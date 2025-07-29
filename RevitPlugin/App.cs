using Autodesk.Revit.UI;
using RealRevitPlugin.Extensions;
using RealRevitPlugin;
using System.Reflection;

namespace RevitPlugin {
    public class App : IExternalApplication {
        public Result OnShutdown(UIControlledApplication application) {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application) {
            ResourceAccessor.SetExecutingAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            // Place your startup logic down here:

            CreateRibbon(application);

            return Result.Succeeded;
        }

        //Create custom ribbon example:
        private void CreateRibbon(UIControlledApplication application)
        {
            var panel = application.CreatePanel("HalftoneViews", "HalftoneViews");

            panel.AddPushButton<HalftoneCommand>("Execute")
                .SetImage("TestIcon.png")
                .SetLargeImage("TestIconBig.png");
        }
    }
}