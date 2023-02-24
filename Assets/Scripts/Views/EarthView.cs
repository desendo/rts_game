using System;
using Locator;
using Services;
using Signals;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class EarthView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Collider _collider;
        private GameMessenger _messenger;
        private ICameraService _cameraService;

        private void Awake()
        {
            Container.BindComplete.Where(x => x).Subscribe(b =>
            {
                _messenger = Container.Get<GameMessenger>();
                _cameraService = Container.Get<ICameraService>();

            });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_cameraService.CameraView.ScreenToWorldPlane(eventData.position, out var planeCoordinates))
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                    _messenger?.Fire(new MainSignals.EarthLeftClick(planeCoordinates));
                if (eventData.button == PointerEventData.InputButton.Right)
                    _messenger?.Fire(new MainSignals.EarthRightClick(planeCoordinates));
            }
        }
    }
}