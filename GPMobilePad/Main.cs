using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GPMobilePad
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			Console.WriteLine ("Main.cs --- about to kick application off!...problem may be here");
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
