using System;
using System.Collections.Generic;
using System.Reflection;

namespace PersimmonRadiant
{
	public class InvokerSet : FunctionSet
	{
		ConsoleAccess access;

		public InvokerSet ()
		{
		}

		public VariableTBase Invoke (string name, VariableTBase[] args)
		{
			List<Invokable> invs;
			lock(functions) {
				if (!functions.ContainsKey (name)) {
					access.WriteText (ConsoleErrorMessages.FunctionNotFound);
					throw new Exceptions.ShellFunctionNotFound ();
				} else
					invs = functions[name];
			}
			foreach (Invokable ivb in invs) {
				ArgResult rs = CheckPopulateArgs (args, ivb);
				if (!rs.IsCorrect) continue;
				MethodInfo minf = ivb.CallStd.generic ? rs.genCr : ivb.MInfo;
				object o = minf.Invoke (null, rs.arguments);
				VariableTBase vb = (VariableTBase)o;
				return vb;
			}
			return null;
		}

		private struct ArgResult
		{
			public bool IsCorrect;
			public Exceptions.ShellException ex;
			public List<Tuple<int, object>> Extracted;
			public object[] arguments;
			public MethodInfo genCr;
		}

		private void FindCallStd (Invokable iv)
		{
			var Arguments = iv.MInfo.GetParameters ();
			Parametrization p = new Parametrization ();
			int usesCFunc;

			/* Check if the function lacks any arguments */
			if (Arguments.Length == 0)
				p.cfunc = false;
			/* Check whether it uses the ConsoleAccess structure and also check it has at least as many arguments as the function inputs */
			else p.cfunc = Arguments[Arguments.Length - 1].ParameterType.IsAssignableFrom (typeof (ConsoleAccess));
			usesCFunc = p.cfunc ? 1 : 0;


			if (Arguments.Length == usesCFunc) {
				p.hp1 = false; p.hp2 = false;
			} else {
				/* Check if it uses a variable number of arguments */
				p.hp1 = Arguments[Arguments.Length - 1 - usesCFunc].ParameterType.IsArray;
				bool hp2 = Arguments[Arguments.Length - 1 - usesCFunc].ParameterType.IsGenericType;
				if (hp2 == true) hp2 = Arguments[Arguments.Length - 1 - usesCFunc].ParameterType.GetGenericTypeDefinition () == typeof (Variable<>);
				if (hp2 == true) { var h = Arguments[Arguments.Length - 1 - usesCFunc].ParameterType.GetGenericArguments (); hp2 = (h.Length == 1 & h[0].IsArray); }
				p.hp2 = hp2;
			}

			p.generic = iv.MInfo.IsGenericMethodDefinition;

			iv.CallStd = p;
		}


