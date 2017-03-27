//
// ConsoleErrorMessages.cs
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


namespace PersimmonRadiant.Utils
{
	/// <summary>
	/// Console error messages strings.
	/// </summary>
	public static class ConsoleErrorMessages
	{
		const string functionNotFound = "Shell: Function not found.\n";

		const string noSignatureMatch = "Shell: No matching function found.\nMulti-signature function was invoked, but no signature matches the command line.\n";

		const string nonemptyArguments = "Shell: Attempted to call function that does not take arguments using one or more arguments.\n";

		const string tooManyArgsNotVarargs = "Shell: Wrong number of arguments.\nToo many arguments. The function does not accept a variable number of arguments. Please read the documentation.\n";

		const string tooFewArguments = "Shell: Wrong number of arguments.\nToo few arguments. Please read the documentation.\n";

		const string argumentTypeMismatch = "Shell: Argument type mismatch.\nThe type of the argument {0} should be {1}, but {2} was specified.\n";

		const string genericFunctionArgumentsMismatch = "Shell: Unable to call function with specified parameters.\nThe arguments do not match the generic function.\n";

		public static string FunctionNotFound {
			get {
				return functionNotFound;
			}
		}

		public static string NoSignatureMatch {
			get {
				return noSignatureMatch;
			}
		}

		public static string NonemptyArguments {
			get {
				return nonemptyArguments;
			}
		}

		public static string TooManyArgsNotVarargs {
			get {
				return tooManyArgsNotVarargs;
			}
		}

		public static string TooFewArguments {
			get {
				return tooFewArguments;
			}
		}

		public static string ArgumentTypeMismatch {
			get {
				return argumentTypeMismatch;
			}
		}

		public static string GenericFunctionArgumentsMismatch {
			get {
				return genericFunctionArgumentsMismatch;
			}
		}
	}
}

