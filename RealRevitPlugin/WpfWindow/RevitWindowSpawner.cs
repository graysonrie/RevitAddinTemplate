using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using Autodesk.Revit.UI;
using RealRevitPlugin.WpfWindow.ApplicationLogic;

namespace RealRevitPlugin
{
    public class RevitWindowSpawner
    {
        private static RevitWindow? _window;
        /// <summary>
        ///  Creates an instance of the custom window. If the window is already open, it will bring it to the front.
        /// </summary>
        /// <returns></returns>
        public static Result Spawn()
        {
            if (_window is null || !_window.IsLoaded)
            {
                try
                {
                    _window = new RevitWindow();
                    // Make Revit the owner so it behaves like a modal dialog
                    WindowInteropHelper helper = new WindowInteropHelper(_window)
                    {
                        Owner = Process.GetCurrentProcess().MainWindowHandle
                    };

                    _window.Show();
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error", ex.Message);
                    return Result.Failed;
                }
            }
            else
            {
                // Focus the window:
                _window.Activate();
            }
            return Result.Succeeded;
        }
    }
}
