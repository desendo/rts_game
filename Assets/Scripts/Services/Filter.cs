using System.Collections.Generic;
using UniRx;

namespace Services
{
            public class Filter<T1> where T1 : class
        {
            private static Filter<T1> _instance;
            private readonly List<int> _list = new List<int>();
            private readonly Subject<(int, T1)> _onAdd = new Subject<(int, T1)>();
            private readonly Subject<(int, T1)> _onRemove = new Subject<(int, T1)>();

            public Filter()
            {
                Storage<T1>.Instance.OnAdd.Subscribe(HandleOnAdd);
                Storage<T1>.Instance.OnRemove.Subscribe(HandleOnRemove);
                Recalculate();
            }

            public static Filter<T1> Instance => _instance ??= new Filter<T1>();

            public Subject<(int, T1)> OnAdd => _onAdd;
            public Subject<(int, T1)> OnRemove => _onRemove;
            public IReadOnlyList<int> IndexList => _list;

            private void HandleOnRemove(int i)
            {
                _onAdd.OnNext((i, Storage<T1>.Instance.Get(i)));
            }

            private void HandleOnAdd(int i)
            {
                _onAdd.OnNext((i, Storage<T1>.Instance.Get(i)));
            }

            private void Recalculate()
            {
                _list.Clear();
                var l1 = Storage<T1>.Instance.Aspects.Length;

                for (var i = 0; i < l1; i++)
                    if (Storage<T1>.Instance.Aspects[i] != null)
                        _list.Add(i);
            }
        }

        public class Filter<T1, T2> where T1 : class where T2 : class
        {
            private static Filter<T1, T2> _instance;
            private readonly HashSet<int> _hashsetCurrent = new HashSet<int>();
            private readonly HashSet<int> _hashsetPrev = new HashSet<int>();
            private readonly List<int> _list = new List<int>();

            public static Filter<T1, T2> Instance => _instance ??= new Filter<T1, T2>();

            public readonly Subject<(int, T1, T2)> OnAdd = new Subject<(int, T1, T2)>();
            public readonly Subject<(int, T1, T2)> OnRemove = new Subject<(int, T1, T2)>();
            public readonly Subject<Filter<T1, T2>> OnChange = new Subject<Filter<T1, T2>>();
            public IReadOnlyList<int> IndexList => _list;
            public HashSet<int> IndexHash => _hashsetCurrent;
            public Filter()
            {

                Storage<T1>.Instance.OnRemove.Subscribe(i =>
                {
                    Recalculate();
                });
                Storage<T1>.Instance.OnAdd.Subscribe(i =>
                {

                    Recalculate();
                });
                Storage<T2>.Instance.OnRemove.Subscribe(i =>
                {
                    Recalculate();
                });
                Storage<T2>.Instance.OnAdd.Subscribe(i =>
                {
                    Recalculate();
                });
                Recalculate();
            }



            private void Recalculate()
            {
                _list.Clear();
                _hashsetCurrent.Clear();
                var l1 = Storage<T1>.Instance.Aspects.Length;
                var l2 = Storage<T2>.Instance.Aspects.Length;

                for (var i = 0; i < l1 && i < l2; i++)
                    if (Storage<T1>.Instance.Aspects[i] != null &&
                        Storage<T2>.Instance.Aspects[i] != null)
                    {
                        _list.Add(i);
                        _hashsetCurrent.Add(i);
                        if (!_hashsetPrev.Contains(i))
                        {
                            OnChange.OnNext(this);
                            OnAdd.OnNext((i, Storage<T1>.Instance.Aspects[i], Storage<T2>.Instance.Aspects[i]));
                        }
                    }

                var listToRemove = new List<int>();
                foreach (var i in _hashsetPrev)
                {
                    if (!_hashsetCurrent.Contains(i))
                        listToRemove.Add(i);
                }

                _hashsetPrev.Clear();
                foreach (var i in _list)
                    _hashsetPrev.Add(i);

                foreach (var i in listToRemove)
                {
                    OnChange.OnNext(this);
                    OnRemove.OnNext((i, Storage<T1>.Instance.Aspects[i], Storage<T2>.Instance.Aspects[i]));
                }

            }
        }
    public class Filter<T1, T2, T3> where T1 : class where T2 : class where T3 : class
    {
        private static Filter<T1, T2, T3> _instance;
        private readonly HashSet<int> _hashsetCurrent = new HashSet<int>();
        private readonly HashSet<int> _hashsetPrev = new HashSet<int>();
        private readonly List<int> _list = new List<int>();

        public static Filter<T1, T2, T3> Instance => _instance ??= new Filter<T1, T2, T3>();

        public readonly Subject<(int, T1, T2, T3)> OnAdd = new Subject<(int, T1, T2, T3)>();
        public readonly Subject<(int, T1, T2, T3)> OnRemove = new Subject<(int, T1, T2, T3)>();
        public readonly Subject<Filter<T1, T2, T3>> OnChange = new Subject<Filter<T1, T2, T3>>();
        public IReadOnlyList<int> IndexList => _list;
        public HashSet<int> IndexHash => _hashsetCurrent;
        public Filter()
        {
            Storage<T1>.Instance.OnRemove
                .Merge(Storage<T1>.Instance.OnAdd)
                .Merge(Storage<T2>.Instance.OnRemove)
                .Merge(Storage<T2>.Instance.OnAdd)
                .Merge(Storage<T3>.Instance.OnRemove)
                .Merge(Storage<T3>.Instance.OnAdd)
                .Subscribe(i => Recalculate());
            Recalculate();
        }

        private void Recalculate()
        {
            _list.Clear();
            _hashsetCurrent.Clear();
            var l1 = Storage<T1>.Instance.Aspects.Length;
            var l2 = Storage<T2>.Instance.Aspects.Length;
            var l3 = Storage<T3>.Instance.Aspects.Length;

            for (var i = 0; i < l1 && i < l2 && i < l3; i++)
                if (Storage<T1>.Instance.Aspects[i] != null &&
                    Storage<T2>.Instance.Aspects[i] != null &&
                    Storage<T3>.Instance.Aspects[i] != null)
                {
                    _list.Add(i);
                    _hashsetCurrent.Add(i);
                    if (!_hashsetPrev.Contains(i))
                    {
                        OnChange.OnNext(this);
                        OnAdd.OnNext((i, Storage<T1>.Instance.Aspects[i], Storage<T2>.Instance.Aspects[i], Storage<T3>.Instance.Aspects[i]));
                    }
                }

            var listToRemove = new List<int>();
            foreach (var i in _hashsetPrev)
            {
                if (!_hashsetCurrent.Contains(i))
                    listToRemove.Add(i);
            }

            _hashsetPrev.Clear();
            foreach (var i in _list)
                _hashsetPrev.Add(i);

            foreach (var i in listToRemove)
            {
                OnChange.OnNext(this);
                OnRemove.OnNext((i, Storage<T1>.Instance.Aspects[i], Storage<T2>.Instance.Aspects[i], Storage<T3>.Instance.Aspects[i]));
            }
        }
    }
}