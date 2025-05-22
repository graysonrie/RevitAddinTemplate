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
            }).Where(x=>x != null);

            foreach(var template in viewTemplates) {
                SetRevitLinksHalftone(doc, template);
            }

            var revitLinks = viewTemplates.SelectMany(vt => GetVisibleRevitLinks(doc, vt)).ToList();

            List<string> names = viewTemplates.SelectMany(vt => GetVisibleRevitLinks(doc, vt).Select(x => $"{vt.Name} : {string.Join("\n", x)}")).ToList();

            HalftoneAllViews(doc);
            TaskDialog.Show("Views", string.Join("\n", names));


            //RevitWindowSpawner.Spawn();

            return Result.Succeeded;
        }
        public List<RevitLinkInstance> GetVisibleRevitLinks(Document doc, View view) {
            List<RevitLinkInstance> visibleLinks = new List<RevitLinkInstance>();

            // Get all RevitLinkInstances in the document
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance));

            foreach (RevitLinkInstance link in collector.Cast<RevitLinkInstance>()) {
                // Check if the element is visible in the view
                visibleLinks.Add(link);
            }

            return visibleLinks;
        }
        public void SetRevitLinksHalftone(Document doc, View viewOrTemplate) {
            // Collect all Revit Link Instances in the entire document
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var linkInstances = collector.OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>();

            using (Transaction tx = new Transaction(doc, "Set Revit Links to Halftone")) {
                tx.Start();

                foreach (var linkInstance in linkInstances) {
                    var ogs = viewOrTemplate.GetElementOverrides(linkInstance.Id);

                    try {
                        var linkOverrides = viewOrTemplate.GetLinkOverrides(linkInstance.Id);
                        linkOverrides.LinkVisibilityType = LinkVisibility.ByLinkView;
                        viewOrTemplate.SetLinkOverrides(linkInstance.Id, linkOverrides);
                        TaskDialog.Show("good", "Set Link Visibility to " + viewOrTemplate.Name);
                    } catch (Exception ex) {
                    }


                    //TaskDialog.Show("good", "Set Halftone to " + viewOrTemplate.Name);
                    viewOrTemplate.SetElementOverrides(linkInstance.Id, ogs.SetHalftone(true));
                }

                tx.Commit();
            }
        }
        public void HalftoneAllViews(Document doc) {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .WhereElementIsNotElementType();

            using (Transaction tx = new Transaction(doc, "Halftone All Views")) {
                tx.Start();

                foreach (View view in viewCollector.Cast<View>()) {
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

                tx.Commit();
            }
        }

    }
}
