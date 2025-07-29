using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace RealRevitPlugin.AppUtil {
    public static class RibbonPanelExtensions {
        public static PushButton AddPushButton<TCommand>(this RibbonPanel panel, string buttonText) where TCommand : IExternalCommand, new() {
            var command = typeof(TCommand);
            var pushButtonData = new PushButtonData(command.FullName, buttonText, Assembly.GetAssembly(command).Location, command.FullName);
            return (PushButton)panel.AddItem(pushButtonData);
        }
        /// <summary>
        /// Sets the optional small image for this button. Should ideally be 16x16
        /// </summary>
        /// <param name="button"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static RibbonButton SetImage(this RibbonButton button, string path) {
            var uri = ResourcesUtil.GetResource(path);
            var img = new BitmapImage(uri);
            button.Image = img;
            return button;
        }
        /// <summary>
        /// Sets the large image for this button. Should ideally be 32x32
        /// </summary>
        /// <param name="button"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static RibbonButton SetLargeImage(this RibbonButton button, string path) {
            var uri = ResourcesUtil.GetResource(path);
            var img = new BitmapImage(uri);
            button.LargeImage = img;
            return button;
        }
    }
}
