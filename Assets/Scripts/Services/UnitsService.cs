using System;
using System.Collections.Generic;
using Data;
using Leopotam.EcsLite;
using Locator;
using Models.Aspects;
using Models.Components;
using Services.PrefabPool;
using UniRx;
using UnityEngine;
using Views;

namespace Services
{
    public interface ISpawn
    {
        public IReadOnlyReactiveProperty<bool> ViewsIsSpawned { get; }
        void SpawnViews();
        void DeSpawnViews();
        void SetSpawned(bool val);
    }

    public interface IUnitsService
    {
        Dictionary<int, UnitView> Units { get; set; }

    }

    public class UnitsService : ITick, IUnitsService, ISpawn, IDataHandler<LevelSaveData>
    {
        private readonly ReactiveProperty<AspectUnit> _selected = new ReactiveProperty<AspectUnit>();

        private readonly List<MonoPoolableObject> _spawnedViews = new List<MonoPoolableObject>();
        private readonly ReactiveProperty<bool> _viewsIsSpawned = new ReactiveProperty<bool>();
        private readonly DataContainer<VisualData> _visualData;
        private readonly EcsWorld _world;
        private readonly EcsFilter _unitFilter;
        public IReadOnlyReactiveProperty<bool> ViewsIsSpawned => _viewsIsSpawned;
        public Dictionary<int, UnitView> Units { get; set; } = new Dictionary<int, UnitView>();
        public UnitsService()
        {
            _visualData = Container.Get<DataContainer<VisualData>>();
            _world = Container.Get<EcsWorld>();
            _unitFilter = _world.Filter<ComponentUnit>().Inc<ComponentTransform>().End();
        }

        public void SaveData(LevelSaveData data)
        {
            data.ComponentUnitSaveData.Clear();
            _world.GetPool<ComponentUnit>();
            var filter = _world.Filter<ComponentUnit>().Inc<ComponentTransform>().Inc<ComponentSelection>().End();
            foreach (var i in filter)
            {
                var c1 = _world.GetPool<ComponentUnit>().Get(i);
                var c2 = _world.GetPool<ComponentTransform>().Get(i);
                var c3 = _world.GetPool<ComponentSelection>().Get(i);

                data.ComponentUnitSaveData.Add(new SaveDataUnit()
                {
                    Id = i,
                    Position = c2.Position,
                    Rotation = c2.Rotation.eulerAngles.y,
                    ConfigId = c1.ConfigId,
                    PlayerIndex = c1.PlayerIndex,
                    Selected = c3.Selected,
                    Selectable = c3.Selectable,
                });
            }

            var filter1 = _world.Filter<ComponentMove>().End();
            data.ComponentMoveSaveData.Clear();
            foreach (var i in filter1)
            {
                var c1 = _world.GetPool<ComponentMove>().Get(i);
                data.ComponentMoveSaveData.Add(new SaveDataMove(i,c1));
            }

            var filter2 = _world.Filter<ComponentProductionSchema>().End();
            data.ComponentProductionSchemaSaveData.Clear();
            foreach (var i in filter2)
            {
                var c1 = _world.GetPool<ComponentProductionSchema>().Get(i);
                data.ComponentProductionSchemaSaveData.Add(new SaveDataProductionSchema(i, c1));
            }

            var filter3 = _world.Filter<ComponentProductionQueue>().End();
            data.ComponentProductionQueueSaveData.Clear();
            foreach (var i in filter3)
            {
                var c1 = _world.GetPool<ComponentProductionQueue>().Get(i);
                data.ComponentProductionQueueSaveData.Add(new SaveDataProductionQueue(i,c1));
            }

            var filter4 = _world.Filter<ComponentMoveTarget>().End();
            data.ComponentMoveTargetSaveData.Clear();
            foreach (var i in filter3)
            {
                var c1 = _world.GetPool<ComponentMoveTarget>().Get(i);
                data.ComponentMoveTargetSaveData.Add(new SaveDataMoveTarget(i,c1));
            }

        }

