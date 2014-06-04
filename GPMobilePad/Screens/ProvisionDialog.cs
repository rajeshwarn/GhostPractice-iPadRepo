using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Net;
using System.IO;
using System.Text;
using System.Drawing;
using Newtonsoft.Json;
using System.Web;
using GhostPractice;
using GhostPracticeLibrary;

namespace GPMobilePad
{
	public partial class ProvisionDialog : DialogViewController
	{
		SplitController split;
		MatterDetail matterDetail;
		DateTime start, end;
		EntryElement activationCode;


		public ProvisionDialog (SplitController split) : base (UITableViewStyle.Grouped, null)
		{
			this.split = split;
			Autorotate = true;
			//TODO - remove after test...
			//RemoveProperties ();
			//SetManualProperties ();
			if (isDeviceProvisioned ()) {
				startMatterSearch ();
			} else {
				buildInterface ();
			}

		}

		bool isBusy;

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			Console.WriteLine ("---> ShouldAutorotateToInterfaceOrientation: " + toInterfaceOrientation.ToString ());
			return false;
		}

		private void buildInterface ()
		{
			Console.WriteLine ("ProvisionDialog  - building user interface");
			Root = new RootElement ("GhostPractice Device Activation");
			Section sec1 = new Section ();
			var welcome = new StyledStringElement ("Welcome to GhostPractice!", "");
			welcome.Alignment = UITextAlignment.Center;
			welcome.Font = UIFont.BoldSystemFontOfSize (30);
			welcome.TextColor = ColorHelper.GetGPPurple ();
			sec1.Add (welcome);
			//Root.Add (sec1);
			//
			var headerLabel = new UILabel (new RectangleF (10, 10, 700, 40)) {
				Font = UIFont.BoldSystemFontOfSize (20),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = "Welcome to GhostPractice Mobile!"
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 10, 700, 40); 
			view.Add (headerLabel);
			var topSection = new Section (view);

			Root.Add (topSection);	
			//
			for (var i = 0; i < 6; i++) {
				Root.Add (new Section ("  "));
			}


			var label = new Section ("Please enter the activation code that was given or sent to you by GhostPractice Support and send it to the office.");

			var sec2 = new Section ("    ");
			Root.Add (sec2);
			Root.Add (label);
			//

			var sec3 = new Section ();
			activationCode = new EntryElement ("Activation Code", "Enter Activation Code", String.Empty);
			sec3.Add (activationCode);
			btn = new StyledStringElement ("Send Activation Code") { Accessory = UITableViewCellAccessory.DetailDisclosureButton };
			btn.Alignment = UITextAlignment.Center;
			btn.BackgroundColor = ColorHelper.GetGPPurple ();
			btn.TextColor = UIColor.White;
			btn.Tapped += delegate() {
				if (!isBusy) {
					validate ();
				} else {
					Wait ();
				}
			};
			btn.AccessoryTapped += delegate {
				if (!isBusy) {
					validate ();
				} else {
					Wait ();
				}
			};
			sec4 = new Section (" ");
			sec4.Add (btn);
			Root.Add (sec3);
			Root.Add (sec4);
		}

		StyledStringElement btn;
		Section sec4;
		int counter;

		private void Wait ()
		{
			new UIAlertView ("Busy Activating", "Activating ... please wait for completion", null, "Close", null).Show ();
			counter++;
			if (counter > 1) {
				isBusy = false;
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			}

		}

		private void validate ()
		{

			if (activationCode.Value == "") {
				new UIAlertView ("Missing Activation Code", "Please enter the activation code and try again.", null, "Close", null).Show ();
				return;
			}
			sec4.Clear ();
			isBusy = true;
			getAsyncAppAndPlatform ();
		}

		private void startMatterSearch ()
		{
			Console.WriteLine ("ProvisionDialog - startMatterSearch - normal work - device provisioned");
//			splitViewController = new SplitController ();
//			var master = new MatterFinderController (splitViewController, null);
//			var detail = new MatterDetail (splitViewController, null, null, null);
//			splitViewController.setMatterControllers (master, detail);

			var finder = new Finder (matterDetail);

			matterDetail = new MatterDetail (split, finder);
			var detailNavigationController = new UINavigationController (matterDetail);
			var masterNavigationController = new UINavigationController (finder);

			split.WeakDelegate = matterDetail;
			split.ViewControllers = new UIViewController[] {
				masterNavigationController,
				detailNavigationController
			};

		}

