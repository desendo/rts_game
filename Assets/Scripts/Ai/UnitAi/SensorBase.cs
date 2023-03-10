using Leopotam.EcsLite;
using UniRx;
using UnityEngine;

namespace Ai.UnitAi
{
    public abstract class SensorBase : IReset
    {
        protected readonly CompositeDisposable sup = new CompositeDisposable();

        public bool IsActive { get; protected set; }

        public virtual void Update()
        {
        }

        public abstract void BindEntity(EcsWorld world, int i);
        protected abstract void OnReset();
        public abstract void BindAnimator(Animator stateMachineAnimator);
        public void Reset()
        {
            sup.Clear();
            OnReset();
        }
    }

    public interface IReset
    {
        void Reset();
    }
}