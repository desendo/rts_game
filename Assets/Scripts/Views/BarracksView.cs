using System;
using System.Collections.Generic;
using Models;
using Models.Components;
using Services;
using UnityEngine;

namespace Views
{
    public class BarracksView : UnitView
    {
        [SerializeField] private Transform _anchor1;
        [SerializeField] private Transform _anchor2;

        public override void Bind(int entity)
        {
            base.Bind(entity);
            ref var path = ref _world.GetPool<ComponentUnitProductionBuilding>().Add(entity);
            path.Path = new List<Vector3> {_anchor1.position, _anchor2.position};
        }
    }
}