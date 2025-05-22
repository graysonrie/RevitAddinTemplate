using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using RealRevitPlugin.WpfWindow.Web.Core.WebEvents;
using RealRevitPlugin.WpfWindow.Web.Core;
using System.Windows.Threading;
using Microsoft.Web.WebView2.Wpf;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace RealRevitPlugin.WpfWindow.Web
{
    public class WebCommands : IDisposable
    {
        private readonly GlobalState _globalState = new GlobalState();
        private readonly CommandRegistry _commandRegistry;

        private readonly WebWindowHandler _windowHandler;

        public WebWindowConfig Config { get; private set; }
        public WebCommands(WebWindowConfig config = null, JsonSerializerOptions jsonOptions = null)
        {
            jsonOptions = jsonOptions ?? new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            Config = config ?? new WebWindowConfig();
            _commandRegistry = new CommandRegistry(jsonOptions);
            _windowHandler = new WebWindowHandler(Config, _commandRegistry);
        }
        /// <summary>
        /// Starts the local server and initializes the WebView2 control.
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="webView"></param>
        public void StartLocalServer(Dispatcher dispatcher, WebView2 webView)
        {
            dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    await _windowHandler.StartLocalServer(webView);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error", $"Error starting local server ${ex.Message}");
                }
            });
        }

        #region Registration methods
        public void RegisterState<T>(T state) where T : IDisposable
        {
            _globalState.Register(state);
        }
        /// <summary>
        /// Get access to the registered state
        /// </summary>
        public GlobalStateAccessor State
        {
            get
            {
                return new GlobalStateAccessor(_globalState);
            }
        }
        public void RegisterAsyncCommand<TArgs, TResponse>(string commandName, Func<TArgs, Task<TResponse>> func) where TArgs : class where TResponse : class
        {
            _commandRegistry.RegisterCommand<TArgs, TResponse>(commandName, async (args) =>
            {
                var output = await func(args);
                return new Result<TResponse>(output);
            });
        }
        public void RegisterCommand<TArgs, TResponse>(string commandName, Func<TArgs, TResponse> func) where TArgs : class where TResponse : class
        {
            _commandRegistry.RegisterCommand<TArgs, TResponse>(commandName, async (args) =>
            {
                var output = await Task.Run(() => func(args));
                return new Result<TResponse>(output);
            });
        }
        // Registering commands with no args:
        public void RegisterAsyncCommand<TResponse>(string commandName, Func<Task<TResponse>> func) where TResponse : class
        {
            _commandRegistry.RegisterCommand<object, TResponse>(commandName, async (args) =>
            {
                var output = await func();
                return new Result<TResponse>(output);
            });
        }
        public void RegisterCommand<TResponse>(string commandName, Func<TResponse> func) where TResponse : class
        {
            _commandRegistry.RegisterCommand<object, TResponse>(commandName, async (args) =>
            {
                var output = await Task.Run(() => func());
                return new Result<TResponse>(output);
            });
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            _windowHandler.Dispose();
            _globalState.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
