using DG.Tweening;
using Leopotam.EcsLite;
using Locator;
using Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Views.UI.Displays
{
    public class MousePointerCanvas : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private RectTransform _iconHolder;
        [SerializeField] private RectTransform _dragPanIconHolder;
        [SerializeField] private RectTransform _hoverIconHolder;
        [SerializeField] private RectTransform _hoverIcon;
        [SerializeField] private RectTransform _selectIconHolder;
        [SerializeField] private RectTransform _selectIcon;
        private Vector2 _hoverIconDefaultSize;
        private Tween _hoverTween;
        private IReadOnlyPointerService _pointerService;

        private bool _previousHoverIconHolderGameObjectActiveState;

        private EcsWorld _world;

        private void Awake()
        {
            _hoverIconDefaultSize = _hoverIcon.sizeDelta;
            Container.BindComplete.Where(x => x).Subscribe(b =>
            {
                _world = Container.Get<EcsWorld>();
                _pointerService = Container.Get<IReadOnlyPointerService>();
                _pointerService.FunctionalState.Subscribe(state =>
                {
                    _dragPanIconHolder.gameObject.SetActive(state == FunctionalState.DragPanning);
                    if (_dragPanIconHolder.gameObject.activeSelf)
                    {
                        var angle = Mathf.Atan2(_pointerService.HoldDelta.Value.y, _pointerService.HoldDelta.Value.x) *
                                    Mathf.Rad2Deg;
                        _dragPanIconHolder.rotation = Quaternion.Euler(0, 0, angle);
                    }
                });
                _pointerService.HoldDelta.Subscribe(delta =>
                {
                    if (_dragPanIconHolder.gameObject.activeSelf)
                    {
                        var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                        _dragPanIconHolder.rotation = Quaternion.Euler(0, 0, angle);
                    }
                });
                _pointerService.PointerPos.Subscribe(pos =>
                {
                    _dragPanIconHolder.position = pos;
                });
            });
        }

        private void Update()
        {
            if (!Container.BindComplete.Value)
                return;

            var isFree1 = _pointerService.FunctionalState.Value == FunctionalState.Free;
            var isFree2 = _pointerService.UnitState.Value == UnitState.Free;
            var isHovered = _pointerService.IsHovered.Value;

            _selectIconHolder.gameObject.SetActive(_pointerService.FunctionalState.Value == FunctionalState.FrameSelecting);
            _selectIconHolder.position = _pointerService.PrevPos.Value;
            var delta = -_pointerService.PrevPos.Value + _pointerService.PointerPos.Value;
            var scaleX = 1;
            var scaleY = -1;
            if (delta.x < 0)
            {
                scaleX = -1;
                delta.x = -delta.x;
            }

            if (delta.y < 0)
            {
                scaleY = 1;
                delta.y = -delta.y;
            }
            _selectIcon.sizeDelta = new Vector2(delta.x, delta.y);
            _selectIcon.localScale = new Vector3(scaleX, scaleY, 1);

            Cursor.visible = isFree1 && isFree2 && !isHovered;
            _hoverIconHolder.gameObject.SetActive(isFree1 && isFree2 && isHovered);
            if (_hoverIconHolder.gameObject.activeSelf)
                _hoverIconHolder.position = Input.mousePosition;
            if (_previousHoverIconHolderGameObjectActiveState != _hoverIconHolder.gameObject.activeSelf)
            {
                if (_hoverIconHolder.gameObject.activeSelf)
                {
                    _hoverTween = _hoverIconHolder.DOScale(1.2f, 0.5f)
                        .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).Play();
                }
                else
                {
                    _hoverTween?.Kill();
                    _hoverIconHolder.localScale = Vector3.one;
                }
            }

            _previousHoverIconHolderGameObjectActiveState = _hoverIconHolder.gameObject.activeSelf;
        }
    }
}