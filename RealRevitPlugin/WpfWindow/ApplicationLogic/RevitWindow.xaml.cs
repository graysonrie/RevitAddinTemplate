using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RealRevitPlugin.WpfWindow.ApplicationLogic
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RevitWindow : Window
    {
        private readonly WebWindowHandler _webWindowHandler;
        public RevitWindow()
        {
            _webWindowHandler = new WebWindowHandler(new WebWindowConfig());

            InitializeComponent();
            _webWindowHandler.StartLocalServer(Webview);
            TaskDialog.Show("Webview", $"Serving from {_webWindowHandler.Config.WebRootPath}");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RevitContext.Events.Execute(uiapp =>
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                // Collect all views in the project
                var collector = new FilteredElementCollector(doc)
                    .OfClass(typeof(View))
                    .Cast<View>()
                    .Where(v => !v.IsTemplate) // Exclude view templates
                    .ToList();

                // Get distinct view types
                HashSet<ViewType> viewTypes = new HashSet<ViewType>();
                foreach (View view in collector){
                    viewTypes.Add(view.ViewType);
                }

                // Output or use the view types
                TaskDialog.Show("View Types",
                    string.Join("\n", viewTypes.Select(vt => vt.ToString())));
            });
        }
        protected override void OnClosed(EventArgs e) {
            _webWindowHandler?.Dispose(); // Gracefully shut down the server
            base.OnClosed(e);
        }
    }
}
