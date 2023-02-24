using Locator;
using Services;
using Signals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    [RequireComponent(typeof(RectTransform))]
    public class MiniMapView : MonoBehaviour, IPointerClickHandler
    {
        public Vector2 TerrainSize;

        private Vector2 _lastPointerPosition;
        private Vector2 _uiSize;
        private RectTransform _rTransform;
        private Vector2 _topLeft;
        private Vector2 _topRight;
        private Vector2 _bottomRight;
        private Vector2 _bottomLeft;
        private float _width;
        private float _height;
        private GameMessenger _messenger;


        private void Start()
        {
            _rTransform = GetComponent<RectTransform>();
            _uiSize = GetComponent<RectTransform>().sizeDelta;
            _lastPointerPosition = Input.mousePosition;
            var corners = new Vector3[4];
            _rTransform.GetWorldCorners(corners);
            _bottomLeft = corners[0];
            _bottomRight = corners[3];
            _topRight = corners[2];
            _topLeft = corners[1];
            _width = _bottomRight.x - _bottomLeft.x;
            _height = _topRight.y - _bottomRight.y;

            _messenger = Container.Get<GameMessenger>();

        }

        private Vector2 GetNormPosition(Vector2 mouse)
        {
            mouse -= _bottomLeft;
            return new Vector2(mouse.x / _width, mouse.y / _height);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _messenger.Fire(new MainSignals.MiniMapLeftClick(GetNormPosition(eventData.position)));
            }
        }
    }
}