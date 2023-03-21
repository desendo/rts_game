using Leopotam.EcsLite;
using UniRx;
using UnityEngine;

namespace Ai.UnitAi
{
    public abstract class SensorBase : IReset
    {
        protected readonly CompositeDisposable sup = new CompositeDisposable();

        public bool IsActive { get; protected set; }
        protected Animator animator;
        protected EcsWorld world;
        protected int i;
        protected bool isSet;
        public virtual void Bind(EcsWorld w, int entity)
        {
            this.i = entity;
            this.world = w;
            isSet = true;
        }
        public virtual void SetAnimator(Animator stateMachineAnimator)
        {
            animator = stateMachineAnimator;
        }
        public virtual void Update()
        {
        }

        protected abstract void OnReset();
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