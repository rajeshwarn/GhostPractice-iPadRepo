using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CarameliPad
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		Splitter splitViewController;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			var controller = new RootViewController ();
			var navigationController = new UINavigationController (controller);
			var detailViewController = new DetailViewController ();
			
			splitViewController = new Splitter (navigationController, detailViewController);
			splitViewController.WeakDelegate = detailViewController;
//			splitViewController.ViewControllers = new UIViewController[] {
//				navigationController,
//				detailViewController
//			};
			
			window.RootViewController = splitViewController;

			// make the window visible
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

