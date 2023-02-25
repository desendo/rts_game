using System;
using System.Collections.Generic;
using Models;
using Models.Aspects;
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
            var path = new AspectUnitExitPath();
            path.Path.Add(_anchor1.position);
            path.Path.Add(_anchor2.position);
            Index.Set<AspectUnitExitPath>(path);

        }

        public override void OnDespawned()
        {
            base.OnDespawned();
            Index.Remove();
        }
    }
}