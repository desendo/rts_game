using System.Collections.Generic;
using UnityEngine;

namespace Services.PrefabPool
{
	public interface IPrefabPool { }

	public abstract class AbstractPrefabPool<T> : IPrefabPool where T : MonoBehaviour, IPoolableObject
	{
		protected Transform _root;
		protected readonly Queue<T> _freeObjects = new Queue<T>();

		public AbstractPrefabPool() { }

		public AbstractPrefabPool(int count)
		{
			CreateInitial(count);
		}

		protected void CreateInitial(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var newObj = CreateNew(_root);
				_freeObjects.Enqueue(newObj);
				OnDespawned(newObj);
			}
		}

		protected virtual Transform CreateRoot(string name, Transform root)
		{
			var go = new GameObject(name);
			go.transform.SetParent(root);

			return go.transform;
		}

		public virtual T Spawn(Transform parent = null)
		{
			var obj = _freeObjects.Count > 0
						  ? _freeObjects.Dequeue()
						  : CreateNew(parent);

			OnSpawned(obj);

			return obj;
		}

		public virtual void Despawn(T instance)
		{
			if (_freeObjects.Contains(instance))
				Debug.LogWarning($"Intstance {instance} already despawned");
			_freeObjects.Enqueue(instance);
			OnDespawned(instance);
		}

		protected abstract T CreateNew(Transform parent = null);

		protected abstract void OnSpawned(T obj);

		protected abstract void OnDespawned(T obj);

		public abstract void Clear();
	}
}