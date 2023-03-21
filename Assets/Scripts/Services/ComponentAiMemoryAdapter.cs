using Data;
using Leopotam.EcsLite;
using Models.Components;

namespace Services
{
    public class ComponentAiMemoryAdapter : UnitAdapter<ComponentAiMemory, SaveDataAiMemory, ComponentAiMemoryAdapter>
    {
        protected override SaveDataAiMemory Save(ComponentAiMemory c1, int i)
        {
            if (!c1.Target.Unpack(_world, out var entity))
            {
                entity = -1;
            }

            return new SaveDataAiMemory()
            {
                Id = i,
                LastPosition = c1.LastPosition,
                MoveTargetPosition = c1.TargetPosition,
                HasNewTarget = c1.HasNewTarget,
                TargetJobType = c1.TargetJobType,
                Timer = c1.Timer,
                TargetEntity = entity
            };

        }

        protected override void Load(ref ComponentAiMemory c1, SaveDataAiMemory save)
        {
            c1.LastPosition = save.LastPosition;
            c1.TargetPosition = save.MoveTargetPosition;
            c1.HasNewTarget = save.HasNewTarget;
            c1.TargetJobType = save.TargetJobType;
            c1.Timer = save.Timer;
            if (save.TargetEntity >= 0)
            {
                c1.Target = _world.PackEntity(save.TargetEntity);
            }
        }

    }
}