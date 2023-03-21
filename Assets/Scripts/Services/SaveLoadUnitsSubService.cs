using Data;
using Leopotam.EcsLite;
using Locator;
using Models.Components;

namespace Services
{
    public class SaveLoadUnitsSubService : IDataHandler<LevelSaveData>
    {
        public void SaveData(LevelSaveData data)
        {
            ComponentUnitAdapter.Instance.Save(data.ComponentUnitSaveData);

            ComponentSourceAdapter.Instance.Save(data.ComponentSourceSaveData);
            ComponentRequiredJobAdapter.Instance.Save(data.ComponentRequiredJobSaveData);
            ComponentProductionSchemaAdapter.Instance.Save(data.ComponentProductionSchemaSaveData);
            ComponentAiMemoryAdapter.Instance.Save(data.ComponentAiMemorySaveData);
            ComponentMoveAdapter.Instance.Save(data.ComponentMoveSaveData);
            ComponentMoveTargetAdapter.Instance.Save(data.ComponentMoveTargetSaveData);
            ComponentProductionQueueAdapter.Instance.Save(data.ComponentProductionQueueSaveData);
        }

        public void LoadData(LevelSaveData data)
        {
            ComponentUnitAdapter.Instance.Load(data.ComponentUnitSaveData);

            ComponentSourceAdapter.Instance.Load(data.ComponentSourceSaveData);
            ComponentRequiredJobAdapter.Instance.Load(data.ComponentRequiredJobSaveData);
            ComponentProductionSchemaAdapter.Instance.Load(data.ComponentProductionSchemaSaveData);
            ComponentAiMemoryAdapter.Instance.Load(data.ComponentAiMemorySaveData);
            ComponentMoveAdapter.Instance.Load(data.ComponentMoveSaveData);
            ComponentMoveTargetAdapter.Instance.Load(data.ComponentMoveTargetSaveData);
            ComponentProductionQueueAdapter.Instance.Load(data.ComponentProductionQueueSaveData);

            //assure ids = ents
            var world = Container.Get<EcsWorld>();
            foreach (var e in world.Filter<ComponentUnit>().Inc<ComponentTransform>().End())
            {
                ref var c = ref world.GetPool<ComponentUnit>().Get(e);
                c.EntityIndex = e;
            }
        }
    }
}