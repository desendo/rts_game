using DG.Tweening;
using Locator;
using Services;
using UniRx;
using UnityEngine;

namespace Views
{
    public class CameraView : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _rotateYHandler;
        [SerializeField] private LineRenderer _lineRenderer;
        private ICameraService _service;
        private Plane _plane = new Plane(Vector3.up, Vector3.zero);
        public Camera Cam => _camera;

        private readonly Vector3[] _worldPlaneCorners = new Vector3[5];
        private Vector3 _mid = new Vector3();
        private Vector3 _deltaToMid;

        public Transform RotateYHandler => _rotateYHandler;

        public Vector3[] WorldPlaneCorners => _worldPlaneCorners;
        public Vector3 Mid => _mid;
        private void Awake()
        {
            Container.BindComplete.Where(x => x).Subscribe(b =>
            {
                _service = Container.Get<ICameraService>();
                _service.SetView(this);
            });
            _lineRenderer.widthMultiplier = 5f;
        }

        public void SetPositionByMid(Vector3 targetMid)
        {
            var y = transform.position.y;
            var targetPos  = _deltaToMid + targetMid;
            targetPos.y = y;

            transform.DOMove(targetPos, 0.05f);
        }
        public bool ScreenToWorldPlane(Vector2 eventDataPosition, out Vector3 pos)
        {
            var r1 = Cam.ScreenPointToRay(eventDataPosition);

            if (_plane.Raycast(r1, out var d1))
            {
                pos = r1.GetPoint(d1);
                return true;
            }
            pos = Vector3.zero;
            return false;
        }
        private void Update()
        {
            InitPlaneProjectedCoordinates();
            _lineRenderer.positionCount = 5;
            _lineRenderer.SetPositions(_worldPlaneCorners);
        }

        private void InitPlaneProjectedCoordinates()
        {
            var r1 = Cam.ViewportPointToRay(new Vector3(0, 0));
            var r2 = Cam.ViewportPointToRay(new Vector3(0, 1));
            var r3 = Cam.ViewportPointToRay(new Vector3(1, 1));
            var r4 = Cam.ViewportPointToRay(new Vector3(1, 0));
            var m = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            var h1 = Vector3.zero;
            var h2 = Vector3.zero;
            var h3 = Vector3.zero;
            var h4 = Vector3.zero;

            if(_plane.Raycast(r1, out var d1))
                h1 = r1.GetPoint(d1);
            if(_plane.Raycast(r2, out var d2))
                h2 = r2.GetPoint(d2);
            if(_plane.Raycast(r3, out var d3))
                h3 = r3.GetPoint(d3);
            if(_plane.Raycast(r4, out var d4))
                h4 = r4.GetPoint(d4);

            if (_plane.Raycast(m, out var m1))
            {
                _mid = m.GetPoint(m1);

                var zeroPos = transform.position;
                zeroPos.y = 0;
                _deltaToMid = zeroPos - _mid;

            }
            h1.y = 0.1f;
            h2.y = 0.1f;
            h3.y = 0.1f;
            h4.y = 0.1f;

            _worldPlaneCorners[0] = h1;
            _worldPlaneCorners[1] = h2;
            _worldPlaneCorners[2] = h3;
            _worldPlaneCorners[3] = h4;
            _worldPlaneCorners[4] = h1;

        }


    }
}
