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
using GhostPractice;
using GhostPracticeLibrary;


namespace GPMobilePad
{
	public partial class MatterDetail : DialogViewController, IPostingComplete
	{
		MatterDTO matter;
		MatterSearchResultDTO searchResult;
		List<MatterSearchResultDTO> searchResults;
		TitleElement matterName;
		StringsElement clientName, ownerName, legacy;
		NumberElement matterID;
		FinanceElement busBal, currBal, trustBal, reserveTrust, unbilled, pending, investTrust;
		DateTime start, end;
		UILabel headerLabel;
		bool isBusy;
		float amountFontSize, captionFontSize;
		SplitController split;
		Finder finder;
		int deviceType;

		public UIPopoverController Popover { get; set; }

		public MatterDetail (SplitController split, Finder finder)
			: base (UITableViewStyle.Grouped, null, true)
		{
			this.split = split;
			this.finder = finder;

			//
			Autorotate = true;
			deviceType = DeviceTypeUtil.getDeviceType ();
			setButtons ();
			BuildInterface ();
			//
			var model = UIDevice.CurrentDevice.Model.ToString ();
			var name = UIDevice.CurrentDevice.Name.ToString ();
			RectangleF rec = UIScreen.MainScreen.Bounds;

			Console.WriteLine ("Device", "Name: " + name + " model: " + model + " Screen Width: " + rec.Width + " Height: " + rec.Height);
			//
			if (searchResult != null) {
				if (matter == null) {
					GetMatterDetails ();
				} else {
					enableButtons ();
				}

			}

		}

		public void Update (MatterSearchResultDTO result)
		{

			searchResult = result;
			//setButtons ();
			//BuildInterface ();
			// dismiss the popover if currently visible
			if (Popover != null)
				Popover.Dismiss (true);

			GetMatterDetails ();
		}

		public void onPostingComplete ()
		{
			pop.Dismiss (true);
		}

		UIPopoverController pop;

