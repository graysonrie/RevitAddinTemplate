using Autodesk.Revit.UI;
using EmbedIO;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RealRevitPlugin.WpfWindow {
    public class WebWindowHandler : IDisposable {
        private WebServer _server;
        public WebWindowConfig Config { get; private set; }
        public WebWindowHandler(WebWindowConfig config) {
            Config = config;
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
    public class WebWindowConfig {
        public int Port { get; private set; } = 3000;
        public string WebRootPath { get; private set; }
        public WebWindowConfig(int port = 3000, string webRootPath = null) {
            Port = port;
            string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string assemblyDir = Path.GetDirectoryName(assemblyPath);
            WebRootPath = webRootPath ?? Path.Combine(assemblyDir, "web");
        }
    }
}
