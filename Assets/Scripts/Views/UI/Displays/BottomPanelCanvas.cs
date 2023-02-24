using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Locator;
using Models.Aspects;
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

        private void Awake()
        {
            Container.BindComplete.Where(x => x).Subscribe(b =>
            {
                _unitsService = Container.Get<IUnitsService>();
                _visualData = Container.Get<VisualData>();
                _unitsService.CurrentUnitSelected.Subscribe(UnitSelected);
            });
        }

        private void UnitSelected(AspectUnit obj)
        {
            if (obj == null)
            {
                _icon.enabled = false;
                ClearIcons();
                _sup?.Clear();
                return;
            }

            _icon.enabled = true;


            var sprite = _visualData.Sprites.FirstOrDefault(x => x.Id == obj.ConfigId)?.Obj;
            if (sprite != null)
                _icon.sprite = sprite;

            var production = obj.UnitIndex.Get<AspectProduction>();
            if (production != null)
            {
                foreach (var variant in production?.ProductionVariants)
                {
                    var icon = PrefabPool.InstanceGlobal.Spawn(_visualData.IconButtonView, _actionIconsParent);
                    _iconsList.Add(icon);
                    icon.Bind(variant.ResultId, new MainSignals.ProductionEnqueueRequest(variant.ResultId, obj.UnitIndex));
                }
            }

            ClearCurrentProgress();
            HandleCurrentProgressChanged(obj.UnitIndex);
            Filter<AspectProductionProcess, AspectQueue>.Instance.OnChange.Subscribe(filter =>
            {
                ClearCurrentProgress();
                if (filter.IndexHash.Contains(obj.UnitIndex))
                    HandleCurrentProgressChanged(obj.UnitIndex);
            }).AddTo(_sup);

            Filter<AspectUnit, AspectQueue>.Instance.OnChange.Subscribe(filter =>
            {
                if (filter.IndexHash.Contains(obj.UnitIndex))
                    UpdateQueueView(obj.UnitIndex);

            }).AddTo(_sup);
            if(Filter<AspectUnit, AspectQueue>.Instance.IndexHash.Contains(obj.UnitIndex))
                UpdateQueueView(obj.UnitIndex);

        }


        private void HandleCurrentProgressChanged(int obj)
        {
            var productionProcess = obj.Get<AspectProductionProcess>();
            if (productionProcess != null)
            {
                _current = PrefabPool.InstanceGlobal.Spawn(_visualData.IconButtonView, _currentParent);
                _current.Bind(productionProcess.ResultId, productionProcess.CurrentTime, productionProcess.MaxTime,
                    new MainSignals.ProductionCancelRequest(productionProcess.ResultId, obj));
            }
        }

        private void UpdateQueueView(int i)
        {
            var queueAspect = i.Get<AspectQueue>();
            ApplyQueue(queueAspect.List, i);
            _queueUpdateSup?.Dispose();
            _queueUpdateSup = queueAspect.OnChange.Subscribe(unit => ApplyQueue(queueAspect.List, i));
        }


        private void ApplyQueue(List<string> queue, int objId)
        {
            _iconsQueueList.ForEach(x=>x.Dispose());
            _iconsQueueList.Clear();
            for (var index = 0; index < queue.Count; index++)
            {
                var id = queue[index];
                var iconView = PrefabPool.InstanceGlobal.Spawn(_visualData.IconButtonView, _queueParent);
                _iconsQueueList.Add(iconView);
                iconView.Bind(id, new MainSignals.ProductionDequeueRequest(id, objId, index));
            }
        }

        private void ClearIcons()
        {
            ClearCurrentProgress();
            _iconsList.ForEach(x=>x.Dispose());
            _iconsQueueList.ForEach(x=>x.Dispose());
            _iconsList.Clear();
            _iconsQueueList.Clear();
        }

        private void ClearCurrentProgress()
        {
            if (_current != null)
            {
                _current.Dispose();
                _current = null;
            }
        }
    }
}
