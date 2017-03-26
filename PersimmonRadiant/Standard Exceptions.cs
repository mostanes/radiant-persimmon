using System;
using System.Runtime.Serialization;

namespace PersimmonRadiant.Exceptions
{
	[Serializable]
	public class ShellException : Exception
	{
		public ShellException ()
		{
		}

		public ShellException (string message) : base (message)
		{
		}

		public ShellException (string message, Exception innerException) : base (message, innerException)
		{
		}

		protected ShellException (SerializationInfo info, StreamingContext context) : base (info, context)
		{
		}
	}

	[Serializable]
	class ShellArgumentMismatch : ShellException
	{
		public ShellArgumentMismatch ()
		{
		}

		public ShellArgumentMismatch (string message) : base (message)
		{
		}

		public ShellArgumentMismatch (string message, Exception innerException) : base (message, innerException)
		{
		}

		protected ShellArgumentMismatch (SerializationInfo info, StreamingContext context) : base (info, context)
		{
		}
	}

	[Serializable]
	class ShellFunctionNotFound : ShellException
	{
		public ShellFunctionNotFound () : base (ConsoleErrorMessages.FunctionNotFound)
		{ }
	}

	class UserException : Exception
	{
	}
}