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

//using MonoTouch.Dialog.PickerElement;
namespace GPMobilePad
{
	public partial class PostNote : DialogViewController, IDateInterface
	{
		MatterDTO matter;

		public DateTime selectedDate { get; set; }

		DateTime start, end;
		EntryElement narration;
		MobileTariffCodeDTO selectedTariff;
		List<MobileTariffCodeDTO> tariffList;
		StyledStringElement btnTar;
		//DateTimeElement2 date;
		Section busySection;
		IPostingComplete listener;
		bool isBusy;
		private UIActivityIndicatorView pv;

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

		public PostNote (MatterDTO matter, IPostingComplete listener) : base (UITableViewStyle.Grouped, null)
		{
			this.matter = matter; 
			this.listener = listener;
			selectedDate = DateTime.Now;
			GetTariffCodes (0);
			BuildInterface ();
		}

		public void BuildInterface ()
		{
			if (Root == null) {
				Root = new RootElement ("PostNoteDialog");
			}
			Root.Clear ();
			BuildSections ();
		}

		UIActionSheet actionSheet;
		UIDatePicker picker;

		private void BuildSections ()
		{
			var headerLabel = new UILabel (new RectangleF (10, 10, 370, 40)) {
				Font = UIFont.BoldSystemFontOfSize (20),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = "Post Note"
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 10, 370, 40); 
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
			var tarSec = new Section ();
			if (tariffList != null && tariffList.Count > 0) {			
				btnTar = new StyledStringElement ("Select Activity Code");
				btnTar.TextColor = ColorHelper.GetGPLightPurple ();
				btnTar.Alignment = UITextAlignment.Center;
				btnTar.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				btnTar.Tapped += delegate {
					if (tariffList == null) {
						
					} else {
						string[] btns = new string[tariffList.Count];
						for (int i = 0; i < tariffList.Count; i++) {
							btns [i] = tariffList [i].name;
						}
						actionSheet = new UIActionSheet ("Activity Codes", null, "Cancel", null, btns) { 
							Style = UIActionSheetStyle.Default
						};

						actionSheet.Clicked += delegate (object sender, UIButtonEventArgs args) {
							if (args.ButtonIndex == tariffList.Count) {
								//ignored - cancelled
							} else {
								narration.Value = tariffList [args.ButtonIndex].narration;
								selectedTariff = tariffList [args.ButtonIndex];	
								BuildInterface ();
							}
							
						};
	
						actionSheet.ShowInView (View);
					}
					
				};
				tarSec.Add (btnTar);
			}
			//
			var narrSec = new Section ("Note Narration");

			if (selectedTariff == null) {
				narration = new EntryElement ("Narration", "Narration", null);
			} else {
				narrSec = new Section (selectedTariff.name);
				narration = new EntryElement ("Narration", "Narration", selectedTariff.narration);
			}
			narration.KeyboardType = UIKeyboardType.Default;
			narrSec.Add (narration);
			//
			picker = new UIDatePicker ();
			//picker.Frame = new RectangleF (10f, 10f, 320f, 320f);
			picker.Mode = UIDatePickerMode.Date;
			picker.Date = DateTime.Now;
			
			var pickerSection = new Section (picker);
			
			Root.Add (sec);
			Root.Add (tarSec);
			Root.Add (narrSec);
			Root.Add (pickerSection);
			BuildButtons ();


		}

