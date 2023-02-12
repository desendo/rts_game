using Locator;
using Services;
using Signals;
using UnityEngine;

namespace Rules
{
    public class ClickRule
    {
        private readonly GameMessenger _messenger;
        private readonly IPointerService _pointerService;

        public ClickRule()
        {
            _pointerService = Container.Get<IPointerService>();
            _messenger = Container.Get<GameMessenger>();
            _messenger.Subscribe<MainSignals.SelectModelView>(OnSelectRequest);
            _messenger.Subscribe<MainSignals.ContextActionModelView>(OnActionRequest);
        }

        private void OnActionRequest(MainSignals.ContextActionModelView obj)
        {
            Debug.Log("action");
        }

        private void OnSelectRequest(MainSignals.SelectModelView obj)
        {
            Debug.Log("select");
        }
    }
}