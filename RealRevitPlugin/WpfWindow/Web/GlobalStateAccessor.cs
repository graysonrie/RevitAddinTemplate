using RealRevitPlugin.WpfWindow.Web.Core.WebEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRevitPlugin.WpfWindow.Web {
    public class GlobalStateAccessor {
        private readonly GlobalState _state;
        public GlobalStateAccessor(GlobalState state) {
            _state = state;
        }
        public State<T> Get<T>() where T : class {
            return _state.GetState<T>();
        }
    }
}
