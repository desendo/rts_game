using Locator;
using Services;
using UnityEngine;
using UniRx;

namespace Views.LevelEditorViews
{
    public class LevelEditorUnitView : MonoBehaviour
    {
        [SerializeField] private LevelEditorUnitData _levelEditorUnitData;
        private ILevelService _levelService;
        private IGameStateService _gameStateService;

        private void Awake()
        {
            Container.BindComplete.Where(x => x).Subscribe(x =>
            {
                _levelService = Container.Get<ILevelService>();
                _gameStateService = Container.Get<IGameStateService>();
                _levelService.OnGatherLevelConfigStartedRequest.Subscribe(OnGatherRequest).AddTo(this);
            }).AddTo(this);
        }


        private void OnGatherRequest(bool isRequired)
        {
            if (isRequired)
            {
                _levelEditorUnitData.Position = transform.position;
                _levelEditorUnitData.Rotation = transform.rotation.eulerAngles.y;
                _levelEditorUnitData.Direction = transform.forward;
                _levelService.AddUnitLevelEditorData(_levelEditorUnitData);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    [System.Serializable]
    public class LevelEditorUnitData
    {
        public int PlayerIndex;
        public string ConfigId;
        [HideInInspector]
        public Vector3 Position;
        [HideInInspector]
        public float Rotation;
        [HideInInspector]
        public Vector3 Direction;
    }
}
