using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRevitPlugin.WpfWindow.Web.Core.WebEvents {
    public class GlobalState : IDisposable {
        private readonly Dictionary<Type, object> _states = new Dictionary<Type, object>();

        public void Register<T>(T state) where T : IDisposable {
            _states[typeof(T)] = state;
        }

        public State<T> GetState<T>() where T : class {
            if (!_states.TryGetValue(typeof(T), out var state)) {
                throw new InvalidOperationException($"State of type {typeof(T).Name} has not been registered");
            }
            return new State<T>((T)state);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                foreach (var kvp in _states) { // Fix: Use explicit KeyValuePair instead of deconstruction
                    if (kvp.Value is IDisposable disposable) {
                        disposable.Dispose();
                    }
                }
            }
        }
    }

}
