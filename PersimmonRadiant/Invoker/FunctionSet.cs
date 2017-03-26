using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PersimmonRadiant
{
	public class FunctionSet
	{
		internal Dictionary<string, List<Invokable>> functions;
		private static Type funcp = typeof (ConsoleFunctionsProvider);
		private static Type funcm = typeof (ConsoleFunctionAttribute);

		/// <summary>
		/// Lists all registered functions
		/// </summary>
		/// <value>A list with descriptors of registered functions.</value>
		public List<ConsoleFunctionDescription> ListFunctions {
			get {
				List<ConsoleFunctionDescription> cfs = new List<ConsoleFunctionDescription> (functions.Count);
				foreach (KeyValuePair<string, List<Invokable>> kvp in functions)
					cfs.AddRange (kvp.Value.Select ((x) => x.Descriptor));
				return cfs;
			}
		}

		public FunctionSet ()
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
					 let d = ConsoleFunctionAttribute.GetDescriptor (f)
					 select new KeyValuePair<string, Invokable> (d.Name, new Invokable (f, d)));
			lock(this) {
				if (functions == null) functions = new Dictionary<string, List<Invokable>> ();
				lock(functions) {
					foreach (var v in w)
						if (functions.ContainsKey (v.Key)) functions[v.Key].Add (v.Value); else functions.Add (v.Key, new List<Invokable> () { v.Value });
				}
			}
		}
	}
}

