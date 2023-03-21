using System.Collections.Generic;
using System.Linq;
using Data;
using Leopotam.EcsLite;
using Locator;
using Models.Components;
using Services;
using UniRx;
using UnityEngine;

namespace Rules
{
    public class UpdateInfoSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly GameMessenger _messenger;
        private readonly CompositeDisposable _sup = new CompositeDisposable();
        private GameConfigData _config;
        private EcsFilter _filter;
        private readonly LocalizationData _localization;
        private EcsWorld _world;
        private string _lang = "rus";
        private const string Rus = "rus";
        private const string En = "en";

        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        public UpdateInfoSystem()
        {
            _config = Container.Get<GameConfigData>();
            _localization = Container.Get<LocalizationData>();
            _messenger = Container.Get<GameMessenger>();
        }


        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<ComponentSelection>().Inc<ComponentInfo>().Inc<ComponentUnit>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                var c0 = entity.Get<ComponentSelection>(_world);
                if (c0.Hovered || c0.Selected)
                {
                    ref var c1 = ref entity.Get<ComponentInfo>(_world);

                    if (entity.Has<ComponentSource>(_world) && entity.Has<ComponentUnit>(_world))
                    {
                        var c2 = entity.Get<ComponentSource>(_world);
                        var c3 = entity.Get<ComponentUnit>(_world);
                        c1.Name = GetName(c3.ConfigId);
                        c1.Title = GetName($"{c3.ConfigId}_title");
                        c1.Description = $"{GetName($"{c3.ConfigId}_description")}\n{GetName(c2.Type.ToString())}:<color=#57100E>{c2.Amount:F1}";

                    }
                }
            }
        }

        private string GetName(string id)
        {
            var resultId = $"{_lang}_{id}";
            if (_cache.ContainsKey(resultId))
            {
                return _cache[resultId];
            }

            var val = _localization.LocalizationConfigs.FirstOrDefault(x => x.Id == id);
            if (val == null)
            {
                _cache.Add(resultId,resultId);
                return resultId;
            }

            if (_lang == En)
            {
                _cache.Add(resultId, val.En);
                return val.En;
            }
            else
            {
                _cache.Add(resultId, val.Rus);
                return val.Rus;
            }
        }
    }
}