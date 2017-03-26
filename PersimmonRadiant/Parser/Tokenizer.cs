using System;
using System.Collections.Generic;
using System.Text;

namespace PersimmonRadiant
{
	static class Tokenizer
	{
		static char[] tokensplitter = new char[] { ' ', '"', '\'', '\\', ';' };

		public static string[][] Tokenize (string fullstring)
		{
			int i, pi, qinhi;
			bool quoted, q1, q2;
			StringBuilder sb = new StringBuilder ();
			List<string> strset = new List<string> ();
			List<string[]> commset = new List<string[]> ();
			pi = 0;
			quoted = false;
			q1 = false;
			q2 = false;
			qinhi = -5;
			for (i = 0; i < fullstring.Length; i++) {
				i = fullstring.IndexOfAny (tokensplitter, pi);
				if (i == -1) {
					sb.Append (fullstring.Substring (pi));
					strset.Add (sb.ToString ());
					sb.Clear ();
					pi = -1;
					break;
				}
				quoted = q1 | q2;
				sb.Append (fullstring.Substring (pi, i - pi));
				if ((fullstring[i] == ' ') && !quoted) {
					if (sb.Length != 0) {
						strset.Add (sb.ToString ());
						sb.Clear ();
					}
				}
				/* Parse quoted strings */
				if (fullstring[i] == '"')
					if (i - qinhi > 1)
						sb.Append (fullstring[i]);
					else
						q1 = !q1;
				if (fullstring[i] == '\'')
					if (i - qinhi > 1)
						sb.Append (fullstring[i]);
					else
						q2 = !q2;
				/* Deal with escaped quote symbol */
				if (fullstring[i] == '\\') {
					if (quoted)
						sb.Append (fullstring[i]);
					else {
						qinhi = i;
						sb.Append (fullstring[i]);
					}
				}
				/* New command */
				if (fullstring[i] == ';' && !(quoted | qinhi == i)) {
					if (sb.Length != 0) {
						strset.Add (sb.ToString ());
						sb.Clear ();
					}
					if (strset.Count != 0) {
						commset.Add (strset.ToArray ());
						strset = new List<string> ();
					}
				}
				pi = i + 1;
			}
			if (i != pi)
				sb.Append (fullstring.Substring (pi, i - pi));
			if (sb.Length != 0)
				strset.Add (sb.ToString ());
			if (strset.Count != 0) {
				commset.Add (strset.ToArray ());
			}
			return commset.ToArray ();
		}
	}

}

