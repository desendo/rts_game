using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Leopotam.EcsLite;
using Locator;
using Models.Components;
using Services;
using Services.PrefabPool;
using Signals;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Views.UI.Elements;

namespace Views.UI.Displays
{
    public class BottomPanelCanvas : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private RawImage _iconRendered;
        [SerializeField] private Camera _avatarCamera;
        [SerializeField] private Transform _actionIconsParent;
        [SerializeField] private Transform _queueParent;
        [SerializeField] private Transform _currentParent;
        [SerializeField] private Transform _currentInfo;
        [SerializeField] private TMP_Text _currentInfoText;
        private IUnitsService _unitsService;
        private VisualData _visualData;
        private readonly List<IconButtonView> _iconsList = new List<IconButtonView>();
        private readonly List<IconButtonView> _iconsQueueList = new List<IconButtonView>();
        private IconButtonView _current;
        private readonly CompositeDisposable _sup = new CompositeDisposable();
        private IDisposable _queueUpdateSup;
        private IDisposable _removeProcessSup;
        private readonly ReactiveProperty<int> _unitSelected = new ReactiveProperty<int>(-1);
        private readonly ReactiveProperty<bool> _unitSelectedHasProgress = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<float> _progressMax = new ReactiveProperty<float>(0);
        private readonly ReactiveProperty<float> _progressCurrent = new ReactiveProperty<float>(0);

        private EcsWorld _world;
        private EcsFilter _filter;
        private EcsFilter _filter2;
        private EcsFilter _filter3;

        private void Awake()
        {
            Container.BindComplete.Where(x => x).Subscribe(b =>
            {
                _unitsService = Container.Get<IUnitsService>();
                _visualData = Container.Get<VisualData>();
                _world = Container.Get<EcsWorld>();
                _unitsService.SelectedUnit.Subscribe(OnSelected);
                _filter = _world.Filter<ComponentProductionQueue>().Inc<ComponentProductionQueueUpdated>().End();
                _filter2 = _world.Filter<ComponentProductionRun>().Inc<ComponentProductionProgressStarted>().End();
                _filter3 = _world.Filter<ComponentProductionRun>().End();
                _current = PrefabPool.InstanceGlobal.Spawn(_visualData.IconButtonView, _currentParent);
                _current.BindTimer(_progressCurrent, _progressMax);
                _current.gameObject.SetActive(false);
            });
        }

        private void OnSelected(int obj)
        {
            ClearIcons();
            _sup?.Clear();
            _unitSelected.Subscribe(i =>
            {
                if(_current != null)
                    _current.gameObject.SetActive(_unitSelectedHasProgress.Value && i >= 0 );

            }).AddTo(_sup);
            _unitSelectedHasProgress.Subscribe(b =>
            {
                if(_current != null)
                    _current.gameObject.SetActive(b && _unitSelected.Value >= 0);
            }).AddTo(_sup);

            _unitSelected.Value = obj;
            _icon.enabled = obj >= 0;

            _avatarCamera.gameObject.SetActive(_unitSelected.Value >= 0);
            _iconRendered.gameObject.SetActive(_unitSelected.Value >= 0);

            if (_unitSelected.Value >= 0)
            {
                _unitsService.Units[_unitSelected.Value].ParentCamera(_avatarCamera);
            }

            UpdateSelectedUnit();
            UpdateProductionSchema();
            UpdateProductionQueue();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (_unitSelected.Value < 0)
            {
                _currentInfo.gameObject.SetActive(false);
                return;
            }

            if (_unitSelected.Value.Has<ComponentInfo>(_world)
                && !_unitSelected.Value.Has<ComponentProductionSchema>(_world))
            {
                _currentInfo.gameObject.SetActive(true);
                var c1 = _unitSelected.Value.Get<ComponentInfo>(_world);
                _currentInfoText.text = $"<size=130%>{c1.Title}</size>\n{c1.Description}";
            }
            else
            {
                _currentInfo.gameObject.SetActive(false);
            }
        }

