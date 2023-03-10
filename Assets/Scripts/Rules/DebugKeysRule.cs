using Locator;
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



        public void Tick(float dt)
        {
            if(Input.GetKeyDown(KeyCode.F5))
                _messenger.Fire(new MainSignals.SaveGameRequest());
            if(Input.GetKeyDown(KeyCode.F9))
                _messenger.Fire(new MainSignals.LoadGameRequest());
        }
    }
    public class HotKeysRule : ITick
    {
        private GameMessenger _messenger;

        public HotKeysRule()
        {
            _messenger = Container.Get<GameMessenger>();
        }



        public void Tick(float dt)
        {
            if(Input.GetKeyDown(KeyCode.S))
                _messenger.Fire(new MainSignals.SaveGameRequest());
            if(Input.GetKeyDown(KeyCode.F9))
                _messenger.Fire(new MainSignals.LoadGameRequest());
        }
    }
}