		private ArgResult CheckPopulateArgs (VariableTBase[] args, Invokable inv)
		{
			if (inv.CallStd == null) FindCallStd (inv);
			var Arguments = inv.MInfo.GetParameters ();
			int usesCFunc;
			/* Check whether it uses the ConsoleAccess structure and also check it has at least as many arguments as the function inputs */
			if (inv.CallStd.cfunc) {
				if (Arguments.Length < args.Length + 1) return new ArgResult () { IsCorrect = false, ex = new Exceptions.ShellArgumentMismatch (ConsoleErrorMessages.TooFewArguments) };
				else usesCFunc = 1;
			} else {
				if (Arguments.Length < args.Length) return new ArgResult () { IsCorrect = false, ex = new Exceptions.ShellArgumentMismatch (ConsoleErrorMessages.TooFewArguments) };
				else usesCFunc = 0;
			}

			/* Get rid of the case where there are no function inputs */
			if (Arguments.Length == usesCFunc) {
				if (args.Length != 0) return new ArgResult () { IsCorrect = false, ex = new Exceptions.ShellArgumentMismatch (ConsoleErrorMessages.NonemptyArguments) };
				else return new ArgResult () { IsCorrect = true, arguments = new object[0] };
			}

			/* Check if it uses a variable number of arguments */
			int hasparams = inv.CallStd.hp1 | inv.CallStd.hp2 ? 1 : 0;
			if (hasparams == 0 & (args.Length + usesCFunc > Arguments.Length))
				return new ArgResult () { IsCorrect = false, ex = new Exceptions.ShellArgumentMismatch (ConsoleErrorMessages.TooManyArgsNotVarargs) };

			/* Prepare ArgResult */
			ArgResult lg = new ArgResult ();
			lg.Extracted = new List<Tuple<int, object>> ();
			lg.arguments = new object[Arguments.Length];

			if (inv.CallStd.generic) {
				lg.genCr = CreateGenericMethod (args, inv);
				Arguments = lg.genCr.GetParameters ();
			}

			int i;
			/* Check if the arguments match the function */
			for (i = 0; i + hasparams + usesCFunc < Arguments.Length; i++) {
				if (!Arguments[i].ParameterType.IsInstanceOfType (args[i])) {
					var z = GetVar (args[i], Arguments[i].ParameterType);
					if (z == null) return new ArgResult () { IsCorrect = false, ex = new Exceptions.ShellArgumentMismatch (ConsoleErrorMessages.ArgumentTypeMismatch) };
					lg.Extracted.Add (new Tuple<int, object> (i, z.Item2));
					lg.arguments[i] = z.Item2;
				} else lg.arguments[i] = args[i];
			}

			/* Check for the 2 possible params cases */
			if (inv.CallStd.hp1) {
				List<VariableTBase> vb = new List<VariableTBase> ();
				for (; i + usesCFunc < Arguments.Length; i++) {
					vb.Add (args[i]);
				}
				lg.arguments[Arguments.Length - 1 - usesCFunc] = vb.ToArray ();
			}
			if (inv.CallStd.hp2) {
				List<object> sobj = new List<object> ();
				for (; i + usesCFunc < Arguments.Length; i++) {
					object[] o2;
					var z = GetVar (args[i], Arguments[i].ParameterType);
					if (z != null) { sobj.Add (z.Item2); } else {
						z = GetVar (args[i], Arguments[i].ParameterType.GetElementType ());
						if (z == null) return new ArgResult () { IsCorrect = false, ex = new Exceptions.ShellArgumentMismatch (ConsoleErrorMessages.ArgumentTypeMismatch) };
						o2 = (object[])z.Item2; sobj.AddRange (o2);
					}
					lg.Extracted.Add (new Tuple<int, object> (i, z.Item2));
				}
				lg.arguments[Arguments.Length - 1 - usesCFunc] = sobj.ToArray ();
			}
			lg.IsCorrect = true;
			return lg;


		}

		Tuple<Type, object> GetVar (VariableTBase vb, Type tp)
		{
			Type tc = tp;
			while (tc != null && tc != typeof (object)) {
				var cur = tc.IsGenericType ? tc.GetGenericTypeDefinition () : tc;
				if (typeof (Variable<>) == cur) {
					return new Tuple<Type, object> (tc.GetGenericArguments ()[0], tc.GetField ("obj").GetValue (vb));
				}
				tc = tc.BaseType;
			}
			return null;
		}

		static MethodInfo CreateGenericMethod (VariableTBase[] args, Invokable iv)
		{
			var pseinfo = iv.MInfo.GetParameters ();
			int mlength = pseinfo.Length;
			if (iv.CallStd.cfunc) mlength--;
			if (iv.CallStd.hp1) mlength--;
			if (iv.CallStd.hp2) mlength--;
			Type[] stype = iv.MInfo.GetGenericArguments ();
			List<Tuple<Type, Type>> typerelist = new List<Tuple<Type, Type>> ();
			int i;
			for (i = 0; i < mlength; i++) {
				ParameterInfo pinf = pseinfo[i];
				bool isGen = pinf.ParameterType.IsGenericParameter;
				bool hasGen = pinf.ParameterType.ContainsGenericParameters;
				if ((!isGen) & (!hasGen)) continue;
				if (isGen) typerelist.Add (new Tuple<Type, Type> (args[i].GetType ().GetGenericArguments()[0], stype[pinf.ParameterType.GenericParameterPosition]));
				if (hasGen) {
					List<Tuple<Type, Type>> ntp = TypeInference.ReplacementTypes (args[i].GetType (), pinf.ParameterType);
					typerelist.AddRange (ntp);
				}
			}
			if (iv.CallStd.hp1 | iv.CallStd.hp2) {
				if (pseinfo[mlength].ParameterType.ContainsGenericParameters) {
					Type vt = args[mlength].GetType ().GetGenericArguments ()[0];
					for (i = mlength + 1; i < args.Length; i++) {
						Type rt = args[i].GetType ().GetGenericArguments ()[0];
						vt = TypeInference.GetCommonBaseClass (vt, rt);
					}
					typerelist.Add (new Tuple<Type, Type> (vt, pseinfo[mlength].ParameterType));
				}
			}
			Type[] relist = new Type[stype.Length];
			for (i = 0; i < stype.Length; i++)
				relist[i] = typerelist.Find ((Tuple<Type, Type> obj) => obj.Item2 == stype[i]).Item1;

			MethodInfo minf = iv.MInfo.MakeGenericMethod (relist);

			return minf;
		}


	}
}

