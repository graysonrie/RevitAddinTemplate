using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RealRevitPlugin.WpfWindow;
using System.Collections.Generic;
using System.Linq;
using System;
using Swan;

namespace RealRevitPlugin
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            RevitContext.Setup();

            //Get application and document objects  
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

           // GetViewsAndApplyHalftones(doc);

            RevitWindowSpawner.Spawn();

            return Result.Succeeded;
        }
        public void GetViewsAndApplyHalftones(Document doc) {
            // Collect all views in the project
            var views = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(v => v != null && !v.IsTemplate)
                .ToList();

            var viewTemplates = views.Select(x => {
                var template = x.ViewTemplateId;
                if (template == ElementId.InvalidElementId) return null;
                return doc.GetElement(template) as View;
            }).Where(x => x != null);


            List<string> names = viewTemplates.Select(x => x.Name).ToList();

            HalftoneAllViews(doc);
            TaskDialog.Show("Views", string.Join("\n", names));
        }
        public void HalftoneAllViews(Document doc) {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(View)).WhereElementIsNotElementType();

            var views = viewCollector.Where(x => x.Name.Contains("Level"));

            using (Transaction tx = new Transaction(doc, "Halftone All Views")) {
                tx.Start();

                int applied = 0;
                foreach (View view in views.Cast<View>()) {
                    applied++;
                    // Skip view templates or non-printable views
                    if (view.IsTemplate || !view.CanBePrinted) continue;

                    // Get all visible elements in this view
                    ICollection<ElementId> elementIds = new FilteredElementCollector(doc, view.Id)
                        .WhereElementIsNotElementType()
                        .ToElementIds();

                    foreach (ElementId id in elementIds) {
                        OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                        ogs.SetHalftone(true);

                        view.SetElementOverrides(id, ogs);
                    }
                }
                TaskDialog.Show("Halftone", $"Applying halftone to {applied} views.");

                tx.Commit();
            }
        }

    }
}
