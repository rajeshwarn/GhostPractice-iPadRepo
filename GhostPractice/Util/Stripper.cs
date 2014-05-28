using System;
using System.Text.RegularExpressions;
using System.Threading;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;
using System.Globalization;

namespace GhostPractice
{
	public class Stripper
	{
		static string output;

		public static void SetReportHeader (RootElement root, string title, string subTitle, float contentWidth)
		{


			Console.WriteLine ("Stripper.....SetReportHeader....frame width: " + contentWidth);

			var headerLabel = new UILabel (new RectangleF (0, 41, contentWidth, 40)) {
				Font = UIFont.BoldSystemFontOfSize (20),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = title
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (0, 41, contentWidth, 40);
			view.Add (headerLabel);
			var sec = new Section (view);
			root.Add (sec);

			if (subTitle == null || subTitle == string.Empty) {
				return;
			}
			var st = new StringElement (subTitle);
			st.Alignment = UITextAlignment.Center;
			var sec2 = new Section ("");
			sec2.Add (st);
			root.Add (sec2);

		}

		public static void BuildPageControl (UIPageControl pageControl)
		{
			pageControl = new UIPageControl (new RectangleF (0, 
				0, 320, 20));
			pageControl.BackgroundColor = UIColor.Red;
			pageControl.Alpha = 0.7f;

		}

		public static NumberFormatInfo GetNumberFormatInfo ()
		{
			Console.WriteLine ("CurrentCulture: {0}", Thread.CurrentThread.CurrentCulture);
			var culture = Thread.CurrentThread.CurrentCulture;

			if (culture.ToString () == "en-ZA") {
				return  new CultureInfo ("en-ZA", false).NumberFormat;
			}
			if (culture.ToString () == "en-CA") {
				return  new CultureInfo ("en-CA", false).NumberFormat;
			}
			if (culture.ToString () == "en-US") {
				return  new CultureInfo ("en-US", false).NumberFormat;
			}
			if (culture.ToString () == "en-GB") {
				return  new CultureInfo ("en-GB", false).NumberFormat;
			}

			return  new CultureInfo ("en-ZA", false).NumberFormat;
		}

		public static string StripByRegionFormat (string source)
		{
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

