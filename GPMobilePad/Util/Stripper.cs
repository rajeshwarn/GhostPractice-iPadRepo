using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace GPMobilePad
{
	public class Stripper
	{
		static string output;

		public static string StripByRegionFormat (string source)
		{
			Console.WriteLine ("CurrentUICulture: {0}", Thread.CurrentThread.CurrentUICulture);
			Console.WriteLine ("CurrentCulture: {0}", Thread.CurrentThread.CurrentCulture);
			Console.WriteLine ("Stripper: source string: " + source);
			var culture = Thread.CurrentThread.CurrentCulture;

			if (culture.ToString () == "en-ZA") {
				output = Regex.Replace (source, ",", ".");
				Regex reg = new Regex (@"\s");
				string[] bits = reg.Split (output);
				Console.WriteLine ("Stripper: en-ZA result string: " + String.Join ("", bits));
				return String.Join ("", bits);
			}
			if (culture.ToString () == "en-CA") {
				output = Regex.Replace (source, ",", "");
				Console.WriteLine ("Stripper: en-CA result string: " + output);
				return output;
			}
			if (culture.ToString () == "en-US") {
				output = Regex.Replace (source, ",", "");
				Console.WriteLine ("Stripper: en-US result string: " + output);
				return output;
			}
			if (culture.ToString () == "en-GB") {
				output = Regex.Replace (source, ",", "");
				Console.WriteLine ("Stripper: en-GB result string: " + output);
				return output;
			}


			return Regex.Replace (source, ",", "");
			;
		}
	}
}

