using System;

namespace Services.PrefabPool
{
	public interface IPoolableObject : IDisposable
	{
		void OnSpawned();

		void OnDespawned();
	}
}