using Locator;
using Models.Aspects;
using Services;
using Signals;
using UniRx;
using UnityEngine;

namespace Rules
{
    public class ProductionQueueRule
    {
        private readonly GameMessenger _messenger;

        public ProductionQueueRule()
        {
            _messenger = Container.Get<GameMessenger>();
            _messenger.Subscribe<MainSignals.ProductionEnqueueRequest>(ProductionEnqueueRequest);
            _messenger.Subscribe<MainSignals.ProductionDequeueRequest>(ProductionDequeueRequest);
        }

        private void ProductionDequeueRequest(MainSignals.ProductionDequeueRequest obj)
        {
            var target = obj.ResultId;
            var index = obj.UnitIndex;

            var aspectQueue = index.Get<AspectQueue>();
            if(aspectQueue == null || aspectQueue.List.Count == 0)
                return;

            aspectQueue.List.RemoveAt(obj.QueueIndex);
            aspectQueue.OnChange.OnNext(Unit.Default);
        }

        private void ProductionEnqueueRequest(MainSignals.ProductionEnqueueRequest obj)
        {
            var target = obj.ResultId;
            var index = obj.UnitIndex;

            var aspect = index.Get<AspectQueue>()
                                ?? index.Set<AspectQueue>(new AspectQueue());

            aspect.List.Add(target);
            aspect.OnChange.OnNext(Unit.Default);
        }
    }
}