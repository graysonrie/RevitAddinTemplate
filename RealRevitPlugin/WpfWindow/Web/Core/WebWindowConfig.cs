using System.IO;
using System.Net;
using System.Net.Sockets;

namespace RealRevitPlugin.WpfWindow.Web.Core {
    public class WebWindowConfig {
        public int Port { get; private set; }
        public string WebRootPath { get; private set; }

        private static int FindAvailablePort() {
            // Create a new TCP listener
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            socket.Listen(1);
            var port = ((IPEndPoint)socket.LocalEndPoint).Port;
            socket.Close();
            return port;
        }

        public WebWindowConfig(int? port = null, string? webRootPath = null) {
            Port = port ?? FindAvailablePort();
            string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string assemblyDir = Path.GetDirectoryName(assemblyPath);
            WebRootPath = webRootPath ?? Path.Combine(assemblyDir, "web");
        }
    }
}
