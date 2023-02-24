using Data;
using Locator;
using Models.Aspects;
using Services;
using UniRx;
using UnityEngine;

namespace Rules
{
    public class RunProductionRule : ITick
    {
        private readonly GameMessenger _messenger;
        private readonly CompositeDisposable _sup = new CompositeDisposable();
        private GameConfigData _config;

        public RunProductionRule()
        {
            _config = Container.Get<GameConfigData>();
            _messenger = Container.Get<GameMessenger>();
        }

        public void Tick(float dt)
        {
            for (var index = 0; index < Storage<AspectProductionProcess>.Instance.Aspects.Length; index++)
            {
                var aspect = Storage<AspectProductionProcess>.Instance.Aspects[index];
                if (aspect == null)
                    continue;

                if (aspect.Ready.Value)
                {
                    index.Remove<AspectProductionProcess>();
                    continue;
                }

                aspect.CurrentTime.Value += dt;
                if (aspect.CurrentTime.Value > aspect.MaxTime.Value)
                {
                    aspect.Ready.Value = true;
                }
            }
        }
    }
}