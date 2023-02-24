using Locator;
using Models.Aspects;
using Services;
using Signals;
using UnityEngine;

namespace Rules
{
    public class ClickRule
    {
        private readonly IPointerService _pointerService;
        private readonly IUnitsService _unitsService;

        public ClickRule()
        {
            _pointerService = Container.Get<IPointerService>();
            _unitsService = Container.Get<IUnitsService>();
            var messenger = Container.Get<GameMessenger>();
            messenger.Subscribe<MainSignals.EarthLeftClick>(x => ClearSelection());
            messenger.Subscribe<MainSignals.EarthRightClick>(x => OnEarthRightClick(x.Click));
            messenger.Subscribe<MainSignals.SelectRequest>(OnSelectRequest);
            messenger.Subscribe<MainSignals.ContextActionRequest>(OnActionRequest);
            messenger.Subscribe<MainSignals.HoverModelView>(OnHoverRequest);
        }

        private void OnEarthRightClick(Vector3 objClick)
        {
            var currentUnit = _unitsService.CurrentUnitSelected.Value;
            if(currentUnit == null)
                return;

            var index = currentUnit.UnitIndex;
            index.Set<AspectMoveTarget>(new AspectMoveTarget(objClick));

        }

        private void ClearSelection()
        {
            foreach (var aspectSelection in Storage<AspectSelection>.Instance.Aspects)
                if (aspectSelection != null)
                    if (aspectSelection.Selected.Value)
                        aspectSelection.Selected.Value = false;

            _unitsService.SetSelected(null);
        }

        private void OnHoverRequest(MainSignals.HoverModelView obj)
        {
            obj.Model.AspectSelection.Hovered.Value = true;
        }

        private void OnActionRequest(MainSignals.ContextActionRequest obj)
        {
            //Debug.Log("action");
        }

        private void OnSelectRequest(MainSignals.SelectRequest obj)
        {
            if(_unitsService.CurrentUnitSelected.Value == obj.Model.AspectUnit)
                return;

            ClearSelection();
            obj.Model.AspectSelection.Selected.Value = true;
            _unitsService.SetSelected(obj.Model.AspectUnit);

        }
    }
}