		private void setButtons ()
		{
			btnReports = new UIBarButtonItem ("Reports", UIBarButtonItemStyle.Plain, delegate (object sender, EventArgs e) {
				Console.WriteLine ("reports button clicked, width: " + View.Frame.Width + " height: " + View.Frame.Height);
				if (isBusy) {
					Busy ();
				} else {
					//start new splitView with report selector and report container
					//split = new UISplitViewController ();
					var pageController = new PagedViewController (split, finder, this);
					//var pageController = new BohaPageViewController ();
					var reportSelector = new ReportSelectorController (pageController, true);
					var masterNavigationController = new UINavigationController (reportSelector);


					//var src = new ReportsDataSource ();
					//reportSelector.pageController = pageController;
					//pageController.PagedViewDataSource = src;
					var detailNavigationController = new UINavigationController (pageController);

					split.WeakDelegate = pageController;
					split.ViewControllers = new UIViewController[] {
						masterNavigationController,
						detailNavigationController
					};

				}
			});
			btnPostNote = new UIBarButtonItem ("Post Note", UIBarButtonItemStyle.Plain, delegate (object sender, EventArgs e) {
				Console.WriteLine ("post note button clicked, width: " + View.Frame.Width + " height: " + View.Frame.Height);
				if (isBusy) {
					Busy ();
				} else {
					//set up popover with postNote
					matter.id = searchResult.matterID;
					var pn = new PostNote (matter, this);
					pop = new UIPopoverController (pn);
					pop.SetPopoverContentSize (new SizeF (400f, 700f), true);
					if (View.Frame.Height > 900) {
						pop.SetPopoverContentSize (new SizeF (400f, 950f), true);
						pop.PresentFromRect (new RectangleF (0f, 30f, 400f, 950f), this.View, UIPopoverArrowDirection.Any, true);
					} else {
						pop.PresentFromRect (new RectangleF (0f, 10f, 400f, 700f), this.TableView, UIPopoverArrowDirection.Any, true);
					}

				}
			});
			btnPostFee = new UIBarButtonItem ("Post Fee", UIBarButtonItemStyle.Plain, delegate (object sender, EventArgs e) {
				Console.WriteLine ("post fee button clicked, width: " + View.Frame.Width + " height: " + View.Frame.Height);
				if (isBusy) {
					Busy ();
				} else {
					//set up popover with postFee
					matter.id = searchResult.matterID;
					var pn = new PostFee (this, this, matter, false);
					pop = new UIPopoverController (pn);
					pop.SetPopoverContentSize (new SizeF (500f, 900f), true);
					if (View.Frame.Height > 900) {
						pop.PresentFromRect (new RectangleF (0f, 40f, 500f, 900f), this.View, UIPopoverArrowDirection.Any, true);
					} else {
						pop.PresentFromRect (new RectangleF (0f, 20f, 500f, 700f), this.TableView, UIPopoverArrowDirection.Any, true);
					}
				}
			});
			btnPostUnbillable = new UIBarButtonItem ("Post Unbillable", UIBarButtonItemStyle.Plain, delegate (object sender, EventArgs e) {
				Console.WriteLine ("post unbilled button clicked, width: " + View.Frame.Width + " height: " + View.Frame.Height);
				if (isBusy) {
					Busy ();
				} else {
					//set up popover with postFee
					matter.id = searchResult.matterID;
					var pn = new PostFee (this, this, matter, true);
					pop = new UIPopoverController (pn);
					pop.SetPopoverContentSize (new SizeF (500f, 900f), true);
					if (View.Frame.Height > 900) {
						pop.PresentFromRect (new RectangleF (0f, 40f, 500f, 900f), this.View, UIPopoverArrowDirection.Any, true);
					} else {
						pop.PresentFromRect (new RectangleF (0f, 20f, 500f, 700f), this.TableView, UIPopoverArrowDirection.Any, true);
					}

				}
			});
			btnAbout = new UIBarButtonItem ("About", UIBarButtonItemStyle.Plain, delegate (object sender, EventArgs e) {
				var s = "\n";
				s += "UserName: " + NSUserDefaults.StandardUserDefaults.StringForKey ("userName") + "\n";
				s += "UserID: " + NSUserDefaults.StandardUserDefaults.StringForKey ("userID") + "\n";
				if (NSUserDefaults.StandardUserDefaults.StringForKey ("companyName") != null) {
					s += "Practice: " + NSUserDefaults.StandardUserDefaults.StringForKey ("companyName") + "\n";
				}
				s += "App Version: " + NSBundle.MainBundle.InfoDictionary ["CFBundleVersion"] + "\n";
				new UIAlertView ("User Information", s, null, "OK").Show ();
			});
			btnTask = new UIBarButtonItem ("Tasks", UIBarButtonItemStyle.Plain, delegate (object sender, EventArgs e) {
				Console.WriteLine ("post task button clicked, width: " + View.Frame.Width + " height: " + View.Frame.Height);
				if (isBusy) {
					Busy ();
				} else {
					//set up popover with postTask
					matter.id = searchResult.matterID;
					var pn = new TaskDialog (searchResult, this);
					pop = new UIPopoverController (pn);
					pop.SetPopoverContentSize (new SizeF (500f, 900f), true);
					if (View.Frame.Height > 900) {
						pop.PresentFromRect (new RectangleF (0f, 40f, 500f, 900f), this.View, UIPopoverArrowDirection.Any, true);
					} else {
						pop.PresentFromRect (new RectangleF (0f, 20f, 500f, 700f), this.TableView, UIPopoverArrowDirection.Any, true);
					}

				}

			});

			//btnReports.TintColor = UIColor.Black;
			disableButtons ();

			UIBarButtonItem[] btns = { btnAbout, btnReports, btnTask, btnPostFee, btnPostUnbillable, btnPostNote };
			this.NavigationItem.SetRightBarButtonItems (btns, true);

		}

		private void disableButtons ()
		{
			btnPostFee.Enabled = false;
			btnPostNote.Enabled = false;
			btnPostUnbillable.Enabled = false;
			btnTask.Enabled = false;
		}

		private void enableButtons ()
		{
			btnPostFee.Enabled = true;
			btnPostNote.Enabled = true;
			btnPostUnbillable.Enabled = true;
			btnTask.Enabled = true;
		}

