using System;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Web;

namespace GhostAdmin
{
	public partial class CompanyListController : UIViewController
	{
		static List<CompanyDTO> list;
		static NSString cellIdentifier = new NSString ("CellId");
		
		public CompanyListController () : base ("CompanyListController", null)
		{
			Title = "GhostPractice Mobile Console";
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
			
			//disable refresh btn
			btnRefresh.Enabled = true;
				
			btnRefresh.Clicked += delegate {
				doRefresh ();
			};
			btnSummaryReport.Clicked += delegate {
				doSummaryRequest ();
			};
			
			btnActivityLogs.Title = "Platforms";
			btnActivityLogs.Clicked += delegate {				
				doPlatformRequest ();
			};
				
			doCompanyRequest ();
		
		}
		
		private void doRefresh ()
		{
			doCompanyRequest ();	
			
		}

		private void doPlatformRequest ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			btnActivityLogs.Enabled = false;

			try {
				ConsoleRequest cr = new ConsoleRequest ();
				string json, encodedJSON, url, resp;
				ConsoleResponseDTO dto;
				cr.requestType = ConsoleRequest.PLATFORM_SUMMARY_ALL;
				cr.appID = 1;
				
				json = JsonConvert.SerializeObject (cr);
				encodedJSON = HttpUtility.UrlEncode (json);
				url = Tools.CONSOLE_URL + encodedJSON;
				resp = TalkToServer.getAsyncData (url);
				
				dto = (ConsoleResponseDTO)JsonConvert.DeserializeObject (resp, typeof(ConsoleResponseDTO));
				if (dto.responseCode > 0) {
					displayConsoleError (dto);
					return;
				}
				List<PlatformSummaryDTO> sList = dto.platformSummaries;
				var contx = new PlatformController (sList,0);
				this.NavigationController.PushViewController (contx, true);
				
				
			} catch (System.Net.WebException ex) {
				Console.WriteLine ("Server unavailable: " + ex.Message);
				btnRefresh.Enabled = true;
				new UIAlertView ("Server Error", "Server not available", null, "OK").Show ();
			} catch (Exception ex) {
				Console.WriteLine ("Network unavailable: " + ex.Message);
				btnRefresh.Enabled = true;
				new UIAlertView ("Network Error", "Network not available", null, "OK").Show ();
			}
			
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			btnActivityLogs.Enabled = true;
		}

