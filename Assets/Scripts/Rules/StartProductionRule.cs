using Data;
using Locator;
using Models.Aspects;
using Services;
using UniRx;
using UnityEngine;

namespace Rules
{
    public class StartProductionRule
    {
        private readonly GameMessenger _messenger;
        private readonly CompositeDisposable _sup = new CompositeDisposable();
        private GameConfigData _config;

        public StartProductionRule()
        {
            _config = Container.Get<GameConfigData>();
            _messenger = Container.Get<GameMessenger>();
            Filter<AspectUnit, AspectQueue>.Instance.OnAdd.Subscribe(x => ReCalcSubscriptions());
            Filter<AspectUnit, AspectQueue>.Instance.OnRemove.Subscribe(x => ReCalcSubscriptions());
            Filter<AspectProductionProcess, AspectQueue>.Instance.OnRemove.Subscribe(x =>
            {
                ReCalcSubscriptions();
            });
            ReCalcSubscriptions();
        }

        private void ReCalcSubscriptions()
        {
            _sup?.Clear();

            foreach (var i in Filter<AspectUnit, AspectQueue>.Instance.IndexList)
            {
                var index = i;
                HandleQueueChange(index);

                index.Get<AspectQueue>().OnChange.Subscribe(unit =>
                {
                    HandleQueueChange(index);
                }).AddTo(_sup);
            }
        }

        private static void HandleQueueChange(int index)
        {

            var aspectQueue = index.Get<AspectQueue>();

            var has = index.Has<AspectProductionProcess>();
            if (has)
                return;

            if (aspectQueue.List.Count > 0)
            {

                var resultId = aspectQueue.List[0];
                var variants = index.Get<AspectProduction>()?.ProductionVariants;

                if (variants == null)
                    return;

                for (var i1 = 0; i1 < variants.Length; i1++)
                {
                    if (variants[i1].ResultId == resultId)
                    {
                        var variant = variants[i1];

                        var aspectProduction = new AspectProductionProcess()
                        {
                            ResultId = variant.ResultId,
                        };
                        aspectProduction.Ready.Value = false;
                        aspectProduction.CurrentTime.Value = 0f;
                        aspectProduction.MaxTime.Value = variant.Duration;
                        index.Set<AspectProductionProcess>(aspectProduction);
                        aspectQueue.List.RemoveAt(0);
                        aspectQueue.OnChange.OnNext(Unit.Default);
                        break;
                    }
                }
            }
        }


    }
}