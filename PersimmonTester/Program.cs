using System;

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
	}
}
