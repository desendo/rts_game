using System.Collections.Generic;
using Data;
using Locator;
using Models;
using Services.PrefabPool;
using UniRx;
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
        public List<UnitModel> Player1Units { get; }
        public List<UnitModel> Player2Units { get; }
    }

    public class UnitsService : ITick, IUnitsService, ISpawn, IDataHandler<LevelSaveData>
    {
        private readonly List<UnitModel> _player1Units = new List<UnitModel>();
        private readonly List<UnitModel> _player2Units = new List<UnitModel>();
        private readonly List<MonoPoolableObject> _spawnedViews = new List<MonoPoolableObject>();
        private readonly ReactiveProperty<bool> _viewsIsSpawned = new ReactiveProperty<bool>();
        private readonly DataContainer<VisualData> _visualData;

        public IReadOnlyReactiveProperty<bool> ViewsIsSpawned => _viewsIsSpawned;

        public UnitsService()
        {
            _visualData = Container.Get<DataContainer<VisualData>>();
        }

        public void Tick(float dt)
        {

        }

        public List<UnitModel> Player1Units => _player1Units;
        public List<UnitModel> Player2Units =>  _player2Units;

        public void SaveData(LevelSaveData data)
        {
            data.UnitsSaveData.Clear();
            foreach (var unit in _player1Units)
            {
                data.UnitsSaveData.Add(new UnitSaveData()
                {
                   Position = unit.AspectTransform.Position.Value,
                   Rotation = unit.AspectTransform.Rotation.Value.eulerAngles.y,
                   ConfigId = unit.ConfigId,
                   ViewId = unit.ViewId,
                   PlayerIndex = unit.PlayerIndex.Value,
                   UnitId = unit.UnitId
                });
            }
        }

        public void LoadData(LevelSaveData data)
        {
            _player1Units.Clear();
            _player2Units.Clear();
            foreach (var unit in data.UnitsSaveData)
            {
                var model = new UnitModel(unit.ConfigId, unit.ViewId, unit.UnitId);
                model.AspectTransform.InitFromSaveData(unit);
                model.PlayerIndex.Value = unit.PlayerIndex;
                if(model.PlayerIndex.Value == 0)
                    _player1Units.Add(model);
                if(model.PlayerIndex.Value == 1)
                    _player2Units.Add(model);
            }
        }

        public void SpawnViews()
        {
            foreach (var unitModelBase in _player1Units)
            {
                var view = CreateView(unitModelBase);
                _spawnedViews.Add(view);
            }
            foreach (var unitModelBase in _player2Units)
            {
                var view = CreateView(unitModelBase);
                _spawnedViews.Add(view);
            }
        }

        private UnitView CreateView(UnitModel unitModel)
        {
            var prefab = _visualData.Data.GetView(unitModel.ViewId);
            var view = PrefabPool.PrefabPool.InstanceGlobal.Spawn(prefab);
            view.Bind(unitModel);

            return view;
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
    }
}