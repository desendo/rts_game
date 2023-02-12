using System;
using System.Collections.Generic;
using System.Linq;
using Services;
using UniRx;
using UnityEngine;

namespace Locator
{
    interface ITick
    {
        public void Tick(float dt);
    }
    interface IInit
    {
        public void Init();
    }
    interface IFixedTick
    {
        public void FixedTick(float dt);
    }

    public static class Container
    {
        private static readonly Dictionary<Type, Type[]> _interfacesByType = new Dictionary<Type, Type[]>();
        private static readonly Dictionary<Type, List<Type>> _typesByInterfaces = new Dictionary<Type, List<Type>>();
        private static readonly Dictionary<Type, List<object>> _instancesByInterfaces = new Dictionary<Type, List<object>>();
        private static readonly Dictionary<Type, object> _instancesByType = new Dictionary<Type, object>();
        private static readonly ReactiveProperty<bool> _bindComplete = new ReactiveProperty<bool>();


        public static IReadOnlyReactiveProperty<bool> BindComplete => _bindComplete;
        static Container()
        {
            _typesByInterfaces.Add(typeof(ITick), new List<Type>());
            _typesByInterfaces.Add(typeof(IInit), new List<Type>());
            _typesByInterfaces.Add(typeof(IFixedTick), new List<Type>());
            _instancesByInterfaces.Add(typeof(IFixedTick), new List<object>());
            _instancesByInterfaces.Add(typeof(IInit), new List<object>());
            _instancesByInterfaces.Add(typeof(ITick), new List<object>());
        }

        public static void UpdateLoop(float dt)
        {
            if(!_bindComplete.Value)
                return;

            var type = typeof(ITick);
            foreach (ITick o in _instancesByInterfaces[type])
            {
                o.Tick(dt);
            }
        }
        public static void FixedUpdateLoop(float dt)
        {
            if(!_bindComplete.Value)
                return;

            var type = typeof(IFixedTick);
            foreach (IFixedTick o in _instancesByInterfaces[type])
            {
                o.FixedTick(dt);
            }
        }
        public static void Init()
        {
            if (!_bindComplete.Value)
            {
                Debug.LogError("Bind not complete before init call");
                return;
            }
            var type = typeof(IInit);
            foreach (IInit o in _instancesByInterfaces[type])
            {
                o.Init();
            }
        }

        public static void Add<T>(T existing = null) where T : class, new()
        {
            var type = typeof(T);
            var interfaces = type.GetInterfaces();


            if (!_interfacesByType.ContainsKey(type))
            {
                if (interfaces.Length != 0)
                {
                    _interfacesByType.Add(type, interfaces);
                    foreach (var singleInterface in interfaces)
                    {
                        if(!_typesByInterfaces.ContainsKey(singleInterface))
                            _typesByInterfaces.Add(singleInterface, new List<Type> { type });
                        else
                            _typesByInterfaces[singleInterface].Add(type);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"duplicate container add {type}. skipping.");
            }
            if (!_instancesByType.ContainsKey(type))
            {
                var instance = existing ?? new T();
                _instancesByType.Add(type, instance);
                if (interfaces.Length != 0)
                {
                    foreach (var singleInterface in interfaces)
                    {
                        if (!_instancesByInterfaces.ContainsKey(singleInterface))
                        {
                            _instancesByInterfaces.Add(singleInterface, new List<object> {instance});
                        }
                        else
                        {
                            _instancesByInterfaces[singleInterface].Add(instance);
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"duplicate container add {type} interfaces. skipping.");
            }
        }

        public static T Get<T>() where T: class
        {
            if (_instancesByType.TryGetValue(typeof(T), out object val))
            {
                return (T)val;
            }
            if (_instancesByInterfaces.TryGetValue(typeof(T), out var list))
            {
                var instance = (T)list.FirstOrDefault();
                if(instance == null)
                    Debug.LogError($"cant resolve instance by {typeof(T)}");
                return instance;
            }

            return default;
        }
        public static List<T> GetList<T>()
        {
            if (_instancesByInterfaces.TryGetValue(typeof(T), out var list))
            {
                var newList = new List<T>();
                foreach (T o in list)
                {
                    newList.Add(o);
                }

                return newList;
            }
            return new List<T>();
        }

        public static void SetBindComplete(bool isComplete)
        {
            _bindComplete.Value = isComplete;
        }
    }
}
