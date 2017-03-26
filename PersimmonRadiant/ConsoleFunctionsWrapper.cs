using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using PersimmonRadiant.Exceptions;

namespace PersimmonRadiant
{
	public class ConsoleFunctionWrapper
	{
		//MethodInfo[] usableFunctions;
		Dictionary<string, List<Invokable>> functions;
		//Dictionary<string, List<MethodInfo>> funclist;
		//Dictionary<string, List<ConsoleFunctionDescription>> cfdlist;
		private static Type funcp = typeof (ConsoleFunctionsProvider);
		private static Type funcm = typeof (ConsoleFunctionAttribute);

		/// <summary>
		/// Lists all registered functions
		/// </summary>
		/// <value>A list with descriptors of registered functions.</value>
		public List<ConsoleFunctionDescription> ListFunctions {
			get {
				List<ConsoleFunctionDescription> cfs = new List<ConsoleFunctionDescription> (cfdlist.Count);
				foreach (KeyValuePair<string, List<ConsoleFunctionDescription>> kvp in cfdlist)
					cfs.AddRange (kvp.Value);
				return cfs;
			}
		}

		/// <summary>
		/// Gets the dictionary of functions
		/// </summary>
		/// <value>The dictionary with functions descriptors.</value>
		public Dictionary<string, List<ConsoleFunctionDescription>> FunctionsDescriptors { get { return cfdlist; } }

		public ConsoleFunctionWrapper ()
		{
		}

		public void FindFunctions (params Assembly[] Assemblies)
		{
			List<Type> lt = new List<Type> ();
			/* Get all types available and remove those that do not provide console functions */
			foreach (Assembly s in Assemblies)
				lt.AddRange (s.GetTypes ());
			lt.RemoveAll ((Type obj) => (obj.GetCustomAttributes (funcp, false).Length == 0));

			/* Select consoel functions and populate the dictionary */
			List<MethodInfo> funcs = new List<MethodInfo> ();
			foreach (Type t in lt)
				funcs.AddRange (t.GetMethods ());
			var w = (from MethodInfo f in funcs
					 let x = f.GetCustomAttributes (funcm, true)
					 where x.Length != 0
					 let d = (ConsoleFunctionDescription)x[0]
					 select new KeyValuePair<string, Invokable> (d.Name, new Invokable ()));
			if (functions == null) functions = new Dictionary<string, List<Invokable>> ();
			foreach (var v in w)
				if (functions.ContainsKey (v.Key)) functions[v.Key].Add (v.Value); else functions.Add (v.Key, new List<Invokable> () { v.Value });
		}

		public VariableTBase InvokeFunction (string name, VariableTBase[] args, ConsoleAccess ds)
		{
			List<MethodInfo> mei = null;
			Type vbas = typeof (Variable<>);
			List<Type> genme = new List<Type> ();
			VariableTBase vtb;
			try {
				mei = funclist[name];
			} catch (KeyNotFoundException) {
				//throw new UserSpecifiedException (sfNFsimple);
			}
			if (mei.Count == 1) {
				return SingleFunctionCall (args, ds, mei[0]);
			} else {
				/* TODO: This select the first method found, not the best */
				foreach (MethodInfo mim in mei) {
					try {
						vtb = SingleFunctionCall (args, ds, mim);
					} catch (ShellArgumentMismatch) {
						continue;
					}

					return vtb;
				}
				//throw new UserSpecifiedException (msFuncNoMatch);
				throw new Exception ();
			}
			/* TODO: From here are some checks that must be made in the function search&add zone */
		}

