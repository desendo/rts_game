using System.Collections.Generic;
using System.Linq;
using Data;
using Locator;
using Models.Aspects;
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
        IReadOnlyReactiveProperty<AspectUnit> CurrentUnitSelected { get; }
        void SetSelected(AspectUnit unit);
        List<UnitView> UnitViews { get; }
        void CreateUnit(Vector3 pos, string configId, List<Vector3> initialPath);

    }

    public class UnitsService : ITick, IUnitsService, ISpawn, IDataHandler<LevelSaveData>
    {
        private readonly ReactiveProperty<AspectUnit> _selected = new ReactiveProperty<AspectUnit>();

        private readonly List<UnitView> _spawnedViews = new List<UnitView>();
        private readonly ReactiveProperty<bool> _viewsIsSpawned = new ReactiveProperty<bool>();
        private readonly DataContainer<VisualData> _visualData;
        private GameConfigData _config;
        public List<UnitView> UnitViews => _spawnedViews;

        public UnitsService()
        {
            _visualData = Container.Get<DataContainer<VisualData>>();
            _config = Container.Get<DataContainer<GameConfigData>>().Data;
        }

        public void SaveData(LevelSaveData data)
        {
            data.AspectsUnitSaveData.Clear();
            for (var i = 0; i < Storage<AspectUnit>.Instance.Aspects.Length; i++)
            {
                var aspect = i.Get<AspectUnit>();
                if (aspect == null)
                    continue;

                data.AspectsUnitSaveData.Add(new AspectUnitSaveData(i, aspect));
            }

            data.AspectsMoveSaveData.Clear();
            for (var i = 0; i < Storage<AspectMove>.Instance.Aspects.Length; i++)
            {
                var aspect = Storage<AspectMove>.Instance.Aspects[i];
                if (aspect != null)
                    data.AspectsMoveSaveData.Add(new AspectMoveSaveData(i, aspect));
            }

            data.AspectsProductionSaveData.Clear();
            for (var i = 0; i < Storage<AspectProduction>.Instance.Aspects.Length; i++)
            {
                var aspect = Storage<AspectProduction>.Instance.Aspects[i];
                if (aspect != null)
                    data.AspectsProductionSaveData.Add(new AspectProductionSaveData(i, aspect));
            }

            data.AspectsQueueSaveData.Clear();
            for (var i = 0; i < Storage<AspectQueue>.Instance.Aspects.Length; i++)
            {
                var aspect = Storage<AspectQueue>.Instance.Aspects[i];
                if (aspect != null)
                    data.AspectsQueueSaveData.Add(new AspectQueueSaveData(i, aspect));
            }
            data.AspectsMoveTargetSaveData.Clear();
            for (var i = 0; i < Storage<AspectMoveTarget>.Instance.Aspects.Length; i++)
            {
                var aspect = Storage<AspectMoveTarget>.Instance.Aspects[i];
                if (aspect != null)
                    data.AspectsMoveTargetSaveData.Add(new AspectMoveTargetSaveData(i, aspect));
            }
        }

        public void LoadData(LevelSaveData data)
        {
            foreach (var save in data.AspectsUnitSaveData)
            {
                save.Id.Set<AspectUnit>(new AspectUnit(save));
                Storage<AspectSelection>.Instance.Set(new AspectSelection(), save.Id);
            }

            foreach (var save in data.AspectsProductionSaveData)
            {
                save.Id.Set<AspectProduction>(new AspectProduction(save));
            }

            foreach (var save in data.AspectsQueueSaveData)
            {
                save.Id.Set<AspectQueue>(new AspectQueue(save));
            }
            foreach (var save in data.AspectsMoveTargetSaveData)
            {
                save.Id.Set<AspectMoveTarget>(new AspectMoveTarget(save));
            }
            foreach (var save in data.AspectsMoveSaveData)
            {
                save.Id.Set<AspectMove>(new AspectMove(save));
            }
        }

        public IReadOnlyReactiveProperty<bool> ViewsIsSpawned => _viewsIsSpawned;

        public void SpawnViews()
        {
            var units = Filter<AspectUnit, AspectSelection>.Instance.IndexList;
            for (var i = 0; i < units.Count; i++)
            {
                if (i.Has<AspectProduction>())
                {
                    var composition = new CompositionUnitFactory(units[i]);
                    var view = CreateView(composition);
                    _spawnedViews.Add(view);
                }
                else
                {
                    var composition = new UnitCompositionBase(units[i]);
                    var view = CreateView(composition);
                    _spawnedViews.Add(view);
                }
            }

            /*foreach (var unitModelBase in _player1Units)
            {
                var view = CreateView(unitModelBase);
                _spawnedViews.Add(view);
            }
            foreach (var unitModelBase in _player2Units)
            {
                var view = CreateView(unitModelBase);
                _spawnedViews.Add(view);
            }*/
        }

        public void DeSpawnViews()
        {
            _spawnedViews.ForEach(o => o.Dispose());
            _spawnedViews.Clear();
        }

        public void SetSpawned(bool val)
        {
            _viewsIsSpawned.Value = val;
        }

        public void CreateUnit(Vector3 pos, string configId, List<Vector3> initialPath)
        {
            var composition = new UnitCompositionBase();
            var aspectUnit = new AspectUnit();
            aspectUnit.Position.Value = pos;
            aspectUnit.Rotation.Value = Quaternion.LookRotation(initialPath[1] - initialPath[0]);
            aspectUnit.ConfigId = configId;
            var newIndex = Storage<AspectUnit>.Instance.Add(aspectUnit);
            aspectUnit.UnitIndex = newIndex;
            var moveConfig = _config.AspectMoveConfigs.FirstOrDefault(x => x.Id == configId);
            newIndex.Set(new AspectMove(new AspectMoveSaveData()
            {
                Acceleration = moveConfig.Acceleration,
                Id = newIndex,
                Speed = moveConfig.Speed,
                RotationSpeed = moveConfig.RotationSpeed
            }));

            composition.AspectUnit = aspectUnit;
            composition.AspectSelection = newIndex.Set(new AspectSelection());
            var view = CreateView(composition);
            _spawnedViews.Add(view);
        }

        public void Tick(float dt)
        {
        }

        public IReadOnlyReactiveProperty<AspectUnit> CurrentUnitSelected => _selected;

        public void SetSelected(AspectUnit unit)
        {
            _selected.Value = unit;
        }

        private UnitView CreateView(UnitCompositionBase composition)
        {
            var id = composition.AspectUnit.ConfigId;
            var prefab = _visualData.Data.GetView(id);
            if (prefab == null)
            {
                Debug.LogError($"missing prefab id {id}");
                return null;
            }

            var view = PrefabPool.PrefabPool.InstanceGlobal.Spawn(prefab);
            view.Bind(composition);

            return view;
        }

        public class CompositionUnitFactory : UnitCompositionBase
        {
            public AspectProduction AspectProduction;
            public CompositionUnitFactory(int i) : base(i)
            {
                AspectProduction = Storage<AspectProduction>.Instance.Aspects[i];
            }
        }
    }

    public class UnitCompositionBase
    {
        public AspectSelection AspectSelection;
        public AspectUnit AspectUnit;

        public UnitCompositionBase(int i)
        {
            AspectUnit = Storage<AspectUnit>.Instance.Aspects[i];
            AspectSelection = Storage<AspectSelection>.Instance.Aspects[i];
        }

        public UnitCompositionBase()
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
        public static void Remove(this in int index)
        {
            //Storage.RemoveAll(index);
        }
        public static bool Has<T>(this in int index) where T : class
        {
            return Storage<T>.Instance.Has(index);
        }
    }
}