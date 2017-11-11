using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameDevCommon
{
    public sealed class ComponentManager
    {
        private readonly Dictionary<Type, IGameComponent> _componentCache = new Dictionary<Type, IGameComponent>();
        
        public void LoadComponents()
        {
            foreach (var source in new[] { typeof(ComponentManager), GameInstanceProvider.Instance.GetType() })
            {
                var componentInterfaceType = typeof(IGameComponent);
                foreach (var t in source.Assembly.GetTypes().Where(t => t.GetInterfaces().Contains(componentInterfaceType)))
                    GameInstanceProvider.Instance.Components.Add(Activator.CreateInstance(t) as IGameComponent);
            }
        }

        public T GetComponent<T>() where T : IGameComponent
        {
            var tType = typeof(T);

            if (!_componentCache.TryGetValue(tType, out IGameComponent component))
            {
                component = GameInstanceProvider.Instance.Components.FirstOrDefault(c => c.GetType() == tType);
                _componentCache.Add(tType, component);
            }

            return (T)component;
        }
    }
}
