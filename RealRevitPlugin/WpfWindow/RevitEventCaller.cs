using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealRevitPlugin.WpfWindow
{
    /// <summary>
    /// Do not instantiate this class other than in the RevitContext. Instead, use <c>RevitContext.Events</c> if you want to perform Revit actions
    /// </summary>
    public class RevitEventCaller : IDisposable
    {
        private readonly RevitEventHandler _handler;
        private readonly ExternalEvent _externalEvent;
        private bool _disposed;

        public RevitEventCaller()
        {
            _handler = new RevitEventHandler();
            _externalEvent = ExternalEvent.Create(_handler);
        }

        public Task<T> Execute<T>(Func<UIApplication, T> action)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RevitEventCaller));

            var tcs = new TaskCompletionSource<T>();
            var wrapper = new ActionWrapper<T>
            {
                Action = action,
                CompletionSource = tcs
            };

            _handler.Enqueue(wrapper);
            _externalEvent.Raise();
            return tcs.Task;
        }

        public Task Execute(Action<UIApplication> action)
        {
            return Execute<object>(app =>
            {
                action(app);
                return null;
            });
        }

        public void Dispose()
        {
            if (_disposed) return;
            _externalEvent?.Dispose();
            _disposed = true;
        }

        private class ActionWrapper<T>
        {
            public Func<UIApplication, T> Action { get; set; }
            public TaskCompletionSource<T> CompletionSource { get; set; }
        }

        private class RevitEventHandler : IExternalEventHandler
        {
            private readonly Queue<object> _actions = new Queue<object>();
            private readonly object _lock = new object();

            public void Enqueue<T>(ActionWrapper<T> wrapper)
            {
                lock (_lock)
                {
                    _actions.Enqueue(wrapper);
                }
            }

            public void Execute(UIApplication app)
            {
                object wrapper = null;
                lock (_lock)
                {
                    if (_actions.Count > 0)
                        wrapper = _actions.Dequeue();
                }

                if (wrapper == null) return;

                try
                {
                    var type = wrapper.GetType();
                    var actionProperty = type.GetProperty("Action");
                    var completionSourceProperty = type.GetProperty("CompletionSource");

                    if (actionProperty?.GetValue(wrapper) is Func<UIApplication, object> action &&
                        completionSourceProperty?.GetValue(wrapper) is TaskCompletionSource<object> tcs)
                    {
                        var result = action(app);
                        tcs.TrySetResult(result);
                    }
                }
                catch (Exception ex)
                {
                    if (wrapper is ActionWrapper<object> objWrapper)
                    {
                        objWrapper.CompletionSource.TrySetException(ex);
                    }
                }
            }

            public string GetName() => "Queued Revit Event Handler";
        }
    }
}
