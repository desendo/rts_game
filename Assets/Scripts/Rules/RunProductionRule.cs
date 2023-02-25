using System.Linq;
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
        private IUnitsService _unitsService;

        public RunProductionRule()
        {
            _config = Container.Get<GameConfigData>();
            _messenger = Container.Get<GameMessenger>();
            _unitsService = Container.Get<IUnitsService>();
        }

        public void Tick(float dt)
        {
            for (var index = 0; index < Storage<AspectProductionProcess>.Instance.Aspects.Length; index++)
            {
                var aspect = Storage<AspectProductionProcess>.Instance.Aspects[index];
                if (aspect == null)
                    continue;

                if (aspect.Ready.Value && index.Has<AspectUnitExitPath>())
                {
                    var result = index.Get<AspectProductionProcess>().ResultId;
                    index.Remove<AspectProductionProcess>();
                    ProcessResult(result, index);
                    continue;
                }

                aspect.CurrentTime.Value += dt;
                if (aspect.CurrentTime.Value > aspect.MaxTime.Value)
                {
                    aspect.Ready.Value = true;
                }
            }
        }

        private void ProcessResult(string result, in int index)
        {
            var resultConfig = _config.ResultConfigs.FirstOrDefault(x => x.Id == result);
            var exitPath = index.Get<AspectUnitExitPath>();
            if (resultConfig != null && resultConfig.ResultType == ResultType.Unit && exitPath != null)
            {
                _unitsService.CreateUnit(exitPath.Path[0], result, exitPath.Path);
            }
        }
    }
}