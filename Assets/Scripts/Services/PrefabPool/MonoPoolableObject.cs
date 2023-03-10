using UnityEngine;
using Views.UI.Elements;

namespace Services.PrefabPool
{
	public abstract class MonoPoolableObject : MonoBehaviour, IPoolableObject
	{
		public virtual void Awake() { }

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
		}

		protected void Dispose<T>(T instance) where T : MonoBehaviour, IPoolableObject
		{
			PrefabPool.InstanceGlobal.Despawn(instance);
		}
	}
}