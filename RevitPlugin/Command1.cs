using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace RevitPlugin {
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            //Get application and document objects  
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your command logic here

            return Result.Succeeded;
        }
    }
}
