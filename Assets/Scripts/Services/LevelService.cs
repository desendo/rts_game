using System.Collections.Generic;
using System.Linq;
using Data;
using Locator;
using UniRx;
using UnityEngine;
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
        void SetMapCorners(Transform[] worldCorners);
        Transform[] WorldCorners { get; }
        float MapXMin { get; }
        float MapXMax { get; }
        float MapYMin { get; }
        float MapYMax { get; }
        Vector3 GetPlaneCoordinates(Vector2 normalized);
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
        private Transform[] _worldCorners;
        public Transform[] WorldCorners => _worldCorners;
        public float MapXMin { get; private set; }
        public float MapXMax { get; private set; }
        public float MapYMin { get; private set; }
        public float MapYMax { get; private set; }
        public Vector3 GetPlaneCoordinates(Vector2 normalized)
        {
            return new Vector3(MapXMin + (MapXMax - MapXMin) * normalized.x,0, MapYMin + (MapYMax - MapYMin) * normalized.y);
        }

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
            if(string.IsNullOrEmpty(levelEditorUnitData.Id))
                levelEditorUnitData.Id = GenerateId();
            _levelEditorUnitsData.Add(levelEditorUnitData);

            string GenerateId()
            {
                return levelEditorUnitData.ConfigId + "_" + _levelEditorUnitsData.Count;
            }
        }

        public void SetMapCorners(Transform[] worldCorners)
        {
            _worldCorners = worldCorners;

            foreach (var worldCorner in _worldCorners)
            {
                if (worldCorner.position.x < MapXMin)
                    MapXMin = worldCorner.position.x;
                if (worldCorner.position.x > MapXMax)
                    MapXMax = worldCorner.position.x;
                if (worldCorner.position.z < MapYMin)
                    MapYMin = worldCorner.position.z;
                if (worldCorner.position.z > MapYMax)
                    MapYMax = worldCorner.position.z;
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
                var id = editorData.ConfigId;
                levelSaveData.ComponentUnitSaveData.Add( new SaveDataUnit
                {
                    Id = i,
                    ConfigId = editorData.ConfigId,
                    Position = editorData.Position,
                    Rotation = editorData.Rotation,
                    PlayerIndex = editorData.PlayerIndex
                });

                var attackConfig = _config.AttackConfigs.FirstOrDefault(x => x.Id == id);
                if (attackConfig != null)
                    levelSaveData.ComponentAttackSaveData.Add(new SaveDataAttack(attackConfig, i));

                var moveConfig = _config.MoveConfigs.FirstOrDefault(x => x.Id == id);
                if (moveConfig != null)
                    levelSaveData.ComponentMoveSaveData.Add(new SaveDataMove(moveConfig, i));

                var productionConfig = _config.ProductionVariantsConfigs.FirstOrDefault(x => x.Id == id);
                if (productionConfig != null)
                    levelSaveData.ComponentProductionSchemaSaveData.Add(new SaveDataProductionVariants(i, productionConfig));

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