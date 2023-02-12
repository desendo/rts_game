using System.Collections.Generic;
using UniRx;

namespace Models
{
    public class UnitModel : IModel
    {
        public readonly List<object> Aspects = new List<object>();
        public readonly AspectTransform AspectTransform = new AspectTransform();
        public readonly ReactiveProperty<int> PlayerIndex = new ReactiveProperty<int>();

        public readonly string ConfigId;
        public readonly string ViewId;
        public readonly string UnitId;
        public UnitModel(string configId, string viewId, string unitId)
        {
            ConfigId = configId;
            ViewId = viewId;
            UnitId = unitId;
            Aspects.Add(AspectTransform);
        }
    }
}