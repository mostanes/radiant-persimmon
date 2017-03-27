//
// Invokable.cs
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
using System.Reflection;

namespace PersimmonRadiant.Invoker
{
	public class Invokable
	{
		public readonly MethodInfo MInfo;
		public readonly ConsoleFunctionDescription Descriptor;
		internal Parametrization CallStd;

		public Invokable (MethodInfo info, ConsoleFunctionDescription descriptor)
		{
			MInfo = info;
			Descriptor = descriptor;
		}
	}

	/// <summary>
	/// Holds information on the function parameters;
	/// </summary>
	class Parametrization
	{
		public bool hp1, hp2, cfunc, generic;
	}
}

