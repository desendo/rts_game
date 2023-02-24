using Locator;
using Models.Aspects;
using Services;
using Signals;
using UnityEngine;

namespace Rules
{
    public class DebugKeysRule : ITick
    {
        private GameMessenger _messenger;

        public DebugKeysRule()
        {
            _messenger = Container.Get<GameMessenger>();
        }

        private void OnRemove((int, AspectSelection) obj)
        {
        }

        private void OnAdd((int, AspectSelection) obj)
        {
        }

        public void Tick(float dt)
        {
            if(Input.GetKeyDown(KeyCode.S))
                _messenger.Fire(new MainSignals.SaveGameRequest());
            if(Input.GetKeyDown(KeyCode.L))
                _messenger.Fire(new MainSignals.LoadGameRequest());
        }
    }
}