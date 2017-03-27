//
// TypeInference.cs
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
using System.Collections.Generic;

namespace PersimmonRadiant.Utils
{
	/// <summary>
	/// Functions for infering type parameters for generics.
	/// </summary>
	public static class TypeInference
	{
		/// <summary>
		/// Gets the GCD base class of the types in the arguments.
		/// </summary>
		/// <returns>The common base class.</returns>
		/// <param name="types">Types.</param>
		[System.Diagnostics.Contracts.Pure]
		public static Type GetCommonBaseClass (params Type[] types)
		{
			/* This is a weeeeird case... should I throw? */
			if (types.Length == 0)
				return typeof (object);

			Type ret = types[0];

			for (int i = 1; i < types.Length; ++i) {
				if (types[i].IsAssignableFrom (ret))
					ret = types[i];
				else {
					while (!ret.IsAssignableFrom (types[i]))
						ret = ret.BaseType;
				}
			}

			return ret;
		}

		/// <summary>
		/// Gets the type of the base class that is an instance of the generic.
		/// </summary>
		/// <returns>The correct base type.</returns>
		/// <param name="genpatt">Generic type used as pattern.</param>
		/// <param name="overtype">Type in whose inheritance hierarchy to look.</param>
		[System.Diagnostics.Contracts.Pure]
		static Type GetCorrectBaseType (Type genpatt, Type overtype)
		{
			Type tc = overtype;
			while (tc != null && tc != typeof (object)) {
				Type cur = tc.IsGenericType ? tc.GetGenericTypeDefinition () : tc;
				if (genpatt.GUID == cur.GUID) {
					return tc;
				}
				tc = tc.BaseType;
			}
			return null;
		}

		/// <summary>
		/// Find types to replace in generic arguments to obtain correct generic
		/// </summary>
		/// <returns>A list of (generic argument, replacement type) tuples.</returns>
		/// <param name="argType">Argument (variable) type.</param>
		/// <param name="parType">Parameter type.</param>
		[System.Diagnostics.Contracts.Pure]
		static List<Tuple<Type, Type>> ReplacementTypesInt (Type argType, Type parType)
		{
			if (parType.IsGenericParameter) return new List<Tuple<Type, Type>> () { new Tuple<Type, Type> (argType, parType) };
			if (parType.IsArray) {
				if (!argType.IsArray) throw new ArgumentException ();
				return ReplacementTypesInt (argType.GetElementType (), parType.GetElementType ());
			}
			if (!parType.ContainsGenericParameters) return null;
			List<Tuple<Type, Type>> lts = new List<Tuple<Type, Type>> ();
			for (int i = 0; i < parType.GetGenericArguments ().Length; i++) {
				var z = parType.GetGenericArguments ()[i];
				List<Tuple<Type, Type>> w = ReplacementTypesInt (argType.GetGenericArguments ()[i], z);
				if (w != null) lts.AddRange (w);
			}

			return lts;
		}

		/// <summary>
		/// Find types to replace in generic arguments to obtain correct generic
		/// </summary>
		/// <returns>A list of (generic argument, replacement type) tuples.</returns>
		/// <param name="argType">Argument (variable) type.</param>
		/// <param name="parType">Parameter type.</param>
		/// [System.Diagnostics.Contracts.Pure]
		public static List<Tuple<Type, Type>> ReplacementTypes (Type argType, Type parType)
		{
			Type st = GetCorrectBaseType (parType, argType);
			if (st == null) return null;
			return ReplacementTypesInt (st, parType);
		}
	}
}

