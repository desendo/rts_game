using Services;
using UnityEngine;

namespace Signals
{
    public class MainSignals
    {
        public class ResetGameRequest : ISignal
        {
        }

        public class SaveGameRequest : ISignal
        {
        }
        public class LoadGameRequest : ISignal
        {
        }
        public class EarthLeftClick : ISignal
        {
            public readonly Vector3 Click;

            public EarthLeftClick(Vector3 click)
            {
                this.Click = click;
            }
        }
        public class EarthRightClick : ISignal
        {
            public readonly Vector3 Click;

            public EarthRightClick(Vector3 click)
            {
                this.Click = click;
            }
        }
        public class SelectRequest : ISignal
        {
            public UnitCompositionBase Model { get; }
            public SelectRequest(UnitCompositionBase model)
            {
                Model = model;
            }
        }
        public class HoverModelView : ISignal
        {
            public UnitCompositionBase Model { get; }
            public HoverModelView(UnitCompositionBase model)
            {
                Model = model;
            }
        }
        public class ContextActionRequest : ISignal
        {
            public UnitCompositionBase Model { get; }
            public ContextActionRequest(UnitCompositionBase model)
            {
                Model = model;
            }
        }

        public class ViewShown : ISignal
        {
            public string Param { get; set; }
        }

        public class ViewClosed : ISignal
        {
            public string Param { get; set; }
        }

        public readonly struct MiniMapLeftClick : ISignal
        {
            public readonly Vector2 Position;

            public MiniMapLeftClick(Vector2 position)
            {
                Position = position;
            }
        }
        public readonly struct ProductionDequeueRequest : ISignal
        {
            public readonly string ResultId;
            public readonly int UnitIndex;
            public readonly int QueueIndex;

            public ProductionDequeueRequest(string resultId, in int unitIndex, in int queueIndex)
            {
                QueueIndex = queueIndex;
                ResultId = resultId;
                UnitIndex = unitIndex;
            }
        }
        public readonly struct ProductionCancelRequest : ISignal
        {
            public readonly string ResultId;
            public readonly int UnitIndex;

            public ProductionCancelRequest(string resultId, in int unitIndex)
            {
                ResultId = resultId;
                UnitIndex = unitIndex;
            }
        }
        public readonly struct ProductionEnqueueRequest : ISignal
        {
            public readonly string ResultId;
            public readonly int UnitIndex;

            public ProductionEnqueueRequest(string resultId, in int unitIndex)
            {
                ResultId = resultId;
                UnitIndex = unitIndex;
            }
        }
    }
}