using System.Collections.Generic;
using UnityEngine;

namespace Services.PrefabPool
{
	public class PrefabPool
	{
		private static PrefabPool _instanceGlobal;

		public PrefabPool()
		{
			var root = new GameObject("PrefabPoolRoot");
			Object.DontDestroyOnLoad(root);
			Initialize(root.transform);
		}

		public static PrefabPool InstanceGlobal => _instanceGlobal ??= new PrefabPool();

		private Transform _root;
		private readonly Dictionary<int, IPrefabPool> _prefabToPool = new Dictionary<int, IPrefabPool>();
		private readonly Dictionary<int, int> _instanceToPrefab = new Dictionary<int, int>();

		public void Initialize(Transform root)
		{
			_root = root;
		}

		public T Spawn<T>(T prefab, Transform parent = null) where T : MonoBehaviour, IPoolableObject
		{
			var prefabId = prefab.gameObject.GetInstanceID();
			var pool = GetPool(prefab) ?? Install(prefab, 1);
			var obj = pool.Spawn(parent);
			_instanceToPrefab[obj.gameObject.GetInstanceID()] = prefabId;

			return obj;
		}

		private ComponentPrefabPool<T> GetPool<T>(T prefab) where T : MonoBehaviour, IPoolableObject
		{
			var prefabId = prefab.gameObject.GetInstanceID();

			if (!_prefabToPool.TryGetValue(prefabId, out var pool))
				return null;

			return (ComponentPrefabPool<T>) pool;
		}

		public void Despawn<T>(T instance) where T : MonoBehaviour, IPoolableObject
		{
			if (!IsValidDespawnableObject(instance.gameObject.GetInstanceID(), out var pool))
			{
				if(instance != null)
					Object.Destroy(instance.gameObject);
				else
					return;
			}
			_instanceToPrefab.Remove(instance.gameObject.GetInstanceID());
			((ComponentPrefabPool<T>) pool).Despawn(instance);
		}

		/// <summary>
		///     Clears all pools with type T or you can specify prefab so then only one pool will be cleared.
		///     Only free(pooled) objects are cleared.
		/// </summary>
		/// <param name="prefab"></param>
		/// <typeparam name="T"></typeparam>
		public void Clear<T>(T prefab = null) where T : MonoBehaviour, IPoolableObject
		{
			if (prefab != null)
			{
				ComponentPrefabPool<T> pool = GetPool(prefab);
				pool?.Clear();
			}
			else
			{
				foreach (var pool in _prefabToPool.Values)
				{
					if (pool is ComponentPrefabPool<T> typedPool)
						typedPool.Clear();
				}
			}
		}

		private bool IsValidDespawnableObject(int instanceId, out IPrefabPool pool)
		{
			pool = null;
			if (!_instanceToPrefab.TryGetValue(instanceId, out var poolId))
			{
				Debug.LogWarning("Despawnable object has no pool id");

				return false;
			}

			if (!_prefabToPool.TryGetValue(poolId, out pool))
			{
				Debug.LogWarning("Despawnable object has no pool");

				return false;
			}

			return true;
		}

		public ComponentPrefabPool<T> Install<T>(T prefab, int count) where T : MonoBehaviour, IPoolableObject
		{
			//TODO resolve the case when _root is null - set default root "Pool" and print notification
			var pool = new ComponentPrefabPool<T>(prefab, _root, count);
			var poolId = prefab.gameObject.GetInstanceID();
			_prefabToPool[poolId] = pool;

			return pool;
		}
	}
}