		private void BuildButtons ()
		{
			var sec = new Section ("");
			var btnSend = new StyledStringElement ("Send to Office");
			btnSend.TextColor = ColorHelper.GetGPLightPurple ();
			btnSend.Tapped += delegate {
				//test
				if (isBusy) {
					Busy ();
					return;
				}
//				if (selectedTariff == null) {
//					new UIAlertView ("Activity Code", "Please select an Activity Code", null, "OK").Show ();
//					return;
//				}
				var note = new MatterNoteDTO ();
				if (picker != null) {
					picker.TimeZone = NSTimeZone.DefaultTimeZone;
					DateTime dt = DateTime.Now;
					Console.WriteLine ("----> PostNote picker local date: " + dt.ToString ());
					note.date = Tools.ConvertDateTimeToJavaMS (dt);
				} 

//				if (date != null) {
//					note.date = Tools.ConvertDateTimeToJavaMS (date.DateValue);
//					Console.WriteLine ("PostNote date selected: " + date.DateValue);
//				} else {
//					note.date = Tools.ConvertDateTimeToJavaMS (DateTime.Now);
//				}

				note.matterID = Convert.ToInt16 (matter.id);
				if (narration.Value.Trim () == "") {
					new UIAlertView ("Narration", "Please enter Note narration", null, "OK").Show ();
					return;
				}
				note.narration = narration.Value;
				note.tariffCodeID = selectedTariff.id;
				PostNoteToOffice (note);
				
			};

			btnSend.Alignment = UITextAlignment.Center;
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
		/*
		 * Methods to access web services async
		 * 
		 */
		public void PostNoteToOffice (MatterNoteDTO note)
		{
			if (isBusy) {
				Console.WriteLine ("##PostNote: comms are busy, slow down!");
				return;
			}
			isBusy = true;
			BuildInterface ();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			start = DateTime.Now;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
			
			cr.requestType = GhostRequestDTO.POST_NOTE;
			cr.note = note;
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.userID = NSUserDefaults.StandardUserDefaults.IntForKey ("userID");
			cr.companyID = NSUserDefaults.StandardUserDefaults.IntForKey ("companyID");
			cr.deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
				
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("@@ PostNote JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;
				
			var request = (HttpWebRequest)WebRequest.Create (url);
			request.BeginGetResponse (PostComplete, request);	
			
			
		}

		void PostComplete (IAsyncResult result)
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
				Console.WriteLine ("## PostNote Response stream received.\n" + resp);
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

					if (dto.responseCode == 0) {
						try {
							new UIAlertView (
								"Posting Note",
								"Posting has been successfully completed",
								null,
								"OK"
							).Show ();
							//no need to refresh MatterDetails?????
							//this.NavigationController.PopViewControllerAnimated (true);
						} catch (Exception e) {
							//ignore - trapping event this is not there to host pop up
							Console.WriteLine ("### IGNORED A: " + e.Message);
						}
					} else {
						try {
							new UIAlertView ("Posting Note Error", dto.responseMessage, null, "OK").Show ();
						} catch (Exception e) {
							//ignore - trapping event this is not there to host pop up
							Console.WriteLine ("### IGNORED B: " + e.Message);
						}
					}
					notifyListener ();
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

		void notifyListener ()
		{
			listener.onPostingComplete ();
		}

		public void GetTariffCodes (int duration)
		{
			if (isBusy) {
				Console.WriteLine ("##GetTariffCodes: comms are busy, slow down!");
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
				
			cr.requestType = GhostRequestDTO.GET_TARIFF_CODES;
			cr.matterID = matter.id;
			cr.duration = duration;
			cr.tarrifCodeType = DataUtil.TARIFF_CODE_TYPE_NOTES;
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.userID = NSUserDefaults.StandardUserDefaults.IntForKey ("userID");
			cr.companyID = NSUserDefaults.StandardUserDefaults.IntForKey ("companyID");
			cr.deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
				
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("$$ GetTariffCodes JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;
				
			var request = (HttpWebRequest)WebRequest.Create (url);
			request.BeginGetResponse (DataDownloaded, request);
			
		}

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
					selectedTariff = null;
					if (dto.responseCode == 0) {
						//insert "none" record on top...
						MobileTariffCodeDTO d = new MobileTariffCodeDTO ();
						d.id = 0;
						d.name = "None";
						d.narration = "None";
						tariffList = new List<MobileTariffCodeDTO> ();
						tariffList.Add (d);
						foreach (var t in dto.mobileTariffCodeList) {
							tariffList.Add (t);
						}
						BuildInterface ();
						if (tariffList == null || tariffList.Count == 0) {
							new UIAlertView (
								"Activity Code Error",
								"No Activity codes found for matter",
								null,
								"OK"
							).Show ();	
						}
					} else {						
						new UIAlertView ("Activity Code Error", dto.responseMessage, null, "OK").Show ();
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

		public override bool ShouldAutorotate ()
		{
			return false;
		}
	}
}
