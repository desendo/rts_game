using UnityEngine;

namespace Services.PrefabPool
{
	public class MonoPoolableObject : MonoBehaviour, IPoolableObject
	{
		protected virtual void Awake() { }

		public virtual void OnSpawned()
		{
			gameObject.SetActive(true);
		}


		public virtual void OnDespawned()
		{
			gameObject.SetActive(false);
		}

		public virtual void Dispose()
		{
			PrefabPool.InstanceGlobal.Despawn(this);
		}
	}
}