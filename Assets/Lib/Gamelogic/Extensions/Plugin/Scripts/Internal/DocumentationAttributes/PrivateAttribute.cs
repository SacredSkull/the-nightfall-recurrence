using System;

namespace Gamelogic.Extensions.Internal
{
	/// <summary>
	/// Use to mark targets that are private, but cannot be implemented as such
	/// because Unity it needs to be public to work with Unity.
	/// </summary>
	[Version(2)]
	[AttributeUsage(AttributeTargets.All)]
	public sealed class PrivateAttribute : Attribute
	{ }
}