using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System;
using RealRevitPlugin.WpfWindow.Web;

namespace RealRevitPlugin.WpfWindow.ApplicationLogic
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RevitWindow : Window
    {
        private readonly WebCommands _webCommands;
        private readonly RevitEventCaller _eventCaller;

        public RevitWindow()
        {
            _webCommands = new WebCommands();
            _eventCaller = new RevitEventCaller();

            InitializeComponent();

            _webCommands.RegisterCommand("GetViews", async () =>
            {
                return await _eventCaller.Execute(uiapp =>
                {
                    UIDocument uidoc = uiapp.ActiveUIDocument;
                    if (uidoc?.Document == null) return new List<string>();

                    Document doc = uidoc.Document;

                    // Collect all views in the project
                    var views = new FilteredElementCollector(doc)
                        .OfClass(typeof(View))
                        .Cast<View>()
                        .Where(v => v != null && !v.IsTemplate)
                        .ToList();

                    return views.Select(vt => vt.ToString()).ToList();
                });
            });

            _webCommands.StartLocalServer(Dispatcher, Webview);
        }

        protected override void OnClosed(EventArgs e)
        {
            _webCommands?.Dispose(); // Gracefully shut down the server
            _eventCaller?.Dispose();
            base.OnClosed(e);
        }
    }
}
