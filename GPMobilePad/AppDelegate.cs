using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GPMobilePad
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		SplitController splitViewController;
		UIWindow window;
		ProvisionDialog provisionDialog;
		MatterDetail matterDetail;
		Finder finder;
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
			splitViewController = new SplitController ();
			window.RootViewController = splitViewController;

			if (isDeviceProvisioned ()) {
				startMatterMain ();
			} else {
				setUpActivation ();
			}

			window.RootViewController = splitViewController;
			window.MakeKeyAndVisible ();
			
			return true;
		}

		private void setUpActivation ()
		{
			Console.WriteLine ("AppDelegate - setUpActivation of new device");

			var dummy = new LaunchSplash ();
			var masterNavigationController = new UINavigationController (dummy);

			provisionDialog = new ProvisionDialog (splitViewController);
			var detailNavigationController = new UINavigationController (provisionDialog);
			
			splitViewController.WeakDelegate = provisionDialog;
			splitViewController.ViewControllers = new UIViewController[] {
				masterNavigationController,
				detailNavigationController
			};

		}

		private void startMatterMain ()
		{
			Console.WriteLine ("AppDelegate - startMatterMain - normal work");

			matterDetail = new MatterDetail (splitViewController, finder);
			var detailNavigationController = new UINavigationController (matterDetail);

			finder = new Finder (matterDetail);
			var masterNavigationController = new UINavigationController (finder);

			splitViewController.WeakDelegate = matterDetail;
			splitViewController.ViewControllers = new UIViewController[] {
				masterNavigationController,
				detailNavigationController
			};

		}

		private bool isDeviceProvisioned ()
		{
			var model = UIDevice.CurrentDevice.Model.ToString ();
			var name = UIDevice.CurrentDevice.Name.ToString ();
			Console.WriteLine ("AppDelegate - name: " + name + " model: " + model);
			string deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
			if (deviceID == null) {
				return false;
			}
			Console.WriteLine ("AppDelegate - Device already activated, deviceID = " + deviceID);
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations (UIApplication application, UIWindow forWindow)
		{
			//Console.WriteLine ("AppDelegate: GetSupportedInterfaceOrientations returns: " + UIInterfaceOrientationMask.Landscape);
			return UIInterfaceOrientationMask.Landscape;
		}
	}
}

