//
//  Author:
//    Stănescu Mălin Octavian malin.stanescu@gmail.com
//
//  Copyright (c) 2016-2017, Stănescu Mălin Octavian
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
	/// <summary>
	/// Variable`T` base class.
	/// Used to specify a generic variable, without a type.
	/// See <see cref="PersimmonRadiant.Variable"/>
	/// </summary>
	[Serializable]
	public class VariableTBase
	{
		public Dictionary<string, VariableTBase> tags;
		public Dictionary<string, VariableTBase> innerVariables;
		public string name;
	}

	/// <summary>
	/// A console variable.
	/// </summary>
	[Serializable]
	public class Variable<T> : VariableTBase
	{
		public T obj;

		public Variable ()
		{
		}

		public Variable (T o, string Name)
		{
			obj = o;
			name = Name;
		}

		public static implicit operator T (Variable<T> v)
		{
			return v.obj;
		}

		public static implicit operator Variable<T>(T o)
		{
			Variable<T> v = new Variable<T> ();
			v.obj = o;
			return v;
		}

		public void AddTag<V> (string TagName, V TagValue)
		{
			lock(this) 
				if (tags == null)
					tags = new Dictionary<string, VariableTBase> ();
			Variable<V> v = new Variable<V> ();
			v.name = TagName;
			v.obj = TagValue;
			lock(tags) {
				v.tags = new Dictionary<string, VariableTBase> ();
				tags.Add (v.name, v);
			}
		}

		public object GetVarTag (string TagName)
		{
			lock (this)
				if (tags == null)
					throw new KeyNotFoundException ();
			lock (tags)
				if (tags.ContainsKey (TagName))
					return tags[TagName];
				else
					throw new KeyNotFoundException ();

		}

		public V GetTag<V> (string TagName)
		{
			object o = null;
			lock (this)
				if (tags == null)
					throw new KeyNotFoundException ();
			lock(tags) {
				if (!tags.ContainsKey (TagName))
					throw new KeyNotFoundException ();
				o = tags[TagName];
			}
			if (o is Variable<V>) {
				Variable<V> tag = (Variable<V>)o;
				return tag.obj;
			} else
				throw new InvalidCastException ();
		}

		public void AddInnerVar (string VarName, VariableTBase vb)
		{
			lock(this) {
				if (innerVariables == null)
					innerVariables = new Dictionary<string, VariableTBase> ();
			}
			VariableTBase v = vb;
			v.name = VarName;
			lock(innerVariables)
				innerVariables.Add (v.name, v);
		}

		public VariableTBase GetInnerVar (string VarName)
		{
			lock (this)
				if (innerVariables == null)
					throw new KeyNotFoundException ();
			lock (innerVariables)
				if (innerVariables.ContainsKey (VarName))
					return innerVariables[VarName];
				else
					throw new KeyNotFoundException ();
		}

		public Variable<V> GetInnerVar<V> (string VarName)
		{
			object o = null;
			lock (this)
				if (innerVariables == null)
					throw new KeyNotFoundException ();
			lock (innerVariables) {
				if (!innerVariables.ContainsKey (VarName))
					throw new KeyNotFoundException ();
				o = innerVariables[VarName];
			}
			if (o is Variable<V>) {
				Variable<V> vb = (Variable<V>)o;
				return vb;
			} else
				throw new InvalidCastException ();
		}
	}

}

