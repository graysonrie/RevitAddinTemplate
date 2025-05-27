// using Autodesk.Revit.UI;
// using RealRevitPlugin.WpfWindow;
// using RealRevitPlugin.AppUtil;

// namespace RealRevitPlugin {
//     public class App : IExternalApplication {
//         public Result OnShutdown(UIControlledApplication application) {
//             return Result.Succeeded;
//         }

//         public Result OnStartup(UIControlledApplication application) {
//             RevitContext.Setup();
//             CreateRibbon(application);
//             return Result.Succeeded;
//         }

//         //Create custom ribbon example:
//         private void CreateRibbon(UIControlledApplication application)
//         {
//             var panel = application.CreatePanel("HalftoneViews", "HalftoneViews");

//             panel.AddPushButton<Command1>("Execute")
//                 .SetImage("TestIcon.png")
//                 .SetLargeImage("TestIconBig.png");
//         }
//     }
// }
