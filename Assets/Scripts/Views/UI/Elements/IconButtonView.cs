using System;
using Data;
using Locator;
using Services;
using Services.PrefabPool;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Views.UI.Elements
{
    public class IconButtonView : MonoPoolableObject, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _timer;
        private GameMessenger _messenger;
        private VisualData _visualData;
        private CompositeDisposable _sup = new CompositeDisposable();
        private IDisposable _timerSup;

        public override void Awake()
        {
            base.Awake();
            _timer.enabled = false;
        }

        public void Bind<T>(string iconId, T value = default) where T : ISignal
        {
            _messenger ??= Container.Get<GameMessenger>();
            _visualData ??= Container.Get<VisualData>();

            var sprite = _visualData.GetSprite(iconId);
            if (sprite != null)
                _icon.sprite = sprite;
            if(value == null)
                _button.onClick.AddListener(() => _messenger.Fire<T>());
            else
                _button.onClick.AddListener(() => _messenger.Fire(value));
        }
        public void BindTimer(ReactiveProperty<float> time, ReactiveProperty<float> max)
        {
            SetStretched();
            _timer.fillAmount = 0;
            _timer.enabled = true;
            time.Subscribe(f => _timer.fillAmount = f/max.Value ).AddTo(_sup);
        }

        private void SetStretched()
        {
            var rectTransform = (RectTransform) transform;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        public override void Dispose()
        {
            base.Dispose(this);
        }

        public override void OnDespawned()
        {
            base.OnDespawned();
            _button.onClick.RemoveAllListeners();
            _sup?.Clear();
            _timer.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}