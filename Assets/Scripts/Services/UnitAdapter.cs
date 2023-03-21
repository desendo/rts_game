using System.Collections.Generic;
using Data;
using Leopotam.EcsLite;
using Locator;
using Models.Components;

namespace Services
{
    public abstract class UnitAdapter<T1, T2, T3> where T1 : struct where T2 : ISaveDataEntity where T3 : new()
    {
        public static T3 Instance => _instance ??= new T3();
        private static T3 _instance;
        protected readonly EcsFilter _filter;
        protected readonly EcsFilter _unitFilter;
        protected readonly EcsWorld _world;
        protected abstract T2 Save(T1 c, int i);
        protected abstract void Load(ref T1 c1, T2 save);
        protected UnitAdapter()
        {
            _world = Container.Get<EcsWorld>();
            _filter = _world.Filter<T1>().End();
            _unitFilter = _world.Filter<ComponentUnit>().End();
        }

        public void Save(List<T2> saves)
        {
            saves.Clear();
            foreach (var i in _filter)
                saves.Add(Save(_world.GetPool<T1>().Get(i), i));
        }

        public void Load(List<T2> saves)
        {
            foreach (var save in saves)
            {
                foreach (var e in _unitFilter)
                {
                    if (!_world.GetPool<ComponentUnit>().Has(e))
                        continue;

                    if (save.Id == _world.GetPool<ComponentUnit>().Get(e).EntityIndex)
                    {
                        ref var c1 = ref _world.GetPool<T1>().Add(e);

                        Load(ref c1, save);
                    }

                }
            }
        }


    }
}