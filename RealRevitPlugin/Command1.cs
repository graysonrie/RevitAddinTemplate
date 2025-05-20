using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RealRevitPlugin.WpfWindow;

namespace RealRevitPlugin
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            RevitContext.Setup();

            //Get application and document objects  
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            RevitWindowSpawner.Spawn();

            return Result.Succeeded;
        }
    }
}
