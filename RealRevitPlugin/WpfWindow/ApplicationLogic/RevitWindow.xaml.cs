using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace RealRevitPlugin.WpfWindow.ApplicationLogic
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RevitWindow : Window
    {
        private readonly RevitEventCaller _eventCaller;

        public RevitWindow()
        {
            InitializeComponent();
            _eventCaller = new RevitEventCaller();
        }

        private void OnTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
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

        protected override void OnSourceInitialized(System.EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            _eventCaller?.Dispose();
        }
    }
}
