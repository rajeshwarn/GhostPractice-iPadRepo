using System;
using System.Net;
using System.IO;
using System.Text;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using MonoTouch.Dialog;
using GhostPracticeLibrary;

namespace GhostPractice
{
	public partial class MatterDetailsDialog : DialogViewController, IPostingComplete
	{
		public MatterDTO matter { get; set; }

		MatterSearchResultDTO searchResult;
		StringsElement clientName, ownerName, legacy;
		NumberElement matterID;
		FinanceElement busBal, currBal, trustBal, reserveTrust, unbilled, pending, investTrust;
		StyledStringElement btnPostFee, btnPostUnbillable, btnPostNote, btnBack, btnAssignTask;
		DateTime start, end;
		UILabel headerLabel;
		bool isBusy;

		public void onPostingComplete ()
		{
			Console.WriteLine ("onPostingComplete");
		}

		public MatterDetailsDialog (MatterSearchResultDTO searchResult) : base (UITableViewStyle.Grouped, null)
		{	
			this.searchResult = searchResult;
			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem (S.GetText (S.SEARCH), UIBarButtonItemStyle.Bordered, delegate(object sender, EventArgs e) {   
				NavigationController.PopViewControllerAnimated (true);
			});
			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Reports", UIBarButtonItemStyle.Bordered, delegate(object sender, EventArgs e) {   
				var con = new ReportsCoordinatorDialog ();
				NavigationController.PushViewController (con, true);
			});
			GetMatterDetails ();
			BuildInterface ();
		}

		private void Busy ()
		{
			headerLabel.Text = "Busy. Loading.....";
			Console.WriteLine ("##Boolean: comms are busy, slow down!");

		}

		public void BuildInterface ()
		{
			if (Root == null) {
				Root = new RootElement ("Matter Details");
			}
			Root.Clear ();
			headerLabel = new UILabel (new RectangleF (10, 10, 300, 40)) {
				Font = UIFont.BoldSystemFontOfSize (20),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = searchResult.matterName
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 10, 300, 40); 
			view.Add (headerLabel);
			var topSection = new Section (view);
			
			Root.Add (topSection);	
			//
			var sec = new Section ("");
			//matterName = new TitleElement (searchResult.matterName);
			ownerName = new StringsElement ("Owner", searchResult.currentOwner);
			matterID = new NumberElement (
				Convert.ToInt16 (searchResult.matterID),
				"Matter ID"
			);
			if (searchResult.matterLegacyAccount == null || searchResult.matterLegacyAccount.Trim () == "") {
				//ignore
			} else {
				legacy = new StringsElement (
					"Legacy Account",
					searchResult.matterLegacyAccount
				);
			}
			clientName = new StringsElement ("Client", "" + searchResult.clientName);
			//
			var sec2 = new Section ("");
			if (matter == null) {
				busBal = new FinanceElement (S.GetText (S.BUSINESS_BALANCE) + ":", 0.00);
				currBal = new FinanceElement (S.GetText (S.CURRENT_BALANCE) + ":", 0.00);
				trustBal = new FinanceElement (S.GetText (S.TRUST_BALANCE) + ":", 0.00);
				reserveTrust = new FinanceElement (S.GetText (S.RESERVE_TRUST) + ":", 0.00);
				unbilled = new FinanceElement (S.GetText (S.UNBILLED_BALANCE) + ":", 0.00);
				pending = new FinanceElement (S.GetText (S.PENDING_DISBURSEMENTS) + ":", 0.00);
				investTrust = new FinanceElement (S.GetText (S.INVESTMENT_TRUST) + ":", 0.00);
			} else {
				busBal = new FinanceElement (S.GetText (S.BUSINESS_BALANCE) + ":", matter.businessBalance);
				currBal = new FinanceElement (S.GetText (S.CURRENT_BALANCE), matter.currentBalance);
				unbilled = new FinanceElement (S.GetText (S.UNBILLED_BALANCE) + ":", matter.unbilledBalance);
				trustBal = new FinanceElement (S.GetText (S.TRUST_BALANCE), matter.trustBalance);
				reserveTrust = new FinanceElement (S.GetText (S.RESERVE_TRUST), matter.reserveTrust);
				pending = new FinanceElement (
					S.GetText (S.PENDING_DISBURSEMENTS) + ":",
					matter.pendingDisbursementBalance
				);
				investTrust = new FinanceElement (
					S.GetText (S.INVESTMENT_TRUST) + ":",
					matter.investmentTrustBalance
				);
				
			}
			//
			//sec.Add (matterName);
			sec.Add (clientName);
			sec.Add (ownerName);
			sec.Add (matterID);
			Root.Add (sec);
			//
			sec2.Add (busBal);
			sec2.Add (trustBal);			
			sec2.Add (investTrust);
			sec2.Add (currBal);
			sec2.Add (reserveTrust);
			sec2.Add (unbilled);
			sec2.Add (pending);
			if (legacy != null) {
				sec2.Add (legacy);	
			}
			
			Root.Add (sec2);
			
			//
			var sec3 = new Section (searchResult.matterName);
			btnPostFee = new StyledStringElement ("Post Fee");
			btnPostUnbillable = new StyledStringElement ("Post Unbillable");
			btnPostNote = new StyledStringElement ("Post Note");
			btnBack = new StyledStringElement (S.GetText (S.MATTER_SEARCH));
			btnAssignTask = new StyledStringElement ("Assign Task");

			btnAssignTask.Tapped += delegate {
				if (isBusy) {
					Busy ();
				} else {
					matter.id = searchResult.matterID;
					var c = new FeeEarnerListController (searchResult);
					this.NavigationController.PushViewController (c, true);
				}
				
			};
			btnPostFee.Tapped += delegate {
				if (isBusy) {
					Busy ();
				} else {
					matter.id = searchResult.matterID;
					var c = new PostFeeDialog (matter, false, this);
					this.NavigationController.PushViewController (c, true);
				}
				
			};
			btnPostUnbillable.Tapped += delegate {
				if (isBusy) {
					Busy ();
				} else {
					matter.id = searchResult.matterID;
					var c = new PostFeeDialog (matter, true, this);
					this.NavigationController.PushViewController (c, true);
				}
			};
			btnPostNote.Tapped += delegate {
				if (isBusy) {
					Busy ();
				} else {
					matter.id = searchResult.matterID;
					var c = new PostNoteDialog (matter, this);
					this.NavigationController.PushViewController (c, true);
				}
			};
			btnBack.Tapped += delegate() {
				if (isBusy) {
					Busy ();
				} else {
					this.NavigationController.PopViewControllerAnimated (true);
				}
				
			};
			btnPostFee.Alignment = UITextAlignment.Center;
			btnPostUnbillable.Alignment = UITextAlignment.Center;
			btnPostNote.Alignment = UITextAlignment.Center;
			btnBack.Alignment = UITextAlignment.Center;
			btnAssignTask.Alignment = UITextAlignment.Center;
			//
			sec3.Add (btnPostFee);
			sec3.Add (btnPostUnbillable);
			sec3.Add (btnPostNote);
			sec3.Add (btnAssignTask);
			sec3.Add (btnBack);
			Root.Add (sec3);
			
		}

		public void GetMatterDetails ()
		{		
			isBusy = true;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			start = DateTime.Now;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
				
			cr.requestType = GhostRequestDTO.GET_MATTER_DETAIL;
			cr.matterID = searchResult.matterID;
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.userID = NSUserDefaults.StandardUserDefaults.IntForKey ("userID");
			cr.companyID = NSUserDefaults.StandardUserDefaults.IntForKey ("companyID");
			cr.deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("Async JSON = " + json);
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
				Console.WriteLine ("## ASYNCResponse stream received.\n" + resp);
				response.Close ();
				readStream.Close ();
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (resp, typeof(WebServiceResponseDTO)
				);
				if (dto != null) {

					Tools.SendElapsedTime (start, end, dto.activityID);
				}
				
				InvokeOnMainThread (delegate {
					isBusy = false;
					try {
						if (dto.responseCode > 0) {						
							new UIAlertView ("Matter Details", dto.responseMessage, null, "OK").Show ();
							return;
						} else {
							matter = dto.matter;
							BuildInterface ();
						}
					} catch (Exception e) {
						Console.WriteLine ("### IGNORED: " + e.Message);
						//ignore - 
					}
				}
				);
				
				
				
			} catch (Exception ex) {
				Console.WriteLine ("shit " + ex.ToString ());
				InvokeOnMainThread (delegate {
					isBusy = false;
					new UIAlertView ("Network Error", "Problem communicating with server, \n\nCheck your network connections and try again later", null, "Close"
					).Show ();
				
				}
				);
			}
		}
	}
}
