//
//  Author:
//    Stănescu Mălin Octavian malin.stanescu@gmail.com
//
//  Copyright (c) 2016, Stănescu Mălin Octavian
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the [ORGANIZATION] nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System;
using System.Collections.Generic;

namespace PersimmonRadiant
{
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
				if (genpatt == cur.GetGenericTypeDefinition()) {
					return cur;
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
			foreach (var z in parType.GetGenericArguments ()) {
				List<Tuple<Type, Type>> w = ReplacementTypesInt (argType, z);
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
			return ReplacementTypesInt (st, parType);
		}
	}
}

