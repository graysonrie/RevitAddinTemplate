using Autodesk.Revit.UI;
using EmbedIO;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using RealRevitPlugin.WpfWindow.Web.Core.WebEvents;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RealRevitPlugin.WpfWindow.Web.Core {
    public class WebWindowHandler : IDisposable {
        private readonly WebEventsHandler _eventsHandler;

        /// <summary>
        /// The EmbedIO web server instance.
        /// </summary>
        private WebServer _server;
        public WebWindowConfig Config { get; private set; }
        public WebWindowHandler(WebWindowConfig config, CommandRegistry commandRegistry) {
            Config = config;
            _eventsHandler = new WebEventsHandler(commandRegistry);
        }

        public async Task StartLocalServer(WebView2 webview) {
            _server = new WebServer(o => o
                    .WithUrlPrefix($"http://localhost:{Config.Port}/")
                    .WithMode(HttpListenerMode.EmbedIO))
                .WithLocalSessionManager()
                .WithStaticFolder("/", Config.WebRootPath, true);

            _server.RunAsync(); // Run server in background

            // Use a custom data path to avoid permission issues in Revit
            string name = RevitContext.AddinName;
            string safeDataPath = Path.Combine(Path.GetTempPath(), $"{name}_RevitWebView2");
            Directory.CreateDirectory(safeDataPath); // Ensure it exists

            _eventsHandler.HandleWebviewEvents(webview);

            // Wait for WebView2 to initialize
            var env = await CoreWebView2Environment.CreateAsync(userDataFolder: safeDataPath);
            await webview.EnsureCoreWebView2Async(env);

            webview.Source = new Uri($"http://localhost:{Config.Port}/index.html");
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                Dispose();
            }
        }

        public void Dispose() {
            if(_server != null) {
                _server.Dispose();
                _server = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
