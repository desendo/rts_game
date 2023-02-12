using Models;
using Services;
using Views;

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

        public class SelectModelView : ISignal
        {
            public IModel Model { get; }
            public SelectModelView(IModel model)
            {
                Model = model;
            }
        }
        public class ContextActionModelView : ISignal
        {
            public IModel Model { get; }
            public ContextActionModelView(IModel model)
            {
                Model = model;
            }
        }
    }
}