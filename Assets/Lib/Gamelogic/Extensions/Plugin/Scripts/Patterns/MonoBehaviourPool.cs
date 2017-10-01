using System;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// A pool suitable for MonoBehaviour objects that can be instantiated from a given prefab. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MonoBehaviourPool<T> where T : MonoBehaviour
	{
		private readonly Pool<T> pool;

		/// <summary>
		/// Gets a value indicating whether a sleeping object is available.
		/// </summary>
		/// <value><c>true</c> if a sleeping object is available; otherwise, <c>false</c>.</value>
		public bool IsObjectAvailable
		{
			get
			{
				return pool.IsObjectAvailable;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBehaviourPool{T}"/> class.
		/// </summary>
		/// <param name="prefab">The prefab used to instantiate objects from.</param>
		/// <param name="initialCount">The initial count of objects to create.</param>
		/// <param name="setToSleep">A function called on objects when they are put to sleep.</param>
		/// <param name="wakeUp">A function called on an object when it is woken up.</param>
		public MonoBehaviourPool(T prefab, int initialCount, Action<T> setToSleep, Action<T> wakeUp)
		{
			pool = new Pool<T>(
				initialCount, 
				() => (T) UnityEngine.Object.Instantiate(prefab), 
				UnityEngine.Object.Destroy, 
				setToSleep,
				wakeUp);
		}

		/// <summary>
		/// Gets a freshly awoken object from the pool.
		/// </summary>
		/// <returns>T.</returns>
		public T GetNewObject()
		{
			return pool.GetNewObject();
		}

		/// <summary>
		/// Releases the specified object back to the pool.
		/// </summary>
		public void ReleaseObject(T obj)
		{
			pool.Release(obj);
		}

		/// <summary>
		/// Increases thew capacity of the pool. 
		/// </summary>
		/// <param name="increment">The number of new pool objects to add.</param>
		public void IncCapacity(int increment)
		{
			pool.IncCapacity(increment);
		}

		/// <summary>
		/// Decreases the capacity of the pool.
		/// </summary>
		/// <param name="decrement">The number of pool objects to kill.</param>
		public void DecCapacity(int decrement)
		{
			pool.DecCapacity(decrement);
		}
	}
}