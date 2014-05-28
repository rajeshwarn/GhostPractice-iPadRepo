using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Web;

namespace GhostAdmin
{
	public partial class LogsController : UIViewController
	{
		static List<ActivityLogDTO> list;
		static NSString cellIdentifier = new NSString ("CellId");
		UserDTO user;

		public LogsController () : base ("LogsController", null)
		{
		}

		public LogsController (UserDTO user)
		{
			this.user = user;
			Title = user.userName;
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
			
			btnSummary.Clicked += delegate {
				//get summary from server
				getSummary ();
			};			
						
			btnLogs.Clicked += delegate {
				new UIAlertView ("Activity Logs", "There are " + list.Count + " activities performed on GhostPractice Mobile in the last 30 days", null, "Done").Show ();
			};
			
			//get logs from server
			getLogs ();
		}

		private void getLogs ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			ConsoleRequest cr = new ConsoleRequest ();
			string json, encodedJSON, url, resp;
			ConsoleResponseDTO dto;
			try {
				cr.requestType = ConsoleRequest.ACTIVITY_LOGS_BY_USER;
				cr.userID = user.userID;
				cr.fromDate = Tools.ConvertDateTimeToJavaMS (Tools.GetDate (Tools.NUMBER_OF_DAYS));
				cr.toDate = Tools.ConvertDateTimeToJavaMS (DateTime.Now);
				json = JsonConvert.SerializeObject (cr);
				encodedJSON = HttpUtility.UrlEncode (json);
				url = Tools.CONSOLE_URL + encodedJSON;
				resp = TalkToServer.getData (url);
				dto = (ConsoleResponseDTO)JsonConvert.DeserializeObject (resp, typeof(ConsoleResponseDTO));
				list = dto.activityLogs;
			
				if (list == null || dto.responseCode > 0) {
					list = new List<ActivityLogDTO> ();
					new UIAlertView ("Activity Logs", "No activity records were found", null, "OK").Show ();
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
					this.NavigationController.PopViewControllerAnimated(true);
				} else {
					btnLogs.Title = "Activities: " + list.Count.ToString ();
					tableView.Delegate = new TableViewDelegate (list, this);
					tableView.DataSource = new TableViewDataSource (list);
					tableView.ReloadData ();
					if (list.Count == 0) {
						new UIAlertView ("Activity Logs", "No activity records were found", null, "OK").Show ();
						UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
						this.NavigationController.PopViewControllerAnimated(true);
					}
				}
			} catch (System.Net.WebException ex) {
				
				Console.WriteLine ("Server unavailable: " + ex.Message);
				new UIAlertView ("Server Error", "Server not available", null, "OK").Show ();
			} catch (Exception ex) {
				Console.WriteLine ("Network unavailable: " + ex.Message);
				new UIAlertView ("Network Error", "Network not available", null, "OK").Show ();
			}
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		private void getSummary ()
		{
			ConsoleRequest cr = new ConsoleRequest ();
			string json, encodedJSON, url, resp;
			ConsoleResponseDTO dto;
			try {
				cr.requestType = ConsoleRequest.ACTIVITY_SUMMARY_USER;
				cr.fromDate = Tools.ConvertDateTimeToJavaMS (Tools.GetDate (Tools.NUMBER_OF_DAYS));
				cr.toDate = Tools.ConvertDateTimeToJavaMS (DateTime.Now);
				cr.userID = user.userID;
				json = JsonConvert.SerializeObject (cr);
				encodedJSON = HttpUtility.UrlEncode (json);
				url = Tools.CONSOLE_URL + encodedJSON;
				resp = TalkToServer.getData (url);
				dto = (ConsoleResponseDTO)JsonConvert.DeserializeObject (resp, typeof(ConsoleResponseDTO));
				List<SummaryDTO> sList = dto.summaries;
				//start summary view
				var cont = new SummaryController (sList);
				this.NavigationController.PushViewController (cont, true);
			} catch (System.Net.WebException ex) {
				
				Console.WriteLine ("Server unavailable: " + ex.Message);
				new UIAlertView ("Server Error", "Server not available", null, "OK").Show ();
			} catch (Exception ex) {
				Console.WriteLine ("Network unavailable: " + ex.Message);
				new UIAlertView ("Network Error", "Network not available", null, "OK").Show ();
			}
		}
		private class TableViewDelegate : UITableViewDelegate
		{
			private List<ActivityLogDTO> list;
			//private UIViewController controller;

			public TableViewDelegate (List<ActivityLogDTO> list, UIViewController controller)
			{
				this.list = list;
				//this.controller = controller;
			}

			public override void RowSelected (
                UITableView tableView, NSIndexPath indexPath)
			{
				Console.WriteLine (
                    "TableViewDelegate.RowSelected: Label={0}",
                     list [indexPath.Row].activityType.activityTypeName);
			}
		}

		private class TableViewDataSource : UITableViewDataSource
		{
			static NSString kCellIdentifier =
                new NSString ("MyIdentifier");
			private List<ActivityLogDTO> list;

			public TableViewDataSource (List<ActivityLogDTO> list)
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
					cell.Accessory = UITableViewCellAccessory.None;
					
				}
				
				UIImage img = UIImage.FromFile ("Images/258-checkmark.png");
				cell.ImageView.Image = img;
				
				ActivityLogDTO log = list [indexPath.Row];
				cell.TextLabel.Text = log.activityType.activityTypeName;
								
				DateTime originDate = Tools.ConvertJavaMiliSecondToDateTime (log.dateStamp);		
				cell.DetailTextLabel.Text = originDate.ToLongDateString () + " " + originDate.ToLongTimeString ();
				cell.DetailTextLabel.TextColor = UIColor.Blue;
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