		private bool isDeviceProvisioned ()
		{
			string deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
			if (deviceID == null) {
				return false;
			}
			return true;
		}

		public void getAsyncProvisionDevice ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			start = DateTime.Now;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
			
			cr.requestType = GhostRequestDTO.PROVISION_NEW_USER;
			cr.activationCode = activationCode.Value;
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.platformID = NSUserDefaults.StandardUserDefaults.IntForKey ("platformID");
			
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("Async JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;
			
			var request = (HttpWebRequest)WebRequest.Create (url);
			request.BeginGetResponse (ProvisioningDownloaded, request);
			
		}

		void ProvisioningDownloaded (IAsyncResult result)
		{
			Console.WriteLine ("## ProvisioningDownloaded - check response data ....\n");
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			end = DateTime.Now;
			isBusy = false;
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			try {
				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (result);
				Stream receiveStream = response.GetResponseStream ();
				StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
				string resp = readStream.ReadToEnd ();
				Console.WriteLine ("## ASYNCResponse stream received.\n" + resp);
				response.Close ();
				readStream.Close ();
				//
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (resp, typeof(WebServiceResponseDTO));
				
				if (dto != null) {
					Tools.SendElapsedTime (start, end, dto.activityID);
				}	
				
				InvokeOnMainThread (delegate {
					if (dto.responseCode == 0) {
						Console.WriteLine ("####### Device successfully activated. Saving user data");
						if (dto.user.cellphone != null) {
							NSUserDefaults.StandardUserDefaults.SetString (dto.user.cellphone, "cellphone");
						}
						NSUserDefaults.StandardUserDefaults.SetInt (dto.user.company.companyID, "companyID");
						NSUserDefaults.StandardUserDefaults.SetInt (dto.user.userID, "userID");
						if (dto.user.userName != null) {
							NSUserDefaults.StandardUserDefaults.SetString (dto.user.userName, "userName");
						}
						if (dto.user.email != null) {
							NSUserDefaults.StandardUserDefaults.SetString (dto.user.email, "email");
						}
						if (dto.deviceID != null) {
							NSUserDefaults.StandardUserDefaults.SetString (dto.deviceID, "deviceID");
						}
						if (dto.user.company.companyName != null) {
							NSUserDefaults.StandardUserDefaults.SetString (dto.user.company.companyName, "companyName");
						}

						startMatterSearch ();
					} else {

						isBusy = false;
						new UIAlertView (
							"Device Provisioning",
							dto.responseMessage + "\n\nPlease contact GhostPractice Support for assistance. \nStatuc code: " + dto.responseCode, null, "OK").Show ();
						buildInterface ();
					}
					
				}
				);
				
				
				
			} catch (Exception ex) {
				Console.WriteLine ("### ERROR: " + ex.Message);
				InvokeOnMainThread (delegate {
					buildInterface ();
					new UIAlertView (
						"Network Error",
						"Problem communicating with server.\nPlease try again or call GhostPractice Support",
						null,
						"Close"
					).Show ();
					isBusy = false;
				}
				);
			}
		}
		//
		// Asynchronous HTTP request
		//
		public void getAsyncAppAndPlatform ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
			
			cr.requestType = GhostRequestDTO.GET_APP_PLATFORM_IDS;
			cr.appName = "GhostPractice Mobile";
			cr.platformName = "iPad";
			
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("Async JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;
			
			var request = (HttpWebRequest)WebRequest.Create (url);
			request.BeginGetResponse (PlatformDownloaded, request);
			
		}

