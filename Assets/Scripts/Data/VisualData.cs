using System;
using System.Collections.Generic;
using System.Linq;
using Services.PrefabPool;
using UnityEngine;
using Views;
using Views.UI.Elements;

namespace Data
{
    [Serializable]
    public class VisualData : IData
    {
        public IconButtonView IconButtonView;
        public MarkerView MarkerView;
        public List<ObjectEntry<UnitView>> UnitViews;
        public List<ObjectEntry<Sprite>> Sprites;
        public List<ObjectEntry<AudioClip>> SoundsConfirm;
        public List<ObjectEntry<AudioClip>> SoundsSelect;
        public List<ObjectEntry<AudioClip>> SoundsMove;
        public List<ObjectEntry<AudioClip>> SoundsAttack;

        private Dictionary<string, UnitView> _cache = new Dictionary<string, UnitView>();
        private Dictionary<string, Sprite> _cacheSprites = new Dictionary<string, Sprite>();

        public List<BaseWindow> WindowPrefabs;

        public UnitView GetView(string id)
        {
            if (_cache.TryGetValue(id, out var obj))
                return obj;

            var target = UnitViews.FirstOrDefault(x => x.Id == id)?.Obj;
            if (target != null)
                _cache.Add(id, target);
            return target;
        }

        public Sprite GetSprite(string id)
        {
            if (_cacheSprites.TryGetValue(id, out var obj))
                return obj;

            var target = Sprites.FirstOrDefault(x => x.Id == id)?.Obj;
            if (target != null)
                _cacheSprites.Add(id, target);
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