		static VariableTBase SingleFunctionCall (VariableTBase[] args, ConsoleAccess ds, MethodInfo mi)
		{
			VariableTBase vtb;
			ParameterInfo[] pis = mi.GetParameters ();
			/* Single parameter shell function - aka no user parameters */
			if (pis.Length == 1) {
				if (args.Length == 0) {
					try {
						vtb = (VariableTBase)mi.Invoke (null, new object[] { ds });
					} catch (NotSupportedException ex) {
						//throw new UserSpecifiedException (ex);
						throw new Exception ();
					}
					return vtb;
				} else
					throw new ShellArgumentMismatch (emptyArgFuncArgs);
			}

			int i;
			/* A function can have params arguments only if the last argument is an array. */
			bool hasparams = pis[pis.Length - 2].ParameterType.IsArray;
			MethodInfo mist;
			if (!hasparams & (args.Length > pis.Length - 1))
				throw new ShellArgumentMismatch (tooManyArgsNoParms);
			if (args.Length < pis.Length - 1)
				throw new ShellArgumentMismatch (tooFewArgs);

			mist = mi.IsGenericMethod ? CreateGenericMethod (args, mi, hasparams, pis) : mi;

			pis = mist.GetParameters ();
			object[] CallArgs = new object[pis.Length];
			for (i = 0; i < pis.Length - 2; i++) {
				if (pis[i].ParameterType.IsInstanceOfType (args[i]))
					CallArgs[i] = args[i];
				else
					throw new ShellArgumentMismatch (string.Format (argTypeMismatch, i, pis[i].ParameterType, args[i].GetType ()));
			}

			if (hasparams) {
				Type elmType = pis[pis.Length - 2].ParameterType.GetElementType ();
				Array r = Array.CreateInstance (elmType, args.Length + 2 - pis.Length);
				for (i = 0; i < r.Length; i++) {
					if (!args[i + pis.Length - 2].GetType ().IsAssignableFrom (elmType))
						throw new ShellArgumentMismatch (string.Format ("Params function not matching argument {0}", (i + pis.Length - 2)));
					r.SetValue (args[i + pis.Length - 2], i);
				}
				CallArgs[pis.Length - 2] = r;
			} else {
				if (pis[pis.Length - 2].ParameterType.IsInstanceOfType (args[pis.Length - 2]))
					CallArgs[i] = args[i];
				else
					throw new ShellArgumentMismatch (string.Format (argTypeMismatch, i, pis[i].ParameterType, args[pis.Length - 2]));
			}

			CallArgs[pis.Length - 1] = ds;

			try {
				vtb = (VariableTBase)mist.Invoke (null, CallArgs);
			} catch (TargetInvocationException ex) {
				System.Diagnostics.Debug.WriteLine (ex.InnerException);
				//throw new UserSpecifiedException (ex.InnerException);
				throw new Exception ();
			}
			return vtb;
		}

		/// <summary>
		/// Generates a method from a generic method definition.
		/// </summary>
		/// <returns>The method.</returns>
		/// <param name="args">Calling arguments.</param>
		/// <param name="mi">Generic Method Definition.</param>
		/// <param name="hasparams">Is a params function.</param>
		/// <param name="pis">The parameters accepted by the generic function.</param>
		static MethodInfo CreateGenericMethod (VariableTBase[] args, MethodInfo mi, bool hasparams, ParameterInfo[] pis)
		{
			List<Type> genme = new List<Type> ();
			List<int> argp = new List<int> ();
			MethodInfo mist;
			int i;
			for (i = 0; i < pis.Length; i++)
				if (pis[i].ParameterType.ContainsGenericParameters)
					argp.Add (i);
			for (i = 0; i < argp.Count; i++) {
				Type bt = args[argp[i]].GetType ().GetGenericArguments ()[0];
				if (hasparams) {
					if (argp[i] == pis.Length - 2) {
						int j;
						Type[] rtype = new Type[2 + args.Length - pis.Length];
						for (j = argp[i]; j < args.Length; j++) {
							rtype[j - pis.Length + 2] = args[j].GetType ().GetGenericArguments ()[0];
						}
						bt = TypeInference.GetCommonBaseClass (rtype);
					}
					bt = TypeInference.InferType (bt, pis[argp[i]].ParameterType.GetElementType (), mi.GetGenericArguments ());
				} else
					bt = TypeInference.InferType (bt, pis[argp[i]].ParameterType, mi.GetGenericArguments ());
				genme.Add (bt);
				/* TODO FIXME Bug on the gafe function: inspect */
			}
			try {
				mist = mi.MakeGenericMethod (genme.ToArray ());
			} catch {
				throw new ShellArgumentMismatch (genFuncArgsMismatch);
			}
			return mist;
		}
	}
}

