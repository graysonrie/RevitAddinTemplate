using System.IO;

namespace RealRevitPlugin.WpfWindow.Web.Core {
    public class WebWindowConfig {
        public int Port { get; private set; } = 3000;
        public string WebRootPath { get; private set; }
        public WebWindowConfig(int port = 3000, string? webRootPath = null) {
            Port = port;
            string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string assemblyDir = Path.GetDirectoryName(assemblyPath);
            WebRootPath = webRootPath ?? Path.Combine(assemblyDir, "web");
        }
    }
}
