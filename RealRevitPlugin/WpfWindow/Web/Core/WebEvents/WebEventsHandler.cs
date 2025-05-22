using Microsoft.Web.WebView2.Wpf;
using Autodesk.Revit.UI;
using System.Text.Json;
using System.Threading.Tasks;

namespace RealRevitPlugin.WpfWindow.Web.Core.WebEvents {
    public class WebEventsHandler {
        private readonly CommandRegistry _commandRegistry;
        public WebEventsHandler(CommandRegistry commandRegistry) {
            _commandRegistry = commandRegistry;
        }

        public void HandleWebviewEvents(WebView2 webView) {
            webView.CoreWebView2InitializationCompleted += async (sender, args) => {
                if (args.IsSuccess) {
                    webView.CoreWebView2.WebMessageReceived += async (s, e) => {
                        string message = e.TryGetWebMessageAsString();
                        var response = await _commandRegistry.HandleCommand(message);
                        var jsonResponse = JsonSerializer.Serialize(response);
                        webView.CoreWebView2.PostWebMessageAsString(jsonResponse);
                    };
                }
                else {
                    TaskDialog.Show("Error", $"WebView2 initialization failed: {args.InitializationException}");
                }
            };
        }
    }
}
