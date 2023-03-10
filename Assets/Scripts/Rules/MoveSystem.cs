using Leopotam.EcsLite;
using Locator;
using Models.Components;
using UnityEngine;

namespace Rules
{
    public class MoveSystem : IEcsRunSystem
    {
        private readonly EcsFilter _moveFilter;
        private readonly EcsWorld _world;
        private readonly EcsFilter _moveTargetFilter;
        private readonly EcsFilter _moveTargetRotateTo;
        private readonly EcsFilter _moveTargetSimple;

        public MoveSystem()
        {
            _world = Container.Get<EcsWorld>();
            _moveFilter = _world.Filter<ComponentTransform>().Inc<ComponentMove>().End();
            _moveTargetFilter = _world.Filter<ComponentTransform>().Inc<ComponentMove>().Inc<ComponentMoveTarget>()
                .Exc<ComponentMoveTargetAgent>()
                .Exc<ComponentMoveRotateToTarget>()
                .Exc<ComponentMoveTargetSimple>()
                .End();

            _moveTargetRotateTo = _world.Filter<ComponentTransform>().Inc<ComponentMove>()
                .Exc<ComponentMoveTargetAgent>()
                .Exc<ComponentMoveTarget>()
                .Exc<ComponentMoveTargetSimple>()
                .Inc<ComponentMoveRotateToTarget>()
                .End();
            _moveTargetSimple = _world.Filter<ComponentTransform>().Inc<ComponentMove>()
                .Exc<ComponentMoveTargetAgent>()
                .Exc<ComponentMoveTarget>()
                .Exc<ComponentMoveRotateToTarget>()
                .Inc<ComponentMoveTargetSimple>()
                .End();

        }

        public void Run(IEcsSystems systems)
        {
            foreach (var i in _moveTargetFilter)
            {
                ref var c2 = ref _world.GetPool<ComponentTransform>().Get(i);
                var c1 = _world.GetPool<ComponentMoveTarget>().Get(i);
                ref var c3 =  ref _world.GetPool<ComponentMove>().Get(i);

                var targetDirMag = (c1.Target - c2.Position).magnitude;
                if(targetDirMag < 0.1f)
                    continue;

                var targetDir = (c1.Target - c2.Position)/targetDirMag;
                var scalar = Vector3.Dot(c2.Direction, targetDir);

                if (scalar <= 0.95f)
                {
                    ref var c = ref _world.GetPool<ComponentMoveRotateToTarget>().Add(i);
                    c.Target = c1.Target;
                    _world.GetPool<ComponentMoveTarget>().Del(i);
                }
                else
                {
                    ref var c = ref _world.GetPool<ComponentMoveTargetAgent>().Add(i);
                    c.Target = c1.Target;

                }
            }

            foreach (var i in _moveTargetRotateTo)
            {

                ref var c2 = ref _world.GetPool<ComponentTransform>().Get(i);
                var c1 = _world.GetPool<ComponentMoveRotateToTarget>().Get(i);
                ref var c3 =  ref _world.GetPool<ComponentMove>().Get(i);

                var targetDirMag = (c1.Target - c2.Position).magnitude;
                var targetDir = (c1.Target - c2.Position)/targetDirMag;

                var scalar = Vector3.Dot(c2.Direction, targetDir);
                c3.RotationSpeedCurrent = Mathf.Pow(Mathf.Clamp(1f - scalar, 0f, 1f),0.2f) * c3.RotationSpeedMax;

                c2.OldDirection = c2.Direction;
                c2.Direction = Vector3.RotateTowards(c2.Direction, targetDir,
                    Time.deltaTime * c3.RotationSpeedCurrent , 0.0f);;
                c2.OldPosition = c2.Position;
                c2.Position += c2.Direction * c3.MoveSpeedCurrent * Time.deltaTime;
                c2.Delta = c2.Position - c2.OldPosition;
                c2.EffectiveVelocity = c2.Delta / Time.deltaTime;
                if (scalar > 0.9993)
                {
                    c3.RotationSpeedCurrent = 0;
                    _world.GetPool<ComponentMoveRotateToTarget>().Del(i);
                    ref var c = ref _world.GetPool<ComponentMoveTarget>().Add(i);
                    c.Target = c1.Target;
                }
            }
            foreach (var i in _moveTargetSimple)
            {

                ref var c2 = ref _world.GetPool<ComponentTransform>().Get(i);
                var c1 = _world.GetPool<ComponentMoveTargetSimple>().Get(i);
                ref var c3 =  ref _world.GetPool<ComponentMove>().Get(i);

                var targetDirMag = (c1.Target - c2.Position).magnitude;
                var targetDir = (c1.Target - c2.Position)/targetDirMag;

                if (targetDirMag < 0.1f)
                {

                    ref var c0 = ref _world.GetPool<ComponentMoveTarget>().Add(i);
                    c0.Target = c1.Target;
                    _world.GetPool<ComponentMoveTargetSimple>().Del(i);
                    return;
                }

                var scalar = Vector3.Dot(c2.Direction, targetDir);
                c3.RotationSpeedCurrent = Mathf.Pow(Mathf.Clamp(1f - scalar, 0f, 1f),0.2f) * c3.RotationSpeedMax;

                c2.OldDirection = c2.Direction;
                c2.Direction = targetDir;
                c2.OldPosition = c2.Position;
                c2.Position += c2.Direction * c3.MoveSpeedCurrent * Time.deltaTime;
                c2.Delta = c2.Position - c2.OldPosition;
                c2.EffectiveVelocity = c2.Delta / Time.deltaTime;

            }
        }
    }
}