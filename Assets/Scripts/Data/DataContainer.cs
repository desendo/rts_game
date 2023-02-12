using UnityEngine;

namespace Data
{
    public class DataContainer<T> : ScriptableObject where T : class, IData
    {
        public T Data;
    }

    public interface IData
    {
    }
}
