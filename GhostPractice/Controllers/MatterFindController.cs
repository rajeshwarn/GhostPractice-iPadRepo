using System;
using System.Net;
using System.IO;
using System.Text;
using System.Drawing;

//using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using System.Globalization;

namespace GhostPractice
{
	public partial class MatterFindController : UIViewController
	{
		//PracticeBranchController pbController;
		ReportsCoordinatorDialog reportsCoordinator;
		static List<MatterSearchResultDTO> list;
		static NSString cellIdentifier = new NSString ("CellId");
		//CLLocationCoordinate2D coordinate;
		DateTime start, end;
		public static bool IsTall 

		{ 

			get 

			{ 

				return UIDevice.CurrentDevice.UserInterfaceIdiom 

					== UIUserInterfaceIdiom.Phone 

						&& UIScreen.MainScreen.Bounds.Height 

						* UIScreen.MainScreen.Scale >= 1136; 

			} 

		} 
		public MatterFindController () : base ("MatterFindController", null)
		{

		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		private void testCurrencies ()
		{
			string name = CultureInfo.CurrentCulture.Name;
			Console.WriteLine ("Current culture name: " + name);
			new UIAlertView ("CultureInfo", "Current culture name: " + name, null, "OK").Show ();
			StringBuilder sb = new StringBuilder ();
			// Loop through all the specific cultures known to the CLR.
			foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures)) {
				// Only show the currency symbols for cultures that speak English.
				if (ci.TwoLetterISOLanguageName != "en")
					continue;

				// Display the culture name and currency symbol.
				NumberFormatInfo nfi = ci.NumberFormat;
				sb.AppendFormat ("The currency symbol for '{0}' is '{1}' groupSeparator is '{2}' name: '{3}'",
                ci.DisplayName, nfi.CurrencySymbol, ci.NumberFormat.CurrencyGroupSeparator, ci.Name);
				sb.AppendLine ();
			}
			Console.WriteLine (sb.ToString ());
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Console.WriteLine ("MatterFindController, ViewDidLoad is running !!!");
			this.tableView.RowHeight = 60;
			this.NavigationController.NavigationBarHidden = true;
			//coordinate = LocationService.GetCurrentLocation ();
			btnReports.TintColor = ColorHelper.GetGPPurple ();
			btnCount.TintColor = ColorHelper.GetGPPurple ();
			btnCount.Style = UIBarButtonItemStyle.Plain;
			btnReports.Clicked += delegate {
//				if (pbController == null) {
//					pbController = new PracticeBranchController ();	
//				}
//				this.NavigationController.PushViewController (pbController, true);
				if (reportsCoordinator == null) {
					reportsCoordinator = new ReportsCoordinatorDialog ();
				}
				this.NavigationController.PushViewController (reportsCoordinator, true);
			};
			searchBar.SearchButtonClicked += delegate {
				searchBar.ResignFirstResponder ();
				if (isBusy) {
					new UIAlertView ("Search", "Searching is continuing, cannot start new search", null, "OK").Show ();
					cnt++;
					if (cnt > 1) {
						isBusy = false;
						cnt = 0;
					}
					return;
				}
				string s = searchBar.Text.Trim ();
				if (s == "" || s.Length == 0) {
					new UIAlertView ("Search", "Please enter search text", null, "OK").Show ();
					return;
				} else {
					getAsyncData ();
				}
			};	
			
			searchBar.BookmarkButtonClicked += delegate {
				Console.WriteLine ("book search bar cancel pressed");
				searchBar.ResignFirstResponder ();
			};
			searchBar.CancelButtonClicked += delegate {
				Console.WriteLine ("search bar cancel pressed");
				searchBar.ResignFirstResponder ();
			};
			
		}	
		int cnt;
		bool isBusy;
		//
		// Asynchronous HTTP request
		//
		public void getAsyncData ()
		{
			isBusy = true;
			start = DateTime.Now;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
				
			cr.requestType = GhostRequestDTO.FIND_MATTER;
			cr.searchString = searchBar.Text;
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
		
		//
		// Invoked when we get the stream back from the twitter feed
		// We parse the RSS feed and push the data into a 
		// table.
		//
		void DataDownloaded (IAsyncResult result)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			isBusy = false;
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			list = new List<MatterSearchResultDTO> ();
			try {
				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (result);
				Stream receiveStream = response.GetResponseStream ();
				StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
				string resp = readStream.ReadToEnd ();
				Console.WriteLine ("## MatterFind Response stream received.\n" + resp);
				response.Close ();
				readStream.Close ();
				
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (resp, typeof(WebServiceResponseDTO));
				
				end = DateTime.Now;	
				Tools.SendElapsedTime (start, end, dto.activityID);
				
				InvokeOnMainThread (delegate {
					searchBar.Hidden = false;
					if (dto.responseCode == 0) {
						list = dto.matterSearchList;
						if (list != null && list.Count == 0) {
							btnCount.Title = "Matters: " + 0;
							new UIAlertView ("Search Result", "No matters found for this search", null, "OK").Show ();	
						}
					} else {
						new UIAlertView ("Search Result", dto.responseMessage, null, "OK").Show ();
					}
					btnCount.Title = "Matters: " + list.Count;
					btnCount.TintColor = UIColor.White;
					//
					tableView.Delegate = new TableViewDelegate (list, this);
					tableView.DataSource = new TableViewDataSource (list);
					tableView.ReloadData ();
					searchBar.ResignFirstResponder ();
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
		private class TableViewDelegate : UITableViewDelegate
		{
			private List<MatterSearchResultDTO> list;
			private UIViewController controller;

			public TableViewDelegate (List<MatterSearchResultDTO> list, UIViewController controller)
			{
				this.list = list;
				this.controller = controller;
			}

			public override void RowSelected (
                UITableView tableView, NSIndexPath indexPath)
			{
				MatterDetailsDialog diag = new MatterDetailsDialog (list [indexPath.Row]);
				controller.NavigationController.PushViewController (diag, true);
			}
			public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
			{
				Console.WriteLine ("## Accessory tapped: " + list [indexPath.Row].matterName);
				MatterDetailsDialog diag = new MatterDetailsDialog (list [indexPath.Row]);
				controller.NavigationController.PushViewController (diag, true);
					
			}
		}

		private class TableViewDataSource : UITableViewDataSource
		{
			static NSString kCellIdentifier =
                new NSString ("MyIdentifier");
			private List<MatterSearchResultDTO> list;

			public TableViewDataSource (List<MatterSearchResultDTO> list)
			{
				this.list = list;
			}

			public override int RowsInSection (
                UITableView tableview, int section)
			{
				return list.Count;
			}
			
			public override UITableViewCell GetCell (
                UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (kCellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellIdentifier);
					cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
				}
				
				//set up image
				
				UIImage img = UIImage.FromFile ("Images/matter.png");

				cell.TextLabel.Text = list [indexPath.Row].matterName;
				cell.ImageView.Image = img;
				cell.DetailTextLabel.Text = list [indexPath.Row].currentOwner;
				
				
				return cell;
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
			//return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			return false;
		}
		
		
			
	}

}