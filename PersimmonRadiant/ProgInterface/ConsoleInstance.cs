//
// ConsoleInstance.cs
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
using System.Collections.Generic;
using System.Text;
using PersimmonRadiant.Invoker;

namespace PersimmonRadiant
{
	public class ConsoleInstance
	{
		StringBuilder screen;
		Variable<object> variableHolder;
		ConsoleAccess instanceDelegates;
		InvokerSet availableFunctions;

		public ConsoleInstance ()
		{
			screen = new StringBuilder ();
			variableHolder = new Variable<object> ();
			instanceDelegates.WriteText = WriteTextOnConsoleBuffer;
			instanceDelegates.ActiveInstance = this;
			availableFunctions = new InvokerSet (instanceDelegates);
		}

		public void LoadFunctions (params System.Reflection.Assembly[] assemblies)
		{
			availableFunctions.FindFunctions (assemblies);
		}

		void WriteTextOnConsoleBuffer (string text)
		{
			screen.Append (text);
		}

		public void InterpretLine (string line)
		{
			string[][] commlist = Tokenizer.Tokenize (line);
			bool hasret;
			hasret = false;
			int funcpos, varstart;
			List<int> varpos = new List<int> ();

			foreach (string[] cs in commlist) {
				varpos.Clear ();
				funcpos = 0;
				varstart = 1;
				if (cs.Length == 0)
					continue;
				if (cs.Length == 2)
					varstart = 1;
				if (cs.Length > 2) {
					if (cs[1] == "=") {
						hasret = true;
						funcpos = 2;
						varstart = 3;
					} else
						varstart = 1;
				}

				/* Fetch the arguments */
				List<VariableTBase> argset = new List<VariableTBase> (cs.Length);
				int i;
				for (i = varstart; i < cs.Length; i++) {
					if ((cs[i][0] == '$') | (cs[i][0] == '#')) {
						string[] spl = cs[i].Split ('.');
						VariableTBase vtb;
						try {
							vtb = variableHolder;
							foreach (string g in spl) vtb = vtb.innerVariables[g];
							argset.Add (vtb);
						} catch (KeyNotFoundException) {
							WriteTextOnConsoleBuffer ("No such variable: " + cs[i]);
							return;
						}
					} else {
						int tint;
						if (int.TryParse (cs[i], out tint)) {
							Variable<int> tmpvar = tint;
							tmpvar.name = "temp";
							argset.Add (tmpvar);
						} else {
							Variable<string> tmpvar = cs[i];
							tmpvar.name = "temp";
							argset.Add (tmpvar);
						}
					}
				}

				/* Call the function & set the return value */
				bool debug = true;
				if (!debug) {
					try {
						VariableTBase o = availableFunctions.Invoke (cs[funcpos], argset.ToArray ());
						if (hasret) {
							o.name = cs[0];
							variableHolder.AddInnerVar (o.name, o);
						}
					} catch (Exceptions.UserException e) {
						WriteTextOnConsoleBuffer (e.Message);
						throw e;
					} catch (Exception e) {
						WriteTextOnConsoleBuffer ("\n--------\nRuntime error: " + e.Message + "\nProbably bad code. Ask the nearest suitable programmer for help.\n--------");
						System.Diagnostics.Debug.WriteLine (e.ToString (), "Runnable function errors.");
						System.Diagnostics.Trace.WriteLine (e.ToString (), "Runnable function errors.");
						throw e;
					}
				} else {
					VariableTBase o = availableFunctions.Invoke (cs[funcpos], argset.ToArray ());
					if (hasret) {
						o.name = cs[0];
						variableHolder.AddInnerVar (o.name, o);
					}
				}
			}
		}
	}

	/// <summary>
	/// Delegate for displaying text on the console.
	/// </summary>
	public delegate void TextDisplay (string msg);

	/// <summary>
	/// Set of delegates used by the console functions.
	/// </summary>
	public struct ConsoleAccess
	{
		public TextDisplay WriteText;
		public ConsoleInstance ActiveInstance;
	}
}

