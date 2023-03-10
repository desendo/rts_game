using Codice.CM.Common;
using Data;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Locator;
using UnityEngine;
using Views;


namespace Services
{
    public interface ICameraService
    {
        void SetView(CameraView cameraView);

        CameraView CameraView { get;}
        Vector3 GetPlanePointAtCursor(Vector3 mousePosition);
        void Zoom(float f);
    }


    public class CameraService : ICameraService, ITick, IDataHandler<LevelSaveData>
    {
        private CameraView _view;
        private float _speedX;
        private float _speedY;
        private CameraSaveData _data;
        private bool _viewSet;
        private readonly float _zoomSpeed = 2f;
        private readonly float _rotSpeed = 2f;
        private Tween _zoomTween;
        private float _targetZoom;
        private float _initialPos;
        private Quaternion _initialRot;
        private Tween _rotween;
        public CameraView CameraView => _view;
        public Vector3 GetPlanePointAtCursor(Vector3 mousePosition)
        {
            _view.ScreenToWorldPlane(mousePosition, out var pos);
            return pos;
        }

        public void Zoom(float f)
        {
            _targetZoom += f;
            if (_targetZoom > 10)
            {
                _targetZoom = 10;
            }
            else if (_targetZoom < -15)
            {
                _targetZoom = -15;
            }
            var targetRot = new Vector3(_initialRot.eulerAngles.x + _rotSpeed * _targetZoom, 0f, 0f);

            _zoomTween?.Kill();
            _rotween?.Kill();
            _zoomTween = _view.Cam.DOFieldOfView( _initialPos + (_zoomSpeed * _targetZoom), 0.1f);
            _rotween = _view.CameraTransform.DOLocalRotate(  targetRot, 0.1f);

        }

        public void Tick(float dt)
        {
            if (_viewSet)
            {
                if(!CheckCornersInsideMap(_view))
                    return;
                var delta = new Vector3(_speedX, 0, _speedY);
                _view.transform.position += delta * dt;
            }
        }

        private bool CheckCornersInsideMap(CameraView cameraView)
        {
            return true;
        }

        public void SetView(CameraView cameraView)
        {
            if (cameraView == null)
            {
                _viewSet = false;
                return;
            }
            _view = cameraView;
            _initialPos = CameraView.Cam.fieldOfView;
            _initialRot = CameraView.CameraTransform.localRotation;
            _viewSet = true;
            if(_data != null)
                ApplyDataToView(_data);
        }



        public void SaveData(LevelSaveData data)
        {
            data.CameraData ??= new CameraSaveData();

            data.CameraData.Height = 0;
            data.CameraData.Position = _view.transform.position;
            data.CameraData.AngleY = _view.transform.eulerAngles.y;
        }

        public void LoadData(LevelSaveData data)
        {
            _data = data.CameraData;
            if (_view != null && _data != null)
                ApplyDataToView(_data);
        }

        private void ApplyDataToView(CameraSaveData data)
        {
            _view.transform.position = data.Position;
            _view.transform.eulerAngles = new Vector3(0, data.AngleY, 0);
        }
    }
}