        private void UpdateSelectedUnit()
        {
            if (_unitSelected.Value < 0)
            {
                _icon.enabled = false;
                return;
            }
            var selectedUnit = _world.GetPool<ComponentUnit>().Get(_unitSelected.Value);
            var sprite = _visualData.Sprites.FirstOrDefault(x => x.Id == selectedUnit.ConfigId)?.Obj;
            if (sprite != null)
            {
                _icon.enabled = true;
                _icon.sprite = sprite;
            }
            else
            {
                _icon.enabled = false;
            }
        }

        private void UpdateProductionQueue()
        {
            if (_unitSelected.Value < 0)
            {
                _queueParent.gameObject.SetActive(false);
                return;
            }
            _queueParent.gameObject.SetActive(true);

            var productionQueuePool = _world.GetPool<ComponentProductionQueue>();
            if (productionQueuePool.Has(_unitSelected.Value))
            {
                ref var queue = ref _world.GetPool<ComponentProductionQueue>().Get(_unitSelected.Value);
                ShowQueue(queue.Queue, _unitSelected.Value);
            }
        }

        private void UpdateProductionSchema()
        {
            if (_unitSelected.Value < 0)
            {
                return;
            }
            var productionSchemaPool = _world.GetPool<ComponentProductionSchema>();
            if (productionSchemaPool.Has(_unitSelected.Value))
            {
                var production = _world.GetPool<ComponentProductionSchema>().Get(_unitSelected.Value);

                foreach (var variant in production.Variants)
                {
                    var icon = PrefabPool.InstanceGlobal.Spawn(_visualData.IconButtonView, _actionIconsParent);
                    _iconsList.Add(icon);
                    if(!_unitSelected.Value.Has<ComponentBuilder>(_world))
                        icon.Bind(variant.ResultId, new MainSignals.ProductionEnqueueRequest(variant.ResultId, _unitSelected.Value));
                    else
                        icon.Bind(variant.ResultId, new MainSignals.ProductionChooseBuildRequest(variant.ResultId, _unitSelected.Value));

                }
            }
        }


        private void Update()
        {
            if(_unitSelected.Value < 0)
                return;

            UpdateInfo();

            foreach (var i in _filter)
            {
                if (i == _unitSelected.Value)
                {
                    var c1 = _world.GetPool<ComponentProductionQueue>();
                    var queue = c1.Get(i).Queue;
                    ShowQueue(queue, i);
                    _world.GetPool<ComponentProductionQueueUpdated>().Del(i);
                }
            }
            var unitSelectedHasProgress = false;
            foreach (var i in _filter3)
            {
                if (i == _unitSelected.Value)
                {
                    unitSelectedHasProgress = true;
                    var c1 = _world.GetPool<ComponentProductionRun>().Get(i);
                    _progressCurrent.Value = c1.Current;
                    _progressMax.Value = c1.Max;
                }
            }
            _unitSelectedHasProgress.Value = unitSelectedHasProgress;

            foreach (var i in _filter2)
            {
                if (i == _unitSelected.Value)
                {
                    var c1 = _world.GetPool<ComponentProductionRun>().Get(i);
                    _current.Bind(c1.Result, new MainSignals.ProductionDequeueRequest(c1.Result,_unitSelected.Value, 0));
                    _world.GetPool<ComponentProductionProgressStarted>().Del(i);
                }
            }
        }

        private void ShowQueue(IReadOnlyList<string> queue, int i)
        {
            _iconsQueueList.ForEach(x=>x.Dispose());
            _iconsQueueList.Clear();

            for (var index = 0; index < queue.Count; index++)
            {
                var id = queue[index];
                var iconView = PrefabPool.InstanceGlobal.Spawn(_visualData.IconButtonView, _queueParent);
                _iconsQueueList.Add(iconView);
                iconView.Bind(id, new MainSignals.ProductionDequeueRequest(id, i, index));
            }
        }

        private void ClearIcons()
        {
            _iconsList.ForEach(x=>x.Dispose());
            _iconsQueueList.ForEach(x=>x.Dispose());
            _iconsList.Clear();
            _iconsQueueList.Clear();
        }

    }
}