        public void LoadData(LevelSaveData data)
        {

            foreach (var save in data.ComponentUnitSaveData)
            {
                var entity = _world.NewEntity();

                var pool = _world.GetPool<ComponentUnit>();
                ref var c1 = ref pool.Add(entity);
                c1.ConfigId = save.ConfigId;
                c1.PlayerIndex = save.PlayerIndex;
                c1.IsUser = true;

                ref var c2 = ref _world.GetPool<ComponentTransform>().Add(entity);
                c2.Position = save.Position;
                c2.Rotation = Quaternion.Euler(0,save.Rotation,0);

                ref var c3 = ref _world.GetPool<ComponentSelection>().Add(entity);
                c3.Selectable = save.Selectable;
                c3.Selected = save.Selected;
            }

            foreach (var save in data.ComponentProductionSchemaSaveData)
            {
                var pool1 = _world.GetPool<ComponentProductionSchema>();
                foreach (var e in _unitFilter)
                {
                    if (save.Id == e)
                    {
                        ref var c1 = ref pool1.Add(e);
                        c1.Variants = save.ProductionVariants;
                    }

                }
            }

            foreach (var save in data.ComponentMoveSaveData)
            {
                var pool1 = _world.GetPool<ComponentMove>();
                foreach (var e in _unitFilter)
                {
                    if (save.Id == e)
                    {
                        ref var c1 = ref pool1.Add(e);
                        c1.MoveAcc = save.Acceleration;
                        c1.MoveSpeed = save.Speed;
                        c1.RotationSpeed = save.RotationSpeed;
                    }

                }
            }
            foreach (var save in data.ComponentProductionSchemaSaveData)
            {
                var pool1 = _world.GetPool<ComponentProductionSchema>();
                foreach (var e in _unitFilter)
                {
                    if (save.Id == e)
                    {
                        ref var c1 = ref pool1.Add(e);
                        c1.Variants = save.ProductionVariants;
                    }

                }
            }
        }


        public void SpawnViews()
        {

            var filter = _world.Filter<ComponentUnit>().Inc<ComponentTransform>().Inc<ComponentSelection>().End();

            foreach (var i in filter)
            {
                var unit = _world.GetPool<ComponentUnit>().Get(i);
                var tr = _world.GetPool<ComponentTransform>().Get(i);
                var selection = _world.GetPool<ComponentSelection>().Get(i);
                var view = CreateView(unit, tr);
                _spawnedViews.Add(view);
                Units.Add(i, view);
            }

        }

        private UnitView CreateView(ComponentUnit unit, in ComponentTransform tr)
        {
            var id = unit.ConfigId;
            var prefab = _visualData.Data.GetView(id);
            if (prefab == null)
            {
                Debug.LogError($"missing prefab id {id}");
                return null;
            }

            var view = PrefabPool.PrefabPool.InstanceGlobal.Spawn(prefab);
            view.transform.position = tr.Position;
            view.transform.rotation = tr.Rotation;
            view.Bind(unit.UnitIndex);

            return view;
        }

        public void DeSpawnViews()
        {
            _spawnedViews.ForEach(o => o.Dispose());
            _spawnedViews.Clear();
            Units.Clear();
        }

        public void SetSpawned(bool val)
        {
            _viewsIsSpawned.Value = val;
        }

        public void Tick(float dt)
        {
        }

        


    }


    public static class Ext
    {
        public static T Get<T>(this in int index) where T : class
        {
            return Storage<T>.Instance.Get(index);
        }

        public static T Set<T>(this in int index, T aspect) where T : class
        {
            return Storage<T>.Instance.Set(aspect, index);
        }

        public static void Remove<T>(this in int index) where T : class
        {
            Storage<T>.Instance.Remove(index);
        }

        public static bool Has<T>(this in int index) where T : class
        {
            return Storage<T>.Instance.Has(index);
        }
    }
}