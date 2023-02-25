using System;
using System.Collections.Generic;
using UniRx;

namespace Services
{
    public class Storage
    {
        protected Dictionary<Type, HashSet<int>> _vacantIndexes = new Dictionary<Type, HashSet<int>>();
    }

    public class Storage<T> : Storage where T : class
    {
        private static Storage<T> _instance;
        public readonly Subject<int> OnAdd = new Subject<int>();
        public readonly Subject<int> OnRemove = new Subject<int>();
        private T[] _aspects = new T[16];

        private int _lastIndex = 0;
        public T[] Aspects => _aspects;
        public static Storage<T> Instance => _instance ??= new Storage<T>();

        public T Get(int index)
        {
            if (index >= Aspects.Length)
                return null;

            return _aspects[index];
        }

        public T Set(T aspect, int index)
        {
            if (index >= Instance._aspects.Length)
                Array.Resize(ref Instance._aspects, index + index / 2);
            _aspects[index] = aspect;
            OnAdd.OnNext(index);
            if (index >= _lastIndex)
                _lastIndex = index;
            return aspect;
        }

        public int Add(T aspect)
        {
            var index = _lastIndex;
            Set(aspect, index);
            _lastIndex++;
            return index;
        }

        public void Remove(int index)
        {
            _aspects[index] = null;
            OnRemove.OnNext(index);
        }

        public bool Has(int index)
        {
            return _aspects.Length > index && _aspects[index] != null;
        }
    }
}