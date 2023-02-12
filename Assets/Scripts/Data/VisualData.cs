using System;
using System.Collections.Generic;
using System.Linq;
using Services.PrefabPool;
using Views;

namespace Data
{
    [Serializable]
    public class VisualData : IData
    {

        public List<ObjectEntry<UnitView>> UnitViews;

        private Dictionary<string, UnitView> _cache =
            new Dictionary<string, UnitView>();

        public UnitView GetView(string id)
        {
            if (_cache.TryGetValue(id, out var obj))
                return obj;

            var target = UnitViews.FirstOrDefault(x => x.Id == id)?.Obj;
            if (target != null)
                _cache.Add(id, target);
            return target;
        }
    }

    [Serializable]
    public class ObjectEntry<T>
    {
        public string Id;
        public T Obj;
    }

    public static class RandomUtils
    {
    }
}