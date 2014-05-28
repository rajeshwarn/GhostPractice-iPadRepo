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

namespace GhostPractice
{
	public partial class ProvisioningDialog : DialogViewController
	{
		DateTime start, end;
		EntryElement activation;
		bool isBusy;

		public ProvisioningDialog () : base (UITableViewStyle.Grouped, null)
		{

			Root = new RootElement ("GhostPractice Mobile");
			var topSec = new Section ("Welcome");
			topSec.Add (new StringElement ("Please enter activation code"));
			activation = new EntryElement ("Code", "Activation Code", null); 
			topSec.Add (activation);
			var submit = new StringElement ("Send Code");
			submit.Alignment = UITextAlignment.Center;
		

			submit.Tapped += delegate {
				if (activation.Value == null || activation.Value == string.Empty) {
					new UIAlertView ("Device Activation", "Please enter activation code", null, "OK", null).Show ();
					return;
				}
				if (!isBusy) {
					getAsyncAppAndPlatform ();
				} else {
					Wait ();
				}

			};
			topSec.Add (submit);
			Root.Add (topSec);

			UIImage img = UIImage.FromFile ("Images/launch_small.png");
			UIImageView v = new UIImageView (new RectangleF (0, 0, 480, 600));
			v.Image = img;
			Root.Add (new Section (v));

		}

		int counter;

		private void Wait ()
		{
			new UIAlertView ("Busy Activating", "Activating ... please wait for completion", null, "Close", null).Show ();
			counter++;
			if (counter > 0) {
				isBusy = false;
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			}

		}
		//
		// Asynchronous HTTP request
		//
		public void getAsyncProvisionDevice ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			isBusy = true;
			start = DateTime.Now;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;

			cr.requestType = GhostRequestDTO.PROVISION_NEW_USER;
			cr.activationCode = activation.Value;
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
						//save user in properties
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
						var cont = new MatterFindController ();
						this.NavigationController.PushViewController (cont, true);
					} else {
						new UIAlertView (
							"Device Provisioning",
							dto.responseMessage + "\n\nPlease contact GhostPractice Support for assistance.", null, "OK").Show ();
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
			isBusy = true;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;

			cr.requestType = GhostRequestDTO.GET_APP_PLATFORM_IDS;
			cr.appName = "GhostPractice Mobile";
			cr.platformName = "iPhone";

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
					isBusy = false;
				}
				);
			}
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			//return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			return false;
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
			} catch (Exception e) {
				//ignore - properties dont exist	
			}
		}
	}
}

