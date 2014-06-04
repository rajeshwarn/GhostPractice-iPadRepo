using System;
using System.Net;
using System.IO;
using System.Text;
using System.Drawing;
using MonoTouch.CoreGraphics;
using System.Text.RegularExpressions;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using MonoTouch.Dialog;
using GhostPractice;
using GhostPracticeLibrary;

namespace GPMobilePad
{
	public partial class TaskDialog : DialogViewController, IDateInterface
	{
		MatterSearchResultDTO matter;

		public DateTime selectedDate { get; set; }

		DateTime start, end;
		EntryElement narration;
		MobileUser selectedUser;
		StyledStringElement btnTar;
		//Section busySection;
		IPostingComplete listener;
		bool isBusy;
		//private UIActivityIndicatorView pv;

		private UIActivityIndicatorView getProgressIndicator ()
		{
			UIActivityIndicatorView pind = new UIActivityIndicatorView (new RectangleF (145f, 165f, 30f, 30f)) {
				ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray,
				AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleTopMargin
				| UIViewAutoresizing.FlexibleBottomMargin,
				Tag = 1
			};
			return pind;
		}

		public  void dateSelected (DateTime date)
		{                    
			selectedDate = date;
		}

		public override void ViewWillAppear (bool animated)
		{                    

		}

		public TaskDialog (MatterSearchResultDTO matter, IPostingComplete listener) : base (UITableViewStyle.Grouped, null)
		{

			this.matter = matter; 
			this.listener = listener;
			selectedDate = DateTime.Now;
			GetFeeEarners (0);
			BuildInterface ();
		}

		public void BuildInterface ()
		{
			if (Root == null) {
				Root = new RootElement ("PostTaskDialog");
			}
			Root.Clear ();
			BuildSections ();
		}

		UIActionSheet actionSheet;
		UIDatePicker picker;

		private void BuildSections ()
		{
			var headerLabel = new UILabel (new RectangleF (10, 10, 400, 40)) {
				Font = UIFont.BoldSystemFontOfSize (20),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = "Post Tasks"
			};

			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 10, 400, 40); 
			view.Add (headerLabel);
			var topSection = new Section (view);
			
			Root.Add (topSection);	

			string name = "";
			if (matter != null) {
				name = matter.matterName;
			}
			var sec = new Section (name);
			//
			addBusySection ();
			var feeEarnerName = new Section ("Fee Earner Name");
			if (selectedUser != null) {
				feeEarnerName = new Section (selectedUser.firstNames + " " + selectedUser.lastName);
			}
			var tarSec = new Section ();
			if (list != null && list.Count > 0) {			
				btnTar = new StyledStringElement ("Select Fee Earner");
				btnTar.TextColor = ColorHelper.GetGPLightPurple ();
				btnTar.Alignment = UITextAlignment.Center;
				btnTar.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				btnTar.Tapped += delegate {
					if (list == null) {
						
					} else {
						string[] btns = new string[list.Count];
						for (int i = 0; i < list.Count; i++) {
							btns [i] = list [i].firstNames + " " + list [i].lastName;
						}
						actionSheet = new UIActionSheet ("Fee Earners", null, "Cancel", null, btns) { 
							Style = UIActionSheetStyle.Default
						};

						actionSheet.Clicked += delegate (object sender, UIButtonEventArgs args) {
							if (args.ButtonIndex == list.Count) {
								//ignored - cancelled
							} else {

								selectedUser = list [args.ButtonIndex];	
								BuildInterface ();
							}
							
						};
	
						actionSheet.ShowInView (View);
					}
					
				};
				tarSec.Add (btnTar);
			}
			//


			narration = new EntryElement ("Description", "Enter description text", null);
			narration.KeyboardType = UIKeyboardType.Default;
			feeEarnerName.Add (narration);
			//
			picker = new UIDatePicker ();
			//picker.Frame = new RectangleF (10f, 10f, 320f, 320f);
			picker.Mode = UIDatePickerMode.Date;
			picker.Date = DateTime.Now;
			
			var pickerSection = new Section (picker);
			
			Root.Add (sec);
			Root.Add (tarSec);
			Root.Add (feeEarnerName);
			Root.Add (pickerSection);
			BuildButtons ();


		}

