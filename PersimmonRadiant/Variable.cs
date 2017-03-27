//
// Variable.cs
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

