﻿using System;

namespace PersimmonTester
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			//PersimmonRadiant.Variable<int> vb;
			PersimmonRadiant.ConsoleInstance inst = new PersimmonRadiant.ConsoleInstance ();
			inst.LoadFunctions (System.Reflection.Assembly.GetExecutingAssembly ());
			string ln = Console.ReadLine ();
			inst.InterpretLine (ln);
			Console.ReadKey ();
		}
	}

	[PersimmonRadiant.ConsoleFunctionsProvider]
	class TesterFunctions
	{
		
		[PersimmonRadiant.ConsoleFunction]
		public static void RunOne ()
		{
			Console.WriteLine ("Z1");
		}

		[PersimmonRadiant.ConsoleFunction]
		public static void RunTwo ()
		{
			Console.WriteLine ("Z2");
		}

		[PersimmonRadiant.ConsoleFunction]
		public static void Rw<T> (PersimmonRadiant.Variable<T>[] obj)
		{
			T er = default (T);
			if (er is int) {
				int q = 0;
				foreach (var z in obj) {
					q += (int)((object)z.obj);
				}
				er = (T)((object)q);
			}
			if (er is double) {
				er = obj[0].obj;
				Console.WriteLine (er);
				Console.WriteLine (((double)(object)er) + 1.5);
			}
			Console.WriteLine (er.ToString ());
		}
	}
}
