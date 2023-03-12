using Data;
using Locator;
using Services;
using Signals;
using UniRx;
using UnityEngine;
using Views;

namespace Rules
{
    public class CameraMoveRule : ITick
    {
        private readonly ICameraService _camService;
        private readonly float _moveSpeed;
        private readonly IPointerService _pointerService;
        private readonly ILevelService _levelService;
        private readonly GameMessenger _messenger;
        private readonly float _rotateSpeed;
        private readonly float _dragScrollSpeed;

        private Vector3 _savedPosition;
        private Vector3 _moveSpeedVector;
        private float _timerHold;
        private float _tolerance = 0.1f;
        private float _dragSumTolerance = 5f;

        public CameraMoveRule()
        {
            _camService = Container.Get<ICameraService>();
            _messenger = Container.Get<GameMessenger>();
            _pointerService = Container.Get<IPointerService>();
            _levelService = Container.Get<ILevelService>();


            var config = Container.Get<DataContainer<GameConfigData>>().Data;
            _moveSpeed = config.GeneralConfig.BorderScrollSpeed;
            _rotateSpeed = config.GeneralConfig.CameraRotateSpeed;
            _dragScrollSpeed = config.GeneralConfig.DragScrollSpeed;

            _pointerService.PointerPos.Subscribe(OnPointer);
            _messenger.Subscribe<MainSignals.MiniMapLeftClick>(click =>
            {
                if(_camService.CameraView)
                    _camService.CameraView.SetPositionByMid(_levelService.GetPlaneCoordinates(click.Position));
            });

        }

        public void Tick(float dt)
        {

            if (_pointerService.MouseState.Value == Services.PointerMouseState.RightButtonHold)
            {
                if (_pointerService.Delta.Value.magnitude > _dragSumTolerance
                    && _pointerService.FunctionalState.Value != Services.FunctionalState.DragPanning)
                {
                    _pointerService.SetFunctionalState(FunctionalState.DragPanning);
                }
            }
            if (_camService.CameraView && _pointerService.MouseState.Value == PointerMouseState.Free)
            {
                _timerHold = 0f;
                //MoveCamera( _moveSpeedVector * dt);
            }
            else if (_camService.CameraView && _pointerService.MouseState.Value == PointerMouseState.MidButtonHold)
            {
                _timerHold = 0f;
                _camService.CameraView.transform.RotateAround(_camService.CameraView.Mid, Vector3.up,
                    _pointerService.HoldDelta.Value.x * _rotateSpeed);
            }
            else if (_camService.CameraView && _pointerService.FunctionalState.Value == Services.FunctionalState.DragPanning)
            {
                _timerHold += dt;
                if(_timerHold> _tolerance)
                    MoveCamera(new Vector3(_pointerService.HoldDelta.Value.x, 0, _pointerService.HoldDelta.Value.y) * _dragScrollSpeed);
            }

            if (_camService.CameraView)
            {
                if(Mathf.Abs(Input.mouseScrollDelta.y) < 0.01f)
                    return;

                _camService.Zoom(Input.mouseScrollDelta.y);
            }

        }

        private void MoveCamera( Vector3 speed)
        {
            if (CameraInBorders(_camService.CameraView))
            {
                var transform = _camService.CameraView.transform;
                var position = transform.position;
                _savedPosition = position;
                position += (transform.rotation * speed);
                transform.position = position;
            }
            else
            {
                _camService.CameraView.transform.position = _savedPosition;
            }
        }

        private bool CameraInBorders(CameraView view)
        {
            foreach (var corner in view.WorldPlaneCorners)
            {
                if (corner.x < _levelService.MapXMin)
                    return false;
                if (corner.x > _levelService.MapXMax)
                    return false;
                if (corner.z < _levelService.MapYMin)
                    return false;
                if (corner.z > _levelService.MapYMax)
                    return false;
            }

            return true;
        }

        private void OnPointer(Vector2 vector2)
        {
            var y = 0f;
            var x = 0f;

            if (vector2.x <= 0)
                x = -_moveSpeed;
            else if (vector2.x >= Screen.width)
                x = _moveSpeed;

            if (vector2.y <= 0)
                y = -_moveSpeed;
            else if (vector2.y >= Screen.height)
                y = _moveSpeed;

            _moveSpeedVector.x = x;
            _moveSpeedVector.z = y;
        }
    }
}