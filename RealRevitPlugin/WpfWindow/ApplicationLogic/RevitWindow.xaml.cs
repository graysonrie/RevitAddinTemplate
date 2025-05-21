using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Input;

namespace RealRevitPlugin.WpfWindow.ApplicationLogic
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RevitWindow : Window
    {
        private readonly WebWindowHandler _webWindowHandler;
        private readonly RevitEventCaller _eventCaller;

        public RevitWindow()
        {
            _webWindowHandler = new WebWindowHandler(new WebWindowConfig());

            InitializeComponent();

            Dispatcher.InvokeAsync(async ()=> {
                await _webWindowHandler.StartLocalServer(Webview);
            });
            Webview.CoreWebView2InitializationCompleted += (s, e) =>
            {
                if (!e.IsSuccess) {
                    TaskDialog.Show("WebView2 Init Failed", e.InitializationException?.Message);
                }
            };

            _eventCaller = new RevitEventCaller();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _eventCaller.Execute((uiapp) =>
            {
                try
                {
                    UIDocument uidoc = uiapp.ActiveUIDocument;
                    if (uidoc?.Document == null) return;

                    Document doc = uidoc.Document;

                    // Collect all views in the project
                    var collector = new FilteredElementCollector(doc)
                        .OfClass(typeof(View))
                        .Cast<View>()
                        .Where(v => v != null && !v.IsTemplate)
                        .ToList();

                    // Get distinct view types
                    HashSet<ViewType> viewTypes = new HashSet<ViewType>();
                    foreach (View view in collector)
                    {
                        viewTypes.Add(view.ViewType);
                    }

                    // Output or use the view types
                    TaskDialog.Show("View Types",
                        string.Join("\n", viewTypes.Select(vt => vt.ToString())));
                }
                catch (System.Exception ex)
                {
                    TaskDialog.Show("Error", $"An error occurred: {ex.Message}");
                }
            });
        }

        protected override void OnClosed(EventArgs e) {
            _webWindowHandler?.Dispose(); // Gracefully shut down the server
            _eventCaller?.Dispose();
            base.OnClosed(e);
        }
    }
}
