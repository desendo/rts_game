using System.Collections.Generic;
using Data;
using Leopotam.EcsLite;
using Locator;
using Models.Components;
using UnityEngine;

namespace Services
{
    public class ComponentUnitAdapter
    {
        public ComponentUnitAdapter()
        {
            _world = Container.Get<EcsWorld>();
        }

        public static ComponentUnitAdapter Instance => _instance ??= new ComponentUnitAdapter();
        private static ComponentUnitAdapter _instance;
        private readonly EcsWorld _world;

        public void Save(List<SaveDataUnit> data)
        {
            data.Clear();
            var filter = _world.Filter<ComponentUnit>().Inc<ComponentTransform>().Inc<ComponentSelection>().End();
            foreach (var i in filter)
            {
                var c1 = _world.GetPool<ComponentUnit>().Get(i);
                var c2 = _world.GetPool<ComponentTransform>().Get(i);
                var c3 = _world.GetPool<ComponentSelection>().Get(i);

                data.Add(new SaveDataUnit()
                {
                    Id = i,
                    Position = c2.Position,
                    Rotation = c2.Rotation.eulerAngles.y,
                    Direction = c2.Direction,
                    EffectiveVelocity = c2.EffectiveVelocity,
                    ConfigId = c1.ConfigId,
                    PlayerIndex = c1.PlayerIndex,
                    Selected = c3.Selected,
                    Selectable = c3.Selectable,
                });
            }
        }

        public void Load(List<SaveDataUnit> data)
        {
            foreach (var save in data)
            {
                var entity = _world.NewEntity();
                var pool = _world.GetPool<ComponentUnit>();
                ref var c1 = ref pool.Add(entity);
                c1.EntityIndex = save.Id;
                c1.ConfigId = save.ConfigId;
                c1.PlayerIndex = save.PlayerIndex;

                ref var c2 = ref _world.GetPool<ComponentTransform>().Add(entity);
                c2.Position = save.Position;
                c2.EffectiveVelocity = save.EffectiveVelocity;
                c2.Direction = save.Direction;
                c2.Rotation = Quaternion.Euler(0, save.Rotation, 0);

                ref var c3 = ref _world.GetPool<ComponentSelection>().Add(entity);
                c3.Selectable = save.Selectable;
                c3.Selected = save.Selected;
                _world.GetPool<ComponentInfo>().Add(entity);
            }
        }
    }
}