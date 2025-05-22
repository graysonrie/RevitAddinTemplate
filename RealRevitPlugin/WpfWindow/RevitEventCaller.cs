using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealRevitPlugin.WpfWindow
{
    /// <summary>
    /// Do not instantiate this class other than in the RevitContext. Instead, use <c>RevitContext.Events</c> if you want to perform Revit actions
    /// </summary>
    public class RevitEventCaller : IDisposable {
        private readonly RevitEventHandler _handler;
        private readonly ExternalEvent _externalEvent;
        private bool _disposed;

        public RevitEventCaller() {
            _handler = new RevitEventHandler();
            _externalEvent = ExternalEvent.Create(_handler);
        }

        public Task<T> Execute<T>(Func<UIApplication, T> func) {
            if (_disposed) throw new ObjectDisposedException(nameof(RevitEventCaller));

            var tcs = new TaskCompletionSource<T>();
            _handler.Enqueue(app =>
            {
                try {
                    var result = func(app);
                    tcs.SetResult(result);
                }
                catch (Exception ex) {
                    tcs.SetException(ex);
                }
            });

            _externalEvent.Raise();
            return tcs.Task;
        }

        public Task Execute(Action<UIApplication> action) {
            return Execute<object>(app =>
            {
                action(app);
                return null;
            });
        }

        public void Dispose() {
            if (_disposed) return;
            _externalEvent.Dispose();
            _disposed = true;
        }

        private class RevitEventHandler : IExternalEventHandler {
            private readonly Queue<Action<UIApplication>> _actions = new Queue<Action<UIApplication>>();
            private readonly object _lock = new object();

            public void Enqueue(Action<UIApplication> action) {
                lock (_lock) {
                    _actions.Enqueue(action);
                }
            }

            public void Execute(UIApplication app) {
                Action<UIApplication> action = null;
                lock (_lock) {
                    if (_actions.Count > 0)
                        action = _actions.Dequeue();
                }

                action?.Invoke(app);
            }

            public string GetName() => "Safe Revit Event Handler";
        }
    }

}
