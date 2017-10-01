using System;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Thrown when a method is called with illegal type parameters, or a class is constructed with 
	/// illegal type parameters. 
	/// </summary>
	/// <remarks>Normally, it is preferable to use type constraints, but in some cases this is not 
	/// possible. This exception can be thrown in such cases.</remarks>
	public class TypeArgumentException : Exception
	{
		public readonly string parameterName;

		public TypeArgumentException(string message)
			: base(message)
		{
			parameterName = "";
		}

		public TypeArgumentException(string parameterName, string message)
			: base(string.Format("{0}\nParameter Name: {1}", message, parameterName))
		{
			this.parameterName = parameterName;
		}
	}
}
