using Codice.CM.Common;
using Data;
using Locator;
using UnityEngine;
using Views;


namespace Services
{
    public interface ICameraService
    {
        void SetView(CameraView cameraView);

        CameraView CameraView { get;}
    }


    public class CameraService : ICameraService, ITick, IDataHandler<LevelSaveData>
    {
        private CameraView _view;
        private float _speedX;
        private float _speedY;
        private CameraSaveData _data;
        private bool _viewSet;
        public CameraView CameraView => _view;

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