		private void BuildButtons ()
		{
			var sec = new Section ("");
			var btnSend = new StyledStringElement ("Send to Office");
			btnSend.TextColor = ColorHelper.GetGPLightPurple ();
			var switchNotify = new BooleanElement ("Notify?", true);
			btnSend.Tapped += delegate {
				//test
				if (isBusy) {
					Busy ();
					return;
				}
				if (selectedUser == null) {
					new UIAlertView ("Fee Earner", "Please select a Fee Earner", null, "OK").Show ();
					return;
				}
				var task = new TaskDTO ();
				DateTime dt = picker.Date;
				DateTime now = DateTime.Now;
				Console.WriteLine ("### Task Due Date picked: " + dt.ToLongDateString ());
				Console.WriteLine ("### Today is: " + now.ToLongDateString ());


				task.dueDate = Tools.ConvertDateTimeToJavaMS (dt);
				task.matterID = matter.matterID;
				task.userID = selectedUser.userID;
				task.taskDescription = narration.Value;
				if (switchNotify.Value == true) {
					task.notifyWhenComplete = true;
				} else {
					task.notifyWhenComplete = false;
				}


				if (narration.Value.Trim () == "") {
					new UIAlertView ("Description", "Please enter Task description", null, "OK").Show ();
					return;
				}
				sendAssignTaskRequest (task);


			};

			btnSend.Alignment = UITextAlignment.Center;
			sec.Add (switchNotify);
			sec.Add (btnSend);

			Root.Add (sec);

		}

		void addBusySection ()
		{
			if (isBusy) {
				var lv = new UIActivityIndicatorView (new RectangleF (10f, 10f, 50f, 50f));
				lv.Color = ColorHelper.GetGPLightPurple ();
				lv.StartAnimating ();
				var sv = new Section (lv);
				Root.Add (sv);
			}
		}

		private void Busy ()
		{
			Console.WriteLine ("##Boolean: comms are busy, slow down!");
			new UIAlertView (
				"Busy",
				"Still processing your last request. Please wait until the server responds or check your network settings",
				null,
				null,
				"OK"
			).Show ();
		}


		void notifyListener ()
		{
			listener.onPostingComplete ();
		}

		public void GetFeeEarners (int duration)
		{
			if (isBusy) {
				Console.WriteLine ("##GetFeeEarners: comms are busy, slow down!");
				return;
			}
			if (matter == null) {
				return;
			}

			isBusy = true;
			BuildInterface ();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			start = DateTime.Now;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
				
			cr.requestType = GhostRequestDTO.GET_FEE_EARNERS;
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.userID = NSUserDefaults.StandardUserDefaults.IntForKey ("userID");
			cr.companyID = NSUserDefaults.StandardUserDefaults.IntForKey ("companyID");
			cr.deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
				
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("$$ GetFeeEarners JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;
				
			var request = (HttpWebRequest)WebRequest.Create (url);
			request.BeginGetResponse (DataDownloaded, request);
			
		}

		List<MobileUser> list;

		void DataDownloaded (IAsyncResult result)
		{
			isBusy = false;

			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			end = DateTime.Now;
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			try {
				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (result);
				Stream receiveStream = response.GetResponseStream ();
				StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
				string resp = readStream.ReadToEnd ();
				Console.WriteLine ("$$ GetTariffCodes Response stream received.\n" + resp);
				response.Close ();
				readStream.Close ();
				//get JSON response deserialized
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (
					resp,
					typeof(WebServiceResponseDTO)
				);
				
				if (dto != null) {
					Tools.SendElapsedTime (start, end, dto.activityID);
				}	
				
				InvokeOnMainThread (delegate {
					BuildInterface ();
					selectedUser = null;
					if (dto.responseCode == 0) {
						//insert "none" record on top...
						MobileUser d = new MobileUser ();
						d.firstNames = "None";
						d.lastName = "None";
						d.userID = 0;
						list = new List<MobileUser> ();
						list.Add (d);
						foreach (var t in dto.mobileUsers) {
							list.Add (t);
						}
						BuildInterface ();

					} else {						
						new UIAlertView ("Fee Earner List Error", dto.responseMessage, null, "OK").Show ();
						return;
					}
				}
				);								
				
			} catch (Exception ex) {
				Console.WriteLine ("### ERROR: " + ex.Message);
				isBusy = false;
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

		public void sendAssignTaskRequest (TaskDTO task)
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
				request.BeginGetResponse (AssignTaskRequestCompleted, request);
			} catch (WebException e) {
				isBusy = false;
				Console.WriteLine ("Exception - " + e.Message);
				new UIAlertView ("Error", "Server Unavailable at this time.\nPlease try later.", null, "OK").Show ();
			}


		}


		void AssignTaskRequestCompleted (IAsyncResult result)
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
							selectedUser.firstNames + " " + selectedUser.lastName, null, "OK").Show ();	
							//navController.PopViewControllerAnimated (true);
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

		public override bool ShouldAutorotate ()
		{
			return false;
		}
	}
}
