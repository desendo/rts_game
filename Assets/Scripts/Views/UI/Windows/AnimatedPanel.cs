using DG.Tweening;
using UnityEngine;

namespace Views
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AnimatedPanel : MonoBehaviour
    {
        private const float Duration = 0.2f;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private Sequence _animation;

        public Tween ShowPanel()
        {
            _animation?.Kill();
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = true;

            _animation = DOTween.Sequence();

            _animation.Append(_canvasGroup
                    .DOFade(1, Duration))
                .AppendCallback(() =>
                {
                    _canvasGroup.alpha = 1;
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                });
            return _animation;
        }

        public Tween HidePanel()
        {
            _animation?.Kill();
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = true;
            _animation = DOTween.Sequence();

            _animation.Append(_canvasGroup
                    .DOFade(0, Duration))
                .AppendCallback(() =>
                {
                    _canvasGroup.alpha = 0;
                    _canvasGroup.interactable = false;
                    _canvasGroup.blocksRaycasts = false;
                });

            return _animation;
        }
    }
}