using Microsoft.Web.WebView2.Wpf;
using Autodesk.Revit.UI;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System;

namespace RealRevitPlugin.WpfWindow.Web.Core.WebEvents
{
    public class WebEventsHandler
    {
        private readonly CommandRegistry _commandRegistry;
        public WebEventsHandler(CommandRegistry commandRegistry)
        {
            _commandRegistry = commandRegistry;
        }

        public void HandleWebviewEvents(WebView2 webView)
        {
            webView.CoreWebView2InitializationCompleted += async (sender, args) =>
            {
                if (args.IsSuccess)
                {
                    Debug.WriteLine("WebView2 initialization successful");
                    webView.CoreWebView2.WebMessageReceived += async (s, e) =>
                    {
                        string message = e.TryGetWebMessageAsString();
                        Debug.WriteLine($"Received message: {message}");

                        var response = await _commandRegistry.HandleCommand(message);
                        var jsonResponse = JsonConvert.SerializeObject(response);
                        Debug.WriteLine($"Sending response: {jsonResponse}");

                        try
                        {
                            webView.CoreWebView2.PostWebMessageAsString(jsonResponse);
                        }
                        catch (Exception ex)
                        {

                            TaskDialog.Show("Error", $"Failed to send response: {ex.Message}");
                        }
                    };
                }
                else
                {
                    TaskDialog.Show("Error", $"WebView2 initialization failed: {args.InitializationException}");
                }
            };
        }
    }
}
