using UnityEngine;

namespace Services.PrefabPool
{
	public class ComponentPrefabPool<T> : AbstractPrefabPool<T> where T : MonoBehaviour, IPoolableObject
	{
		private T _prefab;

		public ComponentPrefabPool(T prefab, Transform root, int count)
		{
			_root = CreateRoot(prefab.name, root);
			_prefab = prefab;
			CreateInitial(count);
		}

		protected override T CreateNew(Transform parent)
		{
			return Object.Instantiate(_prefab, parent != null ? parent : _root);
		}

		public override T Spawn(Transform parent)
		{
			T obj = null;
			while (obj == null)
				obj = base.Spawn(parent);

			obj.transform.SetParent(parent, false);

			return obj;
		}

		public override void Despawn(T instance)
		{
			base.Despawn(instance);
			if (_root != null)
				instance.transform.SetParent(_root, false);
		}

		protected override void OnSpawned(T obj)
		{
			obj.OnSpawned();
		}

		protected override void OnDespawned(T obj)
		{
			obj.OnDespawned();
		}

		public override void Clear()
		{
			while (_freeObjects.Count > 0)
				Object.Destroy(_freeObjects.Dequeue());
		}
	}
}