		[Export ("splitViewController:willHideViewController:withBarButtonItem:forPopoverController:")]
		public void WillHideViewController (UISplitViewController splitController, UIViewController viewController, UIBarButtonItem barButtonItem, UIPopoverController popoverController)
		{
			Console.WriteLine ("MatterDetail - WillHideViewController setting up Find Matter button");
			barButtonItem.Title = S.GetText (S.MATTER_SEARCH);
			NavigationItem.SetLeftBarButtonItem (barButtonItem, true);
			masterPopoverController = popoverController;
		}

		[Export ("splitViewController:willShowViewController:invalidatingBarButtonItem:")]
		public void WillShowViewController (UISplitViewController svc, UIViewController vc, UIBarButtonItem button)
		{
			Console.WriteLine ("MatterDetail - WillShowViewController");
			// Called when the view is shown again in the split view, invalidating the button and popover controller.
			NavigationItem.SetLeftBarButtonItem (null, true);
			masterPopoverController = null;
		}

		private void Busy ()
		{
			headerLabel.Text = "Busy. Loading.....";
			Console.WriteLine ("##Boolean: comms are busy, please slow down!");
			
		}

		public void BuildInterface ()
		{
			if (Root == null) {
				Root = new RootElement ("");
			}
			Root.Clear ();
			String name = "";
			if (searchResult != null) {
				name = searchResult.matterName;
			}
			headerLabel = new UILabel (new RectangleF (40, 10, 620, 40)) {
				Font = UIFont.BoldSystemFontOfSize (20),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = name
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (40, 10, 620, 40); 
			view.Add (headerLabel);
			var topSection = new Section (view);
			
			Root.Add (topSection);	
			//
			var sec = new Section ("");
			matterName = new TitleElement ("Matter Details");
			if (searchResult != null) {
				ownerName = new StringsElement ("Owner", searchResult.currentOwner);
				matterID = new NumberElement (
					Convert.ToInt16 (searchResult.matterID), "Matter ID");
				if (searchResult.matterLegacyAccount == null || searchResult.matterLegacyAccount.Trim () == "") {
					//ignore
				} else {
					legacy = new StringsElement (
						"Legacy Account",
						searchResult.matterLegacyAccount
					);
				}
				clientName = new StringsElement ("Client", "" + searchResult.clientName);
			}
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
				busBal = new FinanceElement (S.GetText (S.BUSINESS_BALANCE) + ":", matter.businessBalance, deviceType);
				currBal = new FinanceElement (S.GetText (S.CURRENT_BALANCE) + ":", matter.currentBalance, deviceType);
				unbilled = new FinanceElement (S.GetText (S.UNBILLED_BALANCE) + ":", matter.unbilledBalance, deviceType);
				trustBal = new FinanceElement (S.GetText (S.TRUST_BALANCE) + ":", matter.trustBalance, deviceType);
				reserveTrust = new FinanceElement (S.GetText (S.RESERVE_TRUST) + ":", matter.reserveTrust, deviceType);
				pending = new FinanceElement (S.GetText (S.PENDING_DISBURSEMENTS) + ":", matter.pendingDisbursementBalance, deviceType);
				investTrust = new FinanceElement (S.GetText (S.INVESTMENT_TRUST) + ":", matter.investmentTrustBalance, deviceType
				);
				
			}
			//
			sec.Add (matterName);
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
		}

		public void GetMatterDetails ()
		{		
			isBusy = true;
			disableButtons ();
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
					matter = dto.matter;
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
							enableButtons ();
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

		UIPopoverController masterPopoverController;
		UIBarButtonItem btnReports, btnPostFee, btnPostUnbillable, btnPostNote, btnAbout, btnTask;

		public override bool ShouldAutorotate ()
		{
			Console.WriteLine ("MatterDetail: ShouldAutoRotate is FALSE");

			return false;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			Console.WriteLine ("MatterDetail ShouldAuto, returning false: " + toInterfaceOrientation.ToString ());
			return false;
		}


	}
}
