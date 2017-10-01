// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Algorithms;
using Gamelogic.Extensions.Internal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Provides some additional functions for MonoBehaviour.
	/// </summary>
	[Version(1)]
	public class GLMonoBehaviour : MonoBehaviour
	{
		#region Static Methods

#if !UNITY_5
		/// <summary>
		/// Instantiates the specified prefab.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prefab">The object.</param>
		/// <returns>T.</returns>
		public static T Instantiate<T>(T prefab) where T : Component
		{
			return (T) Object.Instantiate(prefab);
		}
#endif

		/// <summary>
		/// Instantiates an object at the 
		/// given position in the given orientation.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prefab">The prefab to instantiate.</param>
		/// <param name="position">The position.</param>
		/// <param name="rotation">The rotation.</param>
		/// <returns>T.</returns>
		public static T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
		{
			var newObj = Instantiate<T>(prefab);

			newObj.transform.position = position;
			newObj.transform.rotation = rotation;

			return newObj;
		}


		/// <summary>
		/// Instantiates a prefab and attaches it to the given root. 
		/// </summary>
		public static T Instantiate<T>(T prefab, GameObject root) where T : Component
		{
			var newObj = (T)Object.Instantiate(prefab);
			newObj.transform.parent = root.transform;
			newObj.transform.ResetLocal();

			return newObj;
		}

		/// <summary>
		/// Instantiates a prefab, attaches it to the given root, and
		/// sets the local position and rotation.
		/// </summary>
		public static T Instantiate<T>(T prefab, GameObject root, Vector3 localPosition, Quaternion localRotation) where T : Component
		{
			var newObj = Instantiate<T>(prefab);

			newObj.transform.parent = root.transform;

			newObj.transform.localPosition = localPosition;
			newObj.transform.localRotation = localRotation;
			newObj.transform.ResetScale();

			return newObj;
		}


		/// <summary>
		/// Instantiates a prefab.
		/// </summary>
		/// <param name="prefab">The object.</param>
		/// <returns>GameObject.</returns>
		public static GameObject Instantiate(GameObject prefab)
		{
			return (GameObject) Object.Instantiate(prefab);
		}


		/// <summary>
		/// Instantiates the specified prefab.
		/// </summary>
		public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			var newObj = (GameObject) Object.Instantiate(prefab, position, rotation);

			return newObj;
		}


		/// <summary>
		/// Instantiates a prefab and parents it to the root.
		/// </summary>
		/// <param name="prefab">The prefab.</param>
		/// <param name="root">The root.</param>
		/// <returns>GameObject.</returns>
		public static GameObject Instantiate(GameObject prefab, GameObject root)
		{
			var newObject = (GameObject)Object.Instantiate(prefab);
			newObject.transform.parent = root.transform;
			newObject.transform.ResetLocal();

			return newObject;
		}


		/// <summary>
		/// Instantiates a prefab, attaches it to the given root, and
		/// sets the local position and rotation.
		/// </summary>
		/// <param name="prefab">The prefab.</param>
		/// <param name="root">The root.</param>
		/// <param name="localPosition">The local position.</param>
		/// <param name="localRotation">The local rotation.</param>
		/// <returns>GameObject.</returns>
		public static GameObject Instantiate(GameObject prefab, GameObject root, Vector3 localPosition, Quaternion localRotation)
		{
			var newObj = (GameObject)Object.Instantiate(prefab);

			newObj.transform.parent = newObj.transform;
			newObj.transform.localPosition = localPosition;
			newObj.transform.localRotation = localRotation;
			newObj.transform.ResetScale();

			return newObj;
		}
		#region Find


		/// <summary>
		/// Similar to FindObjectsOfType, except that it looks for components
		/// that implement a specific interface.
		/// </summary>
		public static List<I> FindObjectsOfInterface<I>() where I : class
		{
			var monoBehaviours = FindObjectsOfType<MonoBehaviour>();

			return monoBehaviours.Select(behaviour => behaviour.GetComponent(typeof (I))).OfType<I>().ToList();
		}

		#endregion

		#endregion

		[Version(2, 3)]
		public Coroutine Invoke(System.Action action, float time)
		{
			return MonoBehaviourExtensions.Invoke(this, action, time);
		}

		[Version(2, 3)]
		public Coroutine InvokeRepeating(System.Action action, float time, float repeatTime)
		{
			return MonoBehaviourExtensions.InvokeRepeating(this, action, time, repeatTime);
		}

		[Version(2, 3)]
		public Coroutine InvokeRepeating(System.Action action, IGenerator<float> repeatTime)
		{
			return MonoBehaviourExtensions.InvokeRepeating(this, action, repeatTime);
		}

		[Version(2, 3)]
		public Coroutine Tween<T>(
			T start,
			T finish,
			float totalTime,
			Func<T, T, float, T> lerp,
			Action<T> action,
			Func<float> deltaTime)
		{
			return MonoBehaviourExtensions.Tween(this, start, finish, totalTime, lerp, action, deltaTime);
		}

		[Version(2, 3)]
		public Coroutine Tween<T>(
			T start,
			T finish,
			float totalTime,
			Func<T, T, float, T> lerp,
			Action<T> action)
		{
			return MonoBehaviourExtensions.Tween(this, start, finish, totalTime, lerp, action);
		}
	}

	///<summary>
	/// Provides useful extension methods for MonoBehaviours.
	/// </summary>
	[Version(1)]
	public static class MonoBehaviourExtensions
	{
		#region Cloning

		/// <summary>
		/// Clones an object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T Clone<T>(this T obj) where T:MonoBehaviour
		{
			return GLMonoBehaviour.Instantiate<T>(obj);
		}

		/// <summary>
		/// Clones an object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static List<T> Clone<T>(this T obj, int count) where T : MonoBehaviour
		{
			var list = new List<T>();

			for (int i = 0; i < count; i++)
			{
				list.Add(obj.Clone<T>());
			}

			return list;
		}

		#endregion

		#region Typesafe methods for scheduling

		/// <summary>
		/// Invokes the given action after the given amount of time.
		/// </summary>
		public static Coroutine Invoke(this MonoBehaviour monoBehaviour, System.Action action, float time)
		{
			return monoBehaviour.StartCoroutine(InvokeImpl(action, time));
		}

		private static IEnumerator InvokeImpl(System.Action action, float time)
		{
			yield return new WaitForSeconds(time);

			action();
		}

		/// <summary>
		/// Invokes the given action after the given amount of time, and repeats the 
		/// action after every repeatTime seconds.
		/// </summary>
		public static Coroutine InvokeRepeating(this MonoBehaviour monoBehaviour, System.Action action, float time, float repeatTime)
		{
			return monoBehaviour.StartCoroutine(InvokeRepeatingImpl(action, time, repeatTime));
		}

		/// <summary>
		/// Invokes the given action after the given amount of time, and repeats the 
		/// action after every repeatTime seconds.
		/// </summary>
		public static Coroutine InvokeRepeating(this MonoBehaviour monoBehaviour, System.Action action, IGenerator<float> repeatTime)
		{
			return monoBehaviour.StartCoroutine(InvokeRepeatingImpl(action, repeatTime));
		}

		private static IEnumerator InvokeRepeatingImpl(System.Action action, float time, float repeatTime)
		{
			yield return new WaitForSeconds(time);

			while (true)
			{
				action();
				yield return new WaitForSeconds(repeatTime);
			}
		}

		private static IEnumerator InvokeRepeatingImpl(System.Action action, IGenerator<float> repeatTime)
		{
			while (true)
			{
				yield return new WaitForSeconds(repeatTime.Next());

				action();
			}
		}

		/// <summary>
		/// Cancels the action if it was scheduled.
		/// </summary>
		/// <exception cref="NotSupportedException"></exception>
		[Obsolete("The new Invoke is implemented as a coroutine. Store and cancel the coroutine instead.")]
		public static void CancelInvoke(this MonoBehaviour component, System.Action action)
		{
			throw new NotSupportedException();
		}


		/// <summary>
		/// Returns whether an invoke is pending on an action.
		/// </summary>
		/// <exception cref="NotSupportedException"></exception>
		[Obsolete("The new Invoke is implemented as a coroutine. Store and cancel the coroutine instead.")]
		public static bool IsInvoking(this MonoBehaviour component, System.Action action)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region
		public static IEnumerator TweenImpl<T>(
			T start, 
			T finish, 
			float totalTime, 
			Func<T, T, float, T> lerp,
			Action<T> action,
			Func<float> deltaTime)
		{
			float time = 0;
			float t = 0;

			while (t < 1)
			{
				var current = lerp(start, finish, t);
				action(current);

				time += deltaTime();
				t = time / totalTime;

				yield return null;
			}

			action(finish);
		}

		public static Coroutine Tween<T>(this MonoBehaviour monoBehaviour, T start, T finish, float totalTime, Func<T, T, float, T> lerp, Action<T> action)
		{
			return Tween(monoBehaviour, start, finish, totalTime, lerp, action, () => Time.deltaTime);
		}

		public static Coroutine Tween<T>(
			this MonoBehaviour monoBehaviour, 
			T start, 
			T finish, 
			float totalTime, 
			Func<T, T, float, T> lerp, 
			Action<T> action, 
			Func<float> deltaTime)
		{
			return monoBehaviour.StartCoroutine(TweenImpl(start, finish, totalTime, lerp, action, deltaTime));
		}
		#endregion

		#region Children

		public static GameObject FindChild(this Component component, string childName)
		{
			return component.transform.Find(childName).gameObject;
		}

		public static GameObject FindChild(this Component component, string childName, bool recursive)
		{
			if (recursive) return component.FindChild(childName);

			return FindChildRecursively(component.transform, childName);
		}

		private static GameObject FindChildRecursively(Transform target, string childName)
		{
			if (target.name == childName) return target.gameObject;

			for (var i = 0; i < target.childCount; ++i)
			{
				var result = FindChildRecursively(target.GetChild(i), childName);

				if (result != null) return result;
			}

			return null;
		}

		/// <summary>
		/// Finds a component of the type T in on the same object, or on a child down the hierarchy. This method also works
		/// in the editor and when the game object is inactive.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="component">The component.</param>
		/// <returns>T.</returns>
		[Version(1, 1)]
		public static T GetComponentInChildrenAlways<T>(this Component component) where T : Component
		{
			foreach (var child in component.transform.SelfAndAllChildren())
			{
				var componentInChild = child.GetComponent<T>();

				if (componentInChild != null)
				{
					return componentInChild;
				}
			}

			return null;
		}

		/**
			Finds all components of the type T on the same object and on a children down the hierarchy. This method also works
			in the editor and when the game object is inactive.

			@version_e_1_1
		*/

		public static T[] GetComponentsInChildrenAlways<T>(this Component component) where T : Component
		{
			var components = new List<T>();

			foreach (var child in component.transform.SelfAndAllChildren())
			{
				var componentsInChild = child.GetComponents<T>();

				if (componentsInChild != null)
				{
					components.AddRange(componentsInChild);
				}
			}

			return components.ToArray();
		}

		#endregion

		#region Components

		/// <summary>
		/// Gets a component of the given type, or fail if no such component is attached to the given component.
		/// </summary>
		/// <typeparam name="T">The type of component to get.</typeparam>
		/// <param name="thisComponent">The component to check.</param>
		/// <returns>A component of type T attached to the given component if it exists.</returns>
		/// <exception cref="InvalidOperationException">When the no component of the required type exist on the given component.</exception>
		//TODO Implement variants

		//Make this a instance method too
		public static T GetRequiredComponent<T>(this Component thisComponent) where T : Component
		{
			var retrievedComponent = thisComponent.GetComponent<T>();

			if (retrievedComponent == null)
			{
				throw new InvalidOperationException(string.Format("GameObject \"{0}\" ({1}) does not have a component of type {2}", thisComponent.name, thisComponent.GetType(), typeof(T)));
			}

			return retrievedComponent;
		}

		public static T GetRequiredComponentInChildren<T>(this Component thisComponent) where T : Component
		{
			var retrievedComponent = thisComponent.GetComponentInChildren<T>();

			if (retrievedComponent == null)
			{
				throw new InvalidOperationException(string.Format("GameObject \"{0}\" ({1}) does not have a child with component of type {2}", thisComponent.name, thisComponent.GetType(), typeof(T)));
			}

			return retrievedComponent;
		}


		/// <summary>
		/// Gets an attached component that implements the interface of the type parameter.
		/// </summary>
		/// <typeparam name="TInterface">The type of the t interface.</typeparam>
		/// <param name="thisComponent">The this component.</param>
		/// <returns>TInterface.</returns>
		public static TInterface GetInterfaceComponent<TInterface>(this Component thisComponent) where TInterface : class
		{
			return thisComponent.GetComponent(typeof(TInterface)) as TInterface;
		}
		#endregion
	}
}