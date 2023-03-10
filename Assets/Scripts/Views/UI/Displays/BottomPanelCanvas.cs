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
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Views.UI.Elements;

namespace Views.UI.Displays
{
    public class BottomPanelCanvas : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Transform _actionIconsParent;
        [SerializeField] private Transform _queueParent;
        [SerializeField] private Transform _currentParent;
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
            }).AddTo(_sup);;
            if (obj < 0)
            {
                _icon.enabled = false;
                _unitSelected.Value = obj;
                return;
            }
            _unitSelected.Value = obj;
            _icon.enabled = true;

            UpdateSelectedUnit();
            UpdateProductionSchema();
            UpdateProductionQueue();


        }

        private void UpdateSelectedUnit()
        {
            var selectedUnit = _world.GetPool<ComponentUnit>().Get(_unitSelected.Value);
            var sprite = _visualData.Sprites.FirstOrDefault(x => x.Id == selectedUnit.ConfigId)?.Obj;
            if (sprite != null)
                _icon.sprite = sprite;
        }

        private void UpdateProductionQueue()
        {
            var productionQueuePool = _world.GetPool<ComponentProductionQueue>();
            if (productionQueuePool.Has(_unitSelected.Value))
            {
                ref var queue = ref _world.GetPool<ComponentProductionQueue>().Get(_unitSelected.Value);
                ShowQueue(queue.Queue, _unitSelected.Value);
            }
        }

        private void UpdateProductionSchema()
        {
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
                    _current.Bind(c1.Result, new MainSignals.ProductionEnqueueRequest(c1.Result, _unitSelected.Value));
                    _world.GetPool<ComponentProductionProgressStarted>().Del(i);

                }
            }

        }



        private void ShowQueue(List<string> queue, int i)
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
