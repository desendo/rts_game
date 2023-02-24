using System;
using Locator;
using Services;
using UniRx;
using UnityEngine;

namespace Views
{
    public class WorldView : MonoBehaviour
    {
        [SerializeField] private Transform[] _worldCorners;
        [SerializeField] private Camera _miniMapCamera;
        public Transform[] WorldCorners => _worldCorners;

        private void Awake()
        {
            Container.BindComplete.Where(x => x).Subscribe(b =>
            {
                Container.Get<ILevelService>().SetMapCorners(_worldCorners);
                _miniMapCamera.orthographicSize = Mathf.Abs(_worldCorners[1].position.x - _worldCorners[0].position.x) * 0.5f;

            });
        }
    }
}
