﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRevitPlugin.WpfWindow {
    public class RevitContext {
        private static bool _setupComplete = false;
        private static readonly Dictionary<Type, object> _services = [];

        public static string AddinName {
            get {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        public static RevitEventCaller Events {
            get {
                return Resolve<RevitEventCaller>() ?? throw new InvalidOperationException("RevitEventCaller is not registered");
            }
        }

        private static void Register<TService>(TService instance) where TService : class {
            _services[typeof(TService)] = instance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns>null if the service is not registered</returns>
        private static TService? Resolve<TService>() where TService : class {
            if (_services.TryGetValue(typeof(TService), out var instance))
                return instance as TService;

            return null;
        }

        public static void Clear() {
            _services.Clear();
        }

        /// <summary>
        /// Registers all required services. Does nothing if Setup has already been called
        /// </summary>
        public static void Setup() {
            if (_setupComplete) return;
            if (Resolve<RevitEventCaller>() == null) {
                var eventCaller = new RevitEventCaller();
                Register(eventCaller);
            }
            _setupComplete = true;
        }
    }
}
