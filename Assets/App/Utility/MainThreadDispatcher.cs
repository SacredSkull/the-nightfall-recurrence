using UnityUtilities.Management;

namespace Utility {
	public class MainThreadDispatcher : IDispatcher {
		public void Post(System.Action<object> action, object state = null) {
			UniRx.MainThreadDispatcher.Post(action, state);
		}
	}
}