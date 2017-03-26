using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace PersimmonRadiant
{
	/// <summary>
	/// An attribute applied to all functions that can be called from the console.
	/// Used by reflection to detect which functions are to be accessible from the console.
	/// </summary>
	[AttributeUsage (AttributeTargets.Method)]
	[Serializable]
	public class ConsoleFunctionAttribute : Attribute
	{
		public readonly ConsoleFunctionDescription descriptor;
		static List<ConsoleFunctionDescription> descriptors;
		static List<string> helpPaths;

		public static ConsoleFunctionDescription GetDescriptor (MethodInfo mi)
		{
			if (descriptors == null) PopulateDescriptors ();
			List<ConsoleFunctionDescription> z1 = descriptors.Where ((x) => x.OriginalName == mi.Name).ToList ();
			var zw = z1.Where ((x) => x.Arguments.Length + 1 >= mi.GetParameters ().Length).ToList ();
			var zp = zw.Where ((x) => x.Arguments.Length <= mi.GetParameters ().Length).ToList ();

			ParameterInfo[] z2 = mi.GetParameters ();
			foreach (ParameterInfo pi in z2) {
				if (pi.ParameterType == typeof (ConsoleAccess)) continue;
				if ((pi.ParameterType == typeof (VariableTBase)) | (pi.ParameterType == typeof (VariableTBase).MakeArrayType ())) {
					zp = zp.Where ((x) => x.Arguments[pi.Position].TypeName == "?").ToList ();
					continue;
				} else
					zp = zp.Where ((x) => string.Equals (x.Arguments[pi.Position].TypeName, pi.ParameterType.GetGenericArguments ()[0].Name, StringComparison.OrdinalIgnoreCase)).ToList ();
			}
			if (zp.Count != 1) throw new Exception (); /* TODO: Implement a well-explained exception */
			return zp[0];
		}

		static void PopulateDescriptors ()
		{
			helpPaths = Directory.GetFiles ("Doc\\").Where ((x) => x.EndsWith (".xml")).ToList ();
			descriptors = new List<ConsoleFunctionDescription> ();
			foreach (string path in helpPaths) {
				string Description = File.ReadAllText (path);
				XDocument xd = XDocument.Parse (Description);
				var funcs = xd.Descendants ("func");
				foreach (XElement func in funcs) {
					ConsoleFunctionDescription descr;
					XElement namee = func.Element ("name");
					string nm = namee.Value;
					string onm = func.Attribute ("id").Value;
					if (onm.IndexOf ('~') != -1) onm = onm.Substring (0, onm.IndexOf ('~'));
					XElement descee = func.Element ("shortDescription");
					string desc = descee.Value;
					XElement htext = func.Element ("helpText");
					string help = htext.Value;

					XElement args = func.Element ("arguments");
					List<ParamInfo> argpi = new List<ParamInfo> ();
					var d = args.Elements ();
					foreach (XElement arg in d) {
						XAttribute xnr = arg.Attribute ("nr");
						XAttribute xtype = arg.Attribute ("type");
						XAttribute parms = arg.Attribute ("params");
						string argdesc = arg.Value;
						ParamInfo pip = new ParamInfo (Convert.ToInt32 (xnr.Value), argdesc, xtype.Value, Convert.ToBoolean (parms.Value));
						argpi.Add (pip);
					}
					XElement argen = func.Element ("returns");
					XAttribute xtypeen = argen.Attribute ("type");
					string argdescen = argen.Value;
					ParamInfo pipen = new ParamInfo (0, argdescen, xtypeen.Value, false);
					descr = new ConsoleFunctionDescription (nm, onm, desc, help, argpi.ToArray (), pipen);
					descriptors.Add (descr);
				}
			}
		}
	}

	/// <summary>
	/// Marks a class as having console functions.
	/// </summary>
	[AttributeUsage (AttributeTargets.Class)]
	[Serializable]
	public class ConsoleFunctionsProvider : Attribute
	{

	}

	/// <summary>
	/// Marks a (static) field that is an algorithmic constant.
	/// </summary>
	[AttributeUsage (AttributeTargets.Field)]
	[Serializable]
	public class AlgorithmicConstantAttribute : Attribute
	{
		/* TODO: Implement the ACA so the ACs will be accessible from the console. */
	}

}

