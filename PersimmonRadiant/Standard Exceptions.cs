//
// Standard Exceptions.cs
// 
// Author:
//      Mălin Stănescu malin.stanescu@gmail.com
//
// Copyright (c) 2017 Stănescu Mălin
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.


using System;
using System.Runtime.Serialization;
using PersimmonRadiant.Utils;

namespace PersimmonRadiant.Exceptions
{
	/// <summary>
	/// Base class for exceptions raised by the shell.
	/// </summary>
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

	/// <summary>
	/// Exception raised on mismatch in arguments to functions.
	/// </summary>
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

	/// <summary>
	/// Exception raised on functions not found.
	/// </summary>
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