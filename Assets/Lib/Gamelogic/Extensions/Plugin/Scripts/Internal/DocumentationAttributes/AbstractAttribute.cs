// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;

namespace Gamelogic.Extensions.Internal
{
	/// <summary>
	/// Use to mark classes and methods that are abstract, but cannot be implemented as such
	/// because Unity it needs to be public to work with Unity.
	/// </summary>
	[Version(1, 4)]
	[AttributeUsage(AttributeTargets.Class |
					AttributeTargets.Struct | 
					AttributeTargets.Method |
					AttributeTargets.Property)]
	public sealed class AbstractAttribute : Attribute
	{}
}
