//using Autodesk.Revit.DB;
//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.UI;
//using Autodesk.Revit.UI.Selection;
//using RealRevitPlugin.WpfWindow;
//using System.Collections.Generic;
//using System.Linq;
//using System;
//using Swan;

//namespace RealRevitPlugin
//{
//    [Transaction(TransactionMode.Manual)]
//    public class Command1 : IExternalCommand
//    {
//        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//        {
//            RevitContext.Setup();

//            //Get application and document objects  
//            UIApplication uiapp = commandData.Application;
//            Document doc = uiapp.ActiveUIDocument.Document;

//            GetViewsAndApplyHalftones(doc);

//            //RevitWindowSpawner.Spawn();

//            return Result.Succeeded;
//        }
//        public void GetViewsAndApplyHalftones(Document doc)
//        {
//            // Collect all views in the project
//            var views = new FilteredElementCollector(doc)
//                .OfClass(typeof(View))
//                .Cast<View>()
//                .Where(v => v != null && !v.IsTemplate)
//                .ToList();

//            var viewTemplates = views.Select(x =>
//            {
//                var template = x.ViewTemplateId;
//                if (template == ElementId.InvalidElementId) return null;
//                return doc.GetElement(template) as View;
//            }).Where(x => x != null);


//            List<string> names = viewTemplates.Select(x => x.Name).ToList();

//            HalftoneAllViews(doc);
//            //TaskDialog.Show("Views", string.Join("\n", names));
//        }
//        public void HalftoneAllViews(Document doc)
//        {
//            BuiltInCategory[] targetCategories =
//            [
//                BuiltInCategory.OST_RvtLinks,
//            ];

//            List<View> views = new FilteredElementCollector(doc).OfClass(typeof(View))
//                .Cast<View>()
//                .Where(v => v.ViewType == ViewType.FloorPlan || v.ViewType == ViewType.CeilingPlan
//                 && !v.IsTemplate)
//                .ToList();

//            using Transaction tx = new(doc, "Halftone All Views");
//            tx.Start();

//            //List<string> elements = [];
//            int applied = 0;
//            foreach (View view in views.Cast<View>())
//            {
//                applied++;
//                if (view.IsTemplate || !view.CanBePrinted) continue;

//                var collector = new FilteredElementCollector(doc, view.Id)
//                    .WhereElementIsNotElementType()
//                    .Where(e =>
//                    {
//                        Category cat = e.Category;
//                        if (cat == null) return false;

//                        var parsed = Enum.TryParse(cat.Id.Value.ToString(), out BuiltInCategory bic);
//                        //elements.Add(bic.ToString());
//                        return parsed && targetCategories.Contains(bic);
//                    });

//                foreach (Element element in collector)
//                {
//                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();
//                    ogs.SetHalftone(true);
//                    view.SetElementOverrides(element.Id, ogs);
//                }
//            }
//            tx.Commit();
//            TaskDialog.Show("Success", $"Applied halftone to {applied} views.");

//        }

//    }
//}
