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
        void CreateUnit(string id, Vector3 pos, Quaternion rot, int playerIndex, out int entity);
        UnitView ShowPreview(string objResultId, Vector3 planePoint);
        void HidePreview();
        void SpawnMoveMarker(Vector3 objClick);
        IReadOnlyReactiveProperty<int> CurrentPlayerIndex { get; }

    }

    public class UnitsService : IUnitsService, ISpawn, IDataHandler<LevelSaveData>, IDataHandler<PlayerData>
    {
        private readonly ReactiveProperty<int> _selected = new ReactiveProperty<int>(-1);

        private readonly List<MonoPoolableObject> _spawnedViews = new List<MonoPoolableObject>();
        private readonly ReactiveProperty<bool> _viewsIsSpawned = new ReactiveProperty<bool>();
        private readonly DataContainer<VisualData> _visualData;
        private readonly EcsWorld _world;
        private readonly GameConfigData _config;
        private UnitView _preview;
        private readonly ReactiveProperty<int> _currentPlayer = new ReactiveProperty<int>();
        private readonly SaveLoadUnitsSubService _subService;
        public IReadOnlyReactiveProperty<int> SelectedUnit => _selected;
        public IReadOnlyReactiveProperty<int> CurrentPlayerIndex => _currentPlayer;

        public IReadOnlyReactiveProperty<bool> ViewsIsSpawned => _viewsIsSpawned;
        public Dictionary<int, UnitView> Units { get; set; } = new Dictionary<int, UnitView>();
        public UnitsService()
        {
            _visualData = Container.Get<DataContainer<VisualData>>();
            _config = Container.Get<GameConfigData>();
            _world = Container.Get<EcsWorld>();
            _subService = new SaveLoadUnitsSubService();
        }
        public void SaveData(LevelSaveData data) => _subService.SaveData(data);
        public void LoadData(LevelSaveData data) => _subService.LoadData(data);
        public void SaveData(PlayerData data) => data.CurrentPlayerIndex = _currentPlayer.Value;
        public void LoadData(PlayerData data) => _currentPlayer.Value = data.CurrentPlayerIndex;

        public void SetSelected(int i)
        {
            _selected.Value = i;
        }

        public void SpawnViews()
        {
            var filter = _world.Filter<ComponentUnit>().Inc<ComponentTransform>().End();

            foreach (var i in filter)
            {
                var unit = _world.GetPool<ComponentUnit>().Get(i);

                ref var tr = ref _world.GetPool<ComponentTransform>().Get(i);

                CreateView(unit, ref tr);
            }
        }

        public void CreateUnit(string id, Vector3 pos, Quaternion rot, int playerIndex, out int entity)
        {
            entity = _world.NewEntity();
            ref var c1 = ref _world.GetPool<ComponentUnit>().Add(entity);
            c1.ConfigId = id;
            c1.PlayerIndex = playerIndex;
            c1.EntityIndex = entity;

            ref var c2 = ref _world.GetPool<ComponentTransform>().Add(entity);
            c2.Position = pos;
            c2.Rotation = rot;

            AddSelection(entity);
            AddMove(id, entity);
            AddProductionSchema(id, entity);
            AddSource(id, entity);
            AddInfo(entity);

            CreateView(c1, ref c2);


        }

        private void AddInfo(in int entity)
        {
            _world.GetPool<ComponentInfo>().Add(entity);
        }

        private void AddSelection(int entity)
        {
            _world.GetPool<ComponentSelection>().Add(entity);
        }

        private void AddMove(string id, int entity)
        {
            var moveConfig = _config.MoveConfigs.FirstOrDefault(x => x.Id == id);
            if (moveConfig != null)
            {
                ref var c5 = ref _world.GetPool<ComponentMove>().Add(entity);
                c5.MoveAcc = moveConfig.Acceleration;
                c5.RotationSpeedMax = moveConfig.RotationSpeed;
                c5.MoveSpeedCurrent = moveConfig.Speed;
            }
        }

        private void AddSource(string id, int entity)
        {

            var resourceConfig = _config.ResourceConfigs.FirstOrDefault(x => x.Id == id);
            if (resourceConfig != null)
            {
                ref var c0 = ref _world.GetPool<ComponentSource>().Add(entity);
                c0.Type = resourceConfig.Type;
                c0.Amount = resourceConfig.Amount;
                if (c0.Type == ResourceType.Wood)
                {
                    ref var job = ref entity.Add<ComponentRequiredJob>(_world);
                    job.Type = JobType.Chop;
                }
            }
        }

        private void AddProductionSchema(string id, int entity)
        {
            var productionSchemaConfig = _config.ProductionVariantsConfigs.FirstOrDefault(x => x.Id == id);
            if (productionSchemaConfig != null)
            {
                ref var c = ref _world.GetPool<ComponentProductionSchema>().Add(entity);

                c.Variants = new ProductionVariant[productionSchemaConfig.ProductionVariantConfigs.Count];
                for (var index = 0; index < productionSchemaConfig.ProductionVariantConfigs.Count; index++)
                {
                    var variantConfig = productionSchemaConfig.ProductionVariantConfigs[index];
                    c.Variants[index] = new ProductionVariant()
                    {
                        PriceAmount = variantConfig.Price.Select(x => x.Amount).ToArray(),
                        PriceType = variantConfig.Price.Select(x => x.Type).ToArray(),
                        Duration = variantConfig.Duration,
                        ResultId = variantConfig.ResultId
                    };
                }
            }
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

        private UnitView CreateView(ComponentUnit unit, ref ComponentTransform tr)
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
            view.transform.forward = tr.Direction;
            view.Bind(unit.EntityIndex);
            _spawnedViews.Add(view);
            Units.Add(unit.EntityIndex, view);
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


    }


    public static class Ext
    {

        public static ref T Get<T>(this in int index, EcsWorld world) where T : struct
        {
            var pool = world.GetPool<T>();
            return ref pool.Get(index);

        }

        public static bool Has<T>(this in int index, EcsWorld world) where T : struct
        {
            var pool = world.GetPool<T>();
            return pool.Has(index);
        }
        public static void Del<T>(this in int index, EcsWorld world) where T : struct
        {
            var pool = world.GetPool<T>();
            if(pool.Has(index))
                pool.Del(index);
        }
        public static ref T Add<T>(this in int index, EcsWorld world) where T : struct
        {
            var pool = world.GetPool<T>();
            if (pool.Has(index))
            {
                return ref pool.Get(index);
            }

            return ref pool.Add(index);
        }
    }
}