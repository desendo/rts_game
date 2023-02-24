using System;
using System.Collections.Generic;
using Models;
using Services;
using UnityEngine;

namespace Views
{
    public class BarracksView : UnitView
    {
        [SerializeField] private Transform _anchor1;
        [SerializeField] private Transform _anchor2;

        public override void Bind(UnitCompositionBase model)
        {
            base.Bind(model);
            if (model is UnitsService.CompositionUnitFactory barracksModel)
            {

            }

        }
    }
}