		void PlatformDownloaded (IAsyncResult result)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			
			try {
				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (result);
				Stream receiveStream = response.GetResponseStream ();
				StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
				string resp = readStream.ReadToEnd ();
				Console.WriteLine ("## ASYNCResponse stream received.\n" + resp);
				response.Close ();
				readStream.Close ();
				//get JSON response deserialized
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (resp, typeof(WebServiceResponseDTO));
				
				InvokeOnMainThread (delegate {
					if (dto.responseCode == 0) {
						//save app and platform in properties
						NSUserDefaults.StandardUserDefaults.SetInt (dto.app.appID, "appID");
						NSUserDefaults.StandardUserDefaults.SetString (dto.app.appName, "appName");
						NSUserDefaults.StandardUserDefaults.SetInt (dto.platform.platformID, "platformID");
						NSUserDefaults.StandardUserDefaults.SetString (dto.platform.platformName, "platformName");
						
						getAsyncProvisionDevice ();
					} else {
						buildInterface ();
						isBusy = false;
						new UIAlertView ("Platform Service", dto.responseMessage, null, "OK").Show ();
					}
				}
				);
				
			} catch (Exception ex) {
				Console.WriteLine ("### ERROR: " + ex.Message);
				InvokeOnMainThread (delegate {
					new UIAlertView (
						"Network Error",
						"Problem communicating with server.\nPlease try later or call GhostPractice Support",
						null,
						"Close"
					).Show ();
				}
				);
			}
		}
		//TODO remove after testing
		private void SetManualProperties ()
		{
			//save app and platform in properties
			NSUserDefaults.StandardUserDefaults.SetInt (1, "appID");
			NSUserDefaults.StandardUserDefaults.SetString ("GhostPractice Mobile", "appName");
			NSUserDefaults.StandardUserDefaults.SetInt (2, "platformID");
			NSUserDefaults.StandardUserDefaults.SetString ("iPhone", "platformName");
			//save user in properties
			NSUserDefaults.StandardUserDefaults.SetString ("0828013722", "cellphone");
			NSUserDefaults.StandardUserDefaults.SetInt (17, "companyID");
			NSUserDefaults.StandardUserDefaults.SetInt (336, "userID");
			NSUserDefaults.StandardUserDefaults.SetString ("Aubrey V Malabie", "userName");
			NSUserDefaults.StandardUserDefaults.SetString ("malengatiger@gmail.com", "email");
			NSUserDefaults.StandardUserDefaults.SetString ("e5579a77-cd69-4f0f-a6a2-c9db1c05b98e", "deviceID");	
			Console.WriteLine ("ProvisionDialog  - manual test properties set up");
		}
		//TODO - remove after testing
		private void RemoveProperties ()
		{
			//remove app and platform in properties
			try {
				NSUserDefaults.StandardUserDefaults.RemoveObject ("cellphone");
				NSUserDefaults.StandardUserDefaults.RemoveObject ("companyID");
				NSUserDefaults.StandardUserDefaults.RemoveObject ("userID");
				NSUserDefaults.StandardUserDefaults.RemoveObject ("userName");
				NSUserDefaults.StandardUserDefaults.RemoveObject ("email");
				NSUserDefaults.StandardUserDefaults.RemoveObject ("deviceID");
				Console.WriteLine ("ProvisionDialog  - properties removed");
			} catch (Exception e) {
				Console.WriteLine (e.Data.ToString ());
				//ignore - properties dont exist	
			}
		}

		[Export ("splitViewController:willHideViewController:withBarButtonItem:forPopoverController:")]
		public void WillHideViewController (UISplitViewController splitController, UIViewController viewController, UIBarButtonItem barButtonItem, UIPopoverController popoverController)
		{
			Console.WriteLine ("WillHideViewController -----> setting button to GhostPractice");
			barButtonItem.Title = "GhostPractice";
			NavigationItem.SetLeftBarButtonItem (barButtonItem, true);
			masterPopoverController = popoverController;
		}

		[Export ("splitViewController:willShowViewController:invalidatingBarButtonItem:")]
		public void WillShowViewController (UISplitViewController svc, UIViewController vc, UIBarButtonItem button)
		{
			Console.WriteLine ("WillShowViewController -----> killing popover...");
			// Called when the view is shown again in the split view, invalidating the button and popover controller.
			NavigationItem.SetLeftBarButtonItem (null, true);
			masterPopoverController = null;
		}

		public override bool ShouldAutorotate ()
		{
			return false;
		}

		UIPopoverController masterPopoverController;
	}
}
