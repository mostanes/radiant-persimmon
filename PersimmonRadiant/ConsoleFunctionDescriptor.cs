//
// ConsoleFunctionDescriptor.cs
// 
// Author:
//      Mălin Stănescu malin.stanescu@gmail.com
//
// Copyright (c) 2016-2017 Stănescu Mălin
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
using System.Reflection;

namespace PersimmonRadiant
{
	/// <summary>
	/// Descriptor for console functions.
	/// All console-accessible functions must have a descriptor that describes the function.
	/// </summary>
	[Serializable]
	public class ConsoleFunctionDescription
	{
		public readonly string Name;
		public readonly string OriginalName;
		/// <summary>
		/// Short description
		/// </summary>
		public readonly string Description;
		/// <summary>
		/// Long description
		/// </summary>
		public readonly string HelpStr;
		public readonly ParamInfo[] Arguments;
		public readonly ParamInfo RetVal;

		public ConsoleFunctionDescription (string FuncName, string ShortDescription, string HelpString, ParamInfo[] args, ParamInfo retval)
		{
			Name = FuncName;
			Description = ShortDescription;
			HelpStr = HelpString;
			Arguments = args;
			RetVal = retval;
		}

		public ConsoleFunctionDescription (string FuncName, string Original, string ShortDescription, string HelpString, ParamInfo[] args, ParamInfo retval)
		{
			Name = FuncName;
			OriginalName = Original;
			Description = ShortDescription;
			HelpStr = HelpString;
			Arguments = args;
			RetVal = retval;
		}
	}

	/// <summary>
	/// Parameter info for a console function.
	/// </summary>
	[Serializable]
	public struct ParamInfo
	{
		public readonly string Name;
		public readonly ParameterInfo Pi;
		public readonly string Description;
		public readonly int Number;
		public readonly string TypeName;
		public readonly bool IsParams;

		public ParamInfo (string name, ParameterInfo pi, ParamInfo prevPI)
		{
			Name = name;
			Pi = pi;
			Description = prevPI.Description;
			Number = prevPI.Number;
			TypeName = prevPI.TypeName;
			IsParams = prevPI.IsParams;
		}

		public ParamInfo (int number, string description, string typeName, bool parms)
		{
			Number = number;
			Description = description;
			TypeName = typeName;
			IsParams = parms;
			Name = null;
			Pi = null;
		}
	}

}

