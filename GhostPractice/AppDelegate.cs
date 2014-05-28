using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GhostPractice
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		UINavigationController navigationController;
		//		CLLocationManager PhoneLocationManager = null;
		//		LocationDelegate locationDelegate = null;
		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			//#######################################################
			//TODO - remove and test the real new user auth mechanism
			//RemoveProperties ();
			//SetManualProperties ();


			var provDialog = new ProvisioningDialog ();
			var matterDialog = new Finder ();

			if (isDeviceProvisioned ()) {
				navigationController = new UINavigationController (matterDialog);
			} else {
				navigationController = new UINavigationController (provDialog);
			}
			

			navigationController.Toolbar.TintColor = ColorHelper.GetGPPurple ();
			window.RootViewController = navigationController;
			
			//---- initialize our location manager and callback handler
//			this.PhoneLocationManager = new CLLocationManager ();
//			this.locationDelegate = new LocationDelegate (this);
//			this.PhoneLocationManager.Delegate = this.locationDelegate;
//			
//			//---- start updating our location, et. al.
//			this.PhoneLocationManager.StartUpdatingLocation ();
//			//this._iPhoneLocationManager.StartUpdatingHeading ();
//			
			
			// make the window visible
			window.MakeKeyAndVisible ();
			
			return true;
		}

		private bool isDeviceProvisioned ()
		{
			string deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
			if (deviceID == null) {
				return false;
			}
			return true;
		}
		//TODO remove after testing
		private void SetManualProperties ()
		{
			//save app and platform in properties
			NSUserDefaults.StandardUserDefaults.SetInt (1, "appID");
			NSUserDefaults.StandardUserDefaults.SetString ("GhostPractice Mobile", "appName");
			NSUserDefaults.StandardUserDefaults.SetInt (1, "platformID");
			NSUserDefaults.StandardUserDefaults.SetString ("iPhone", "platformName");
			//save user in properties
			NSUserDefaults.StandardUserDefaults.SetString ("0828013722", "cellphone");
			NSUserDefaults.StandardUserDefaults.SetInt (1, "companyID"); //17
			NSUserDefaults.StandardUserDefaults.SetInt (298, "userID"); //336
			NSUserDefaults.StandardUserDefaults.SetString ("Aubrey V Malabie", "userName");
			NSUserDefaults.StandardUserDefaults.SetString ("malengatiger@gmail.com", "email");
			NSUserDefaults.StandardUserDefaults.SetString ("a9c79555-4604-4a0b-bb5c-dbc745c000bd", "deviceID");	//e5579a77-cd69-4f0f-a6a2-c9db1c05b98e
		}
	}
}