		private void doSummaryRequest ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			btnSummaryReport.Enabled = false;
			ConsoleRequest cr = new ConsoleRequest ();
			string json, encodedJSON, url, resp;
			ConsoleResponseDTO dto;
			try {
				cr.requestType = ConsoleRequest.ACTIVITY_SUMMARY_ALL;
				cr.fromDate = Tools.ConvertDateTimeToJavaMS (Tools.GetDate (Tools.NUMBER_OF_DAYS));
				cr.toDate = Tools.ConvertDateTimeToJavaMS (DateTime.Now);
				
				json = JsonConvert.SerializeObject (cr);
				encodedJSON = HttpUtility.UrlEncode (json);
				url = Tools.CONSOLE_URL + encodedJSON;
				resp = TalkToServer.getAsyncData (url);
				dto = (ConsoleResponseDTO)JsonConvert.DeserializeObject (resp, typeof(ConsoleResponseDTO));
				if (dto.responseCode > 0) {
					displayConsoleError (dto);
					return;
				}
				List<SummaryDTO> sList = dto.summaries;
				
				var cont = new SummaryController (sList);
				this.NavigationController.PushViewController (cont, true);
			} catch (System.Net.WebException ex) {
				
				btnRefresh.Enabled = true;
				Console.WriteLine ("Server unavailable: " + ex.Message);
				new UIAlertView ("Server Error", "Server not available", null, "OK").Show ();
			} catch (Exception ex) {
				Console.WriteLine ("Network unavailable: " + ex.Message);
				btnRefresh.Enabled = true;
				new UIAlertView ("Network Error", "Network not available", null, "OK").Show ();
			}
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			btnSummaryReport.Enabled = true;
		}

		private void doCompanyRequest ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;

			ConsoleRequest cr = new ConsoleRequest ();
			string json, encodedJSON, url, resp;
			ConsoleResponseDTO dto;
			try {
				cr.requestType = ConsoleRequest.COMPANY_LIST;
				json = JsonConvert.SerializeObject (cr);
				encodedJSON = HttpUtility.UrlEncode (json);
				url = Tools.CONSOLE_URL + encodedJSON;
				resp = TalkToServer.getAsyncData(url);
				dto = (ConsoleResponseDTO)JsonConvert.DeserializeObject (resp, typeof(ConsoleResponseDTO));
				if (dto.responseCode > 0) {
					displayConsoleError (dto);
					return;
				}
				list = dto.companies;			
				tableView.Delegate = new TableViewDelegate (list, this);
				tableView.DataSource = new TableViewDataSource (list);
				tableView.ReloadData ();
			} catch (System.Net.WebException ex) {
				Console.WriteLine ("Server unavailable: " + ex.Message);
				btnRefresh.Enabled = true;
				new UIAlertView ("Server Error", "Server not available", null, "OK").Show ();
			} catch (Exception ex) {
				Console.WriteLine ("Network unavailable: " + ex.Message);
				btnRefresh.Enabled = true;
				new UIAlertView ("Network Error", "Network not available", null, "OK").Show ();
			}
			
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}
		
		private void displayConsoleError (ConsoleResponseDTO resp)
		{
			new UIAlertView ("Console Error", resp.responseMessage, null, "OK").Show ();
			
		}
		
		
		private class TableViewDelegate : UITableViewDelegate
		{
			private List<CompanyDTO> list;
			private UIViewController controller;

			public TableViewDelegate (List<CompanyDTO> list, UIViewController controller)
			{
				this.list = list;
				this.controller = controller;
			}

			public override void RowSelected (
                UITableView tableView, NSIndexPath indexPath)
			{
				Console.WriteLine (
                    "TableViewDelegate.RowSelected: Label={0}",
                     list [indexPath.Row].companyName);
				UserListController cont = new UserListController (list [indexPath.Row]);
				
				controller.NavigationController.PushViewController (cont, true);
			}
		}

		private class TableViewDataSource : UITableViewDataSource
		{
			static NSString kCellIdentifier =
                new NSString ("MyIdentifier");
			private List<CompanyDTO> list;

			public TableViewDataSource (List<CompanyDTO> list)
			{
				this.list = list;
			}

			public override int RowsInSection (
                UITableView tableview, int section)
			{
				return list.Count;
			}
			
			/*
			 * public override UITableViewCell GetCell 
   (UITableView tableView, NSIndexPath indexPath)
{
   TDBadgedCell cell = new TDBadgedCell (UITableViewCellStyle.Subtitle, "Cell");
   cell.TextLabel.Text = contents[indexPath.Row].Title;
   cell.TextLabel.Font = UIFont.BoldSystemFontOfSize (14);
 
   cell.DetailTextLabel.Text = contents[indexPath.Row].Detail;
   cell.DetailTextLabel.Font = UIFont.SystemFontOfSize (13);
 
   cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
   cell.BadgeNumber = Convert.ToInt32 (contents[indexPath.Row].Badge);
 
   if (indexPath.Row == 1)
      cell.BadgeColor = UIColor.FromRGBA (1.000f, 0.397f, 0.419f, 1.000f);
   if (indexPath.Row == 2)
      cell.BadgeColor = UIColor.FromWhiteAlpha (0.783f, 1.000f);
   return cell;
}
			 */ 
			public override UITableViewCell GetCell (
                UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (kCellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellIdentifier);
					cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
				}
				
				//set up image
				
				UIImage img = UIImage.FromFile ("Images/logo_29.png");

				cell.TextLabel.Text = list [indexPath.Row].companyName;
				cell.ImageView.Image = img;
				cell.DetailTextLabel.Text = list [indexPath.Row].email;
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
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

