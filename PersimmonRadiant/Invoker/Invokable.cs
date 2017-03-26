using System;
using System.Reflection;

namespace PersimmonRadiant
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

	internal class Parametrization
	{
		public bool hp1, hp2, cfunc, generic;
	}
}

