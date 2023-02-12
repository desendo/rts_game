using System.Collections.Generic;
using System.Linq;
using Data;
using Locator;
using UniRx;
using Views.LevelEditorViews;

namespace Services
{
    public interface ILevelService : IDataHandler<PlayerData>
    {
        public IReadOnlyReactiveProperty<bool> LevelDataInitialized { get; }
        public IReactiveCommand<bool> OnGatherLevelConfigStartedRequest { get; }
        public IReadOnlyReactiveProperty<LevelSaveData> CurrentLevelSaveData { get; }
        void InitLevelSaveData();
        void AddUnitLevelEditorData(LevelEditorUnitData levelEditorUnitData);
    }

    public class LevelService : ILevelService
    {
        private readonly ReactiveProperty<bool> _levelDataInitialized = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<LevelSaveData> _levelSaveData = new ReactiveProperty<LevelSaveData>();
        private readonly ReactiveCommand<bool> _onGatherLevelConfigStartedRequest = new ReactiveCommand<bool>();



        public IReadOnlyReactiveProperty<bool> LevelDataInitialized => _levelDataInitialized;
        public IReactiveCommand<bool> OnGatherLevelConfigStartedRequest => _onGatherLevelConfigStartedRequest;
        public IReadOnlyReactiveProperty<LevelSaveData> CurrentLevelSaveData => _levelSaveData;

        private readonly List<LevelEditorUnitData> _levelEditorUnitsData = new List<LevelEditorUnitData>();
        private readonly GameConfigData _config;
        public LevelService()
        {
            _config = Container.Get<DataContainer<GameConfigData>>().Data;
        }
        public void InitLevelSaveData()
        {
            _levelEditorUnitsData.Clear();
            if (CurrentLevelSaveData.Value == null || !CurrentLevelSaveData.Value.IsValid)
            {
                _onGatherLevelConfigStartedRequest.Execute(true);
                _levelSaveData.Value = GenerateFromGatheredData();
            }
            _onGatherLevelConfigStartedRequest.Execute(false);
            _levelDataInitialized.Value = true;
        }

        public void AddUnitLevelEditorData(LevelEditorUnitData levelEditorUnitData)
        {
            if(string.IsNullOrEmpty(levelEditorUnitData.UnitId))
                levelEditorUnitData.UnitId = GenerateUnitId();
            _levelEditorUnitsData.Add(levelEditorUnitData);

            string GenerateUnitId()
            {
                return levelEditorUnitData.ConfigId + "_" + _levelEditorUnitsData.Count;
            }
        }

        private LevelSaveData GenerateFromGatheredData()
        {
            var levelSaveData = new LevelSaveData
            {
                IsValid = true
            };
            for (var i = 0; i < _levelEditorUnitsData.Count; i++)
            {
                var editorData = _levelEditorUnitsData[i];
                var viewId = _config.UnitConfigs.FirstOrDefault(x => x.Id == editorData.ConfigId)?.ViewId;
                levelSaveData.UnitsSaveData.Add( new UnitSaveData()
                {
                    Position = editorData.Position,
                    Rotation = editorData.Rotation,
                    UnitId = editorData.UnitId,
                    ConfigId = editorData.ConfigId,
                    ViewId = viewId,
                    PlayerIndex = editorData.PlayerIndex

                });
            }
            return levelSaveData;

        }

        public void SaveData(PlayerData data)
        {
            data.CurrentLevelSaveData = _levelSaveData.Value;
        }

        public void LoadData(PlayerData data)
        {
            _levelSaveData.Value = data.CurrentLevelSaveData;
        }
    }
}