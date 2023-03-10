using System.Collections.Generic;
using System.Linq;
using Data;
using Leopotam.EcsLite;
using Locator;
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

        void SetSelected(int i);
        IReadOnlyReactiveProperty<int> SelectedUnit { get; }
        void CreateUnit(string id, Vector3 pos, Quaternion rot, out int unitEntity);
        UnitView ShowPreview(string objResultId, Vector3 planePoint);
        void HidePreview();
        void SpawnMoveMarker(Vector3 objClick);
        IReadOnlyReactiveProperty<int> CurrentPlayerIndex { get; }

    }

    public class UnitsService : ITick, IUnitsService, ISpawn, IDataHandler<LevelSaveData>
    {
        private readonly ReactiveProperty<int> _selected = new ReactiveProperty<int>(-1);

        private readonly List<MonoPoolableObject> _spawnedViews = new List<MonoPoolableObject>();
        private readonly ReactiveProperty<bool> _viewsIsSpawned = new ReactiveProperty<bool>();
        private readonly DataContainer<VisualData> _visualData;
        private readonly EcsWorld _world;
        private readonly EcsFilter _unitFilter;
        private GameConfigData _config;
        private UnitView _preview;
        private readonly ReactiveProperty<int> _currentPlayer = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<bool> ViewsIsSpawned => _viewsIsSpawned;
        public Dictionary<int, UnitView> Units { get; set; } = new Dictionary<int, UnitView>();
        public void SetSelected(int i)
        {
            _selected.Value = i;
        }

        public IReadOnlyReactiveProperty<int> SelectedUnit => _selected;
        public IReadOnlyReactiveProperty<int> CurrentPlayerIndex => _currentPlayer;

        public UnitsService()
        {
            _visualData = Container.Get<DataContainer<VisualData>>();
            _config = Container.Get<GameConfigData>();
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
                    Direction = c2.Direction,
                    EffectiveVelocity = c2.EffectiveVelocity,
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
                data.ComponentProductionSchemaSaveData.Add(new SaveDataProductionVariants(i, c1));
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
            foreach (var i in filter4)
            {
                var c1 = _world.GetPool<ComponentMoveTarget>().Get(i);
                data.ComponentMoveTargetSaveData.Add(new SaveDataMoveTarget(i,c1.Target));
            }
            var filter5 = _world.Filter<ComponentMoveTargetAgent>().End();
            data.ComponentMoveAgentTargetsData.Clear();
            foreach (var i in filter5)
            {
                var c1 = _world.GetPool<ComponentMoveTargetAgent>().Get(i);
                data.ComponentMoveAgentTargetsData.Add(new SaveDataMoveTarget(i,c1.Target));
            }
            var filter6 = _world.Filter<ComponentMoveRotateToTarget>().End();
            data.ComponentMoveRotateTargetsData.Clear();
            foreach (var i in filter6)
            {
                var c1 = _world.GetPool<ComponentMoveRotateToTarget>().Get(i);
                data.ComponentMoveRotateTargetsData.Add(new SaveDataMoveTarget(i,c1.Target));
            }

        }

        public void LoadData(LevelSaveData data)
        {

            foreach (var save in data.ComponentUnitSaveData)
            {
                var entity = _world.NewEntity();
                var pool = _world.GetPool<ComponentUnit>();
                ref var c1 = ref pool.Add(entity);
                c1.EntityIndex = save.Id;
                c1.ConfigId = save.ConfigId;
                c1.PlayerIndex = save.PlayerIndex;
                c1.IsUser = true;

                ref var c2 = ref _world.GetPool<ComponentTransform>().Add(entity);
                c2.Position = save.Position;
                c2.EffectiveVelocity = save.EffectiveVelocity;
                c2.Direction = save.Direction;
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
                    if(!_world.GetPool<ComponentUnit>().Has(e))
                        continue;

                    if (save.Id == _world.GetPool<ComponentUnit>().Get(e).EntityIndex)
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
                    if(!_world.GetPool<ComponentUnit>().Has(e))
                        continue;

                    if (save.Id == _world.GetPool<ComponentUnit>().Get(e).EntityIndex)
                    {
                        ref var c1 = ref pool1.Add(e);
                        c1.MoveAcc = save.Acceleration;
                        c1.MoveSpeedCurrent = save.Speed;
                        c1.RotationSpeedMax = save.RotationSpeed;
                    }

                }
            }
            foreach (var save in data.ComponentProductionQueueSaveData)
            {
                var pool1 = _world.GetPool<ComponentProductionQueue>();
                foreach (var e in _unitFilter)
                {
                    if(!_world.GetPool<ComponentUnit>().Has(e))
                        continue;

                    if (save.Id == _world.GetPool<ComponentUnit>().Get(e).EntityIndex)
                    {
                        ref var c1 = ref pool1.Add(e);
                        c1.Queue = save.List.ToList();
                    }

                }
            }
            foreach (var save in data.ComponentMoveTargetSaveData)
            {
                foreach (var e in _unitFilter)
                {
                    if(!_world.GetPool<ComponentUnit>().Has(e))
                        continue;

                    if (save.Id == _world.GetPool<ComponentUnit>().Get(e).EntityIndex)
                    {
                        ref var c1 = ref _world.GetPool<ComponentMoveTarget>().Add(e);
                        c1.Target = save.Target;
                    }

                }
            }
            foreach (var save in data.ComponentMoveRotateTargetsData)
            {
                foreach (var e in _unitFilter)
                {
                    if(!_world.GetPool<ComponentUnit>().Has(e))
                        continue;

                    if (save.Id == _world.GetPool<ComponentUnit>().Get(e).EntityIndex)
                    {
                        ref var c1 = ref _world.GetPool<ComponentMoveTarget>().Add(e);
                        c1.Target = save.Target;
                    }

                }
            }
            foreach (var save in data.ComponentMoveAgentTargetsData)
            {
                foreach (var e in _unitFilter)
                {
                    if(!_world.GetPool<ComponentUnit>().Has(e))
                        continue;

                    if (save.Id == _world.GetPool<ComponentUnit>().Get(e).EntityIndex)
                    {

                        ref var c1 = ref _world.GetPool<ComponentMoveTarget>().Add(e);
                        c1.Target = save.Target;
                    }

                }
            }

            //assure ids = ents
            foreach (var e in _unitFilter)
            {
                ref var c  = ref _world.GetPool<ComponentUnit>().Get(e);
                c.EntityIndex = e;
            }
        }


        public void SpawnViews()
        {
            var filter = _world.Filter<ComponentUnit>().Inc<ComponentTransform>().End();

            foreach (var i in filter)
            {
                var unit = _world.GetPool<ComponentUnit>().Get(i);

                var tr = _world.GetPool<ComponentTransform>().Get(i);

                var view = CreateView(unit, tr);
                _spawnedViews.Add(view);
                Units.Add(i, view);
            }
        }

        public void CreateUnit(string id, Vector3 pos, Quaternion rot, out int e)
        {
            e = _world.NewEntity();
            ref var c1 = ref _world.GetPool<ComponentUnit>().Add(e);
            c1.ConfigId = id;
            c1.EntityIndex = e;

            ref var c2 = ref _world.GetPool<ComponentTransform>().Add(e);
            c2.Position = pos;
            c2.Rotation = rot;
            _world.GetPool<ComponentSelection>().Add(e);
            var moveConfig = _config.MoveConfigs.FirstOrDefault(x => x.Id == id);
            if (moveConfig != null)
            {
                ref var c5 = ref _world.GetPool<ComponentMove>().Add(e);
                c5.MoveAcc = moveConfig.Acceleration;
                c5.RotationSpeedMax = moveConfig.RotationSpeed;
                c5.MoveSpeedCurrent = moveConfig.Speed;
            }
            var productionSchemaConfig = _config.ProductionVariantsConfigs.FirstOrDefault(x => x.Id == id);
            if (productionSchemaConfig != null)
            {
                ref var c5 = ref _world.GetPool<ComponentProductionSchema>().Add(e);

                c5.Variants = new ProductionVariant[productionSchemaConfig.ProductionVariantConfigs.Count];
                for (var index = 0; index < productionSchemaConfig.ProductionVariantConfigs.Count; index++)
                {
                    var variantConfig = productionSchemaConfig.ProductionVariantConfigs[index];
                    c5.Variants[index] = new ProductionVariant()
                    {
                        PriceAmount = variantConfig.Price.Select(x=>x.Amount).ToArray(),
                        PriceType = variantConfig.Price.Select(x=>x.Type).ToArray(),
                        Duration = variantConfig.Duration,
                        ResultId = variantConfig.ResultId
                    };
                }
            }
            var view = CreateView(c1, c2);
            _spawnedViews.Add(view);
            Units.Add(e, view);

        }

        public UnitView ShowPreview(string objResultId, Vector3 planePoint)
        {
            var preview = _visualData.Data.GetView($"{objResultId}_preview");
            _preview = PrefabPool.PrefabPool.InstanceGlobal.Spawn(preview);
            _preview.transform.position = planePoint;
            _preview.SetId(objResultId);
            return _preview;
        }
        public void HidePreview()
        {
            if (_preview != null)
            {
                PrefabPool.PrefabPool.InstanceGlobal.Despawn(_preview);
                _preview = null;
            }
        }

        public void SpawnMoveMarker(Vector3 objClick)
        {
            var marker = PrefabPool.PrefabPool.InstanceGlobal.Spawn(_visualData.Data.MarkerView);
            marker.transform.position = objClick;
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
            
            view.Bind(unit.EntityIndex);

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
        public static ref T Get<T>(this in int index, EcsWorld world) where T : struct
        {
            var pool = world.GetPool<T>();
            return ref pool.Get(index);

        }
        public static T Set<T>(this in int index, T aspect) where T : class
        {
            return Storage<T>.Instance.Set(aspect, index);
        }

        public static void Remove<T>(this in int index) where T : class
        {
            Storage<T>.Instance.Remove(index);
        }

        public static bool Has<T>(this in int index, EcsWorld world) where T : struct
        {
            var pool = world.GetPool<T>();
            return pool.Has(index);
        }
    }
}