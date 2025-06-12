using Autodesk.Revit.UI;
using RealRevitPlugin.Extensions;

namespace RevitPlugin {
    public class App : IExternalApplication {
        public Result OnShutdown(UIControlledApplication application) {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application) {
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