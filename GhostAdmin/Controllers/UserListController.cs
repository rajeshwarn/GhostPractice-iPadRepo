using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Web;

namespace GhostAdmin
{
	public partial class UserListController : UIViewController
	{
		static List<UserDTO> list;
		static NSString cellIdentifier = new NSString ("CellId");
		CompanyDTO company;

		public UserListController () : base ("UserListController", null)
		{
		}
		
		public UserListController (CompanyDTO company)
		{
			this.company = company;
			Title = company.companyName;
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
			
			btnSummaryReport.Clicked += delegate {
				doCompanySummary ();
			};
			
			
			btnActivityLogs.Title = "Platforms";
			btnActivityLogs.Clicked += delegate {				
				doPlatformSummary ();				
			};
			
			doUserList ();
		}
		
		private void doPlatformSummary ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			btnActivityLogs.Enabled = false;
			ConsoleRequest cr = new ConsoleRequest ();
			string json, encodedJSON, url, resp;
			ConsoleResponseDTO dto;
			try {
				cr.requestType = ConsoleRequest.PLATFORM_SUMMARY_COMPANY;
				cr.appID = 1;
				cr.companyID = company.companyID;
				
				json = JsonConvert.SerializeObject (cr);
				encodedJSON = HttpUtility.UrlEncode (json);
				url = Tools.CONSOLE_URL + encodedJSON;
				resp = TalkToServer.getData (url);
				dto = (ConsoleResponseDTO)JsonConvert.DeserializeObject (resp, typeof(ConsoleResponseDTO));
				List<PlatformSummaryDTO> sList = dto.platformSummaries;
				
				
				
				var contx = new PlatformController (sList, company.companyID);
				this.NavigationController.PushViewController (contx, true);
			} catch (System.Net.WebException ex) {
				Console.WriteLine ("Server unavailable: " + ex.Message);
				new UIAlertView ("Server Error", "Server not available", null, "OK").Show ();
			} catch (Exception ex) {
				Console.WriteLine ("Network unavailable: " + ex.Message);
				new UIAlertView ("Network Error", "Network not available", null, "OK").Show ();
			}
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			btnActivityLogs.Enabled = true;
		}

		private void doUserList ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			ConsoleRequest cr = new ConsoleRequest ();
			string json, encodedJSON, url, resp;
			ConsoleResponseDTO dto;
			try {
				cr.requestType = ConsoleRequest.COMPANY_USER_LIST;
				cr.companyID = company.companyID;
				json = JsonConvert.SerializeObject (cr);
				encodedJSON = HttpUtility.UrlEncode (json);
				url = Tools.CONSOLE_URL + encodedJSON;
				resp = TalkToServer.getData (url);
				dto = (ConsoleResponseDTO)JsonConvert.DeserializeObject (resp, typeof(ConsoleResponseDTO));
				list = dto.users;
			
				tableView.Delegate = new TableViewDelegate (list, this);
				tableView.DataSource = new TableViewDataSource (list);
				tableView.ReloadData ();
				btnCount.Title = "" + list.Count;
			} catch (System.Net.WebException ex) {
				Console.WriteLine ("Server unavailable: " + ex.Message);
				new UIAlertView ("Server Error", "Server not available", null, "OK").Show ();
			} catch (Exception ex) {
				Console.WriteLine ("Network unavailable: " + ex.Message);
				new UIAlertView ("Network Error", "Network not available", null, "OK").Show ();
			}
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		private void doCompanySummary ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			btnSummaryReport.Enabled = false;
			ConsoleRequest cr = new ConsoleRequest ();
			string json, encodedJSON, url, resp;
			ConsoleResponseDTO dto;
			try {
				cr.requestType = ConsoleRequest.ACTIVITY_SUMMARY_COMPANY;
				cr.fromDate = Tools.ConvertDateTimeToJavaMS (Tools.GetDate (Tools.NUMBER_OF_DAYS));
				cr.toDate = Tools.ConvertDateTimeToJavaMS (DateTime.Now);
				cr.companyID = company.companyID;
				json = JsonConvert.SerializeObject (cr);
				encodedJSON = HttpUtility.UrlEncode (json);
				url = Tools.CONSOLE_URL + encodedJSON;
				resp = TalkToServer.getData (url);
				dto = (ConsoleResponseDTO)JsonConvert.DeserializeObject (resp, typeof(ConsoleResponseDTO));
				List<SummaryDTO> sList = dto.summaries;
				
				var cont = new SummaryController (sList);
				this.NavigationController.PushViewController (cont, true);
			} catch (System.Net.WebException ex) {
				
				Console.WriteLine ("Server unavailable: " + ex.Message);
				new UIAlertView ("Server Error", "Server not available", null, "OK").Show ();
			} catch (Exception ex) {
				Console.WriteLine ("Network unavailable: " + ex.Message);
				new UIAlertView ("Network Error", "Network not available", null, "OK").Show ();
			}
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			btnSummaryReport.Enabled = true;
		}
		private class TableViewDelegate : UITableViewDelegate
		{
			private List<UserDTO> list;
			private UIViewController controller;

			public TableViewDelegate (List<UserDTO> list, UIViewController controller)
			{
				this.list = list;
				this.controller = controller;
			}

			public override void RowSelected (
                UITableView tableView, NSIndexPath indexPath)
			{
				Console.WriteLine (
                    "TableViewDelegate.RowSelected: Label={0}",
                     list [indexPath.Row].userName);
				LogsController cont = new LogsController (list [indexPath.Row]);				
				controller.NavigationController.PushViewController (cont, true);
			}
		}

		private class TableViewDataSource : UITableViewDataSource
		{
			static NSString kCellIdentifier =
                new NSString ("MyIdentifier");
			private List<UserDTO> list;

			public TableViewDataSource (List<UserDTO> list)
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
				
				UIImage img = UIImage.FromFile ("Images/253-person.png");

				cell.TextLabel.Text = list [indexPath.Row].userName;
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

