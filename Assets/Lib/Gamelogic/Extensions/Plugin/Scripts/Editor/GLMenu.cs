using UnityEngine;
using UnityEditor;

namespace Gamelogic.Editor.Internal
{
	/// <summary>
	/// Class with static functions for menu options.
	/// </summary>
	public partial class GLMenu
	{
		public static void OpenUrl(string url)
		{
			Application.OpenURL(url);
		}

		[MenuItem("Gamelogic/Email Support")]
		public static void OpenSupportEmail()
		{
			OpenUrl("mailto:support@gamelogic.co.za");
		}

		[MenuItem("Gamelogic/Online KnowledgeBase")]

		public static void OpenKnowledgeBase()
		{
			OpenUrl("https://gamelogic.quandora.com/");
		}


		[MenuItem("Gamelogic/Extensions/API Documentation")]
		public static void OpenExtensionsAPI()
		{
			OpenUrl("http://www.gamelogic.co.za/documentation/extensions/");
		}
	}
}