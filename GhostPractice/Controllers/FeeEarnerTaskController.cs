using System;
using System.Net;
using System.IO;
using System.Text;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using System.Globalization;

namespace GhostPractice
{
	public partial class FeeEarnerTaskController : UIViewController
	{
		MatterSearchResultDTO matter;
		MobileUser mobileUser;
		DateTime start, end;
		UINavigationController navController;

		public FeeEarnerTaskController (MatterSearchResultDTO matter, MobileUser user) : base ("FeeEarnerTaskController", null)
		{
			this.matter = matter;
			this.mobileUser = user;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			navController = this.NavigationController;
			this.Title = mobileUser.firstNames + " " + mobileUser.lastName;

			btnAssignTask.TintColor = ColorHelper.GetGPPurple ();
			btnAssignTask.Clicked += delegate {
				if (taskDescription.Text == null ||
				    taskDescription.Text.Trim ().Length == 0) {
					new UIAlertView ("Missing Data", "Please enter task description.", null, "OK").Show ();
					return;
				}
				Console.WriteLine ("taskDescription.Text = " + taskDescription.Text);
				if (isBusy) {
					return;
				}
				var task = new TaskDTO ();
				DateTime dt = datePicker.Date;
				DateTime now = DateTime.Now;
				Console.WriteLine ("### Task Due Date picked: " + dt.ToLongDateString ());
				Console.WriteLine ("### Today is: " + now.ToLongDateString ());


				task.dueDate = Tools.ConvertDateTimeToJavaMS (dt);
				task.matterID = matter.matterID;
				task.userID = mobileUser.userID;
				task.taskDescription = taskDescription.Text;
				if (switchNotify.On == true) {
					task.notifyWhenComplete = true;
				} else {
					task.notifyWhenComplete = false;
				}

				getAsyncData (task);

			};
			taskDescription.ShouldReturn += delegate {
				taskDescription.ResignFirstResponder ();
				return true;
			};
			labelMatterName.Text = matter.matterName;
			labelMatterName.TextColor = ColorHelper.GetGPPurple ();
			labelMatterName.Font = UIFont.BoldSystemFontOfSize (18);

			//labelDueDate.Font = UIFont.SystemFontOfSize (12);
			//labelDueDate.Alpha = 1;

		}

		bool isBusy;
		//
		// Asynchronous HTTP request
		//
		public void getAsyncData (TaskDTO task)
		{
			isBusy = true;
			start = DateTime.Now;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;

			cr.requestType = GhostRequestDTO.ASSIGN_TASK;
			cr.task = task;
			Console.WriteLine ("Task Description in getAsyncData: " + cr.task.taskDescription);
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.userID = NSUserDefaults.StandardUserDefaults.IntForKey ("userID");
			cr.companyID = NSUserDefaults.StandardUserDefaults.IntForKey ("companyID");
			cr.deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
			
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("Async JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;				
			
			try {
				var request = (HttpWebRequest)WebRequest.Create (url);
				request.BeginGetResponse (DataDownloaded, request);
			} catch (WebException e) {
				isBusy = false;
				Console.WriteLine ("Exception - " + e.Message);
				new UIAlertView ("Error", "Server Unavailable at this time.\nPlease try later.", null, "OK").Show ();
			}
			
			
		}

		void DataDownloaded (IAsyncResult result)
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
				
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (resp, typeof(WebServiceResponseDTO));
				
				end = DateTime.Now;	
				Tools.SendElapsedTime (start, end, dto.activityID);
				
				InvokeOnMainThread (delegate {
					if (dto.responseCode == 0) {
						if (dto.taskCreated) {
							new UIAlertView ("Assign Task", "Task has been successfully assigned to " +
							mobileUser.firstNames + " " + mobileUser.lastName, null, "OK").Show ();	
							navController.PopViewControllerAnimated (true);
						} else {
							new UIAlertView ("Assign Task Error", "Task has not been assigned", null, "OK").Show ();	
						}
					} else {
						new UIAlertView ("Assign Task", dto.responseMessage, null, "OK").Show ();
					}

				}
				);
				
				
				
			} catch (Exception ex) {
				Console.WriteLine ("### ERROR: " + ex.Message);
				InvokeOnMainThread (delegate {
					isBusy = false;
					new UIAlertView ("Network Error", "Problem communicating with server.\nPlease try later or call GhostPractice Support", null, "Close").Show ();
				}
				);
				
			}
		}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

