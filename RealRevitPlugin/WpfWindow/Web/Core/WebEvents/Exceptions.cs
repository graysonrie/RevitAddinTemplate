using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRevitPlugin.WpfWindow.Web.Core.WebEvents {
    public class CommandExecutionException : Exception {
        public CommandExecutionException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
