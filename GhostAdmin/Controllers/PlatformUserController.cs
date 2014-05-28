using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Web;

namespace GhostAdmin
{
	public partial class PlatformUserController : UIViewController
	{
		static List<UserDTO> list;
		static NSString cellIdentifier = new NSString ("CellId");
		static PlatformDTO platform;
		static int companyID;

		public PlatformUserController () : base ("PlatformUserController", null)
		{
		}

		public PlatformUserController (PlatformDTO p, int coID)
		{
			platform = p;
			this.Title = platform.platformName;
			companyID = coID;
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
			tableView.RowHeight = 60;
			
			btnEmail.Clicked += delegate {
				new UIAlertView ("eMail Broadcast", "Send eMail to list", null, "OK").Show ();
			};
			btnSMS.Clicked += delegate {
				new UIAlertView ("SMS Broadcast", "Send SMS to list", null, "OK").Show ();
			};
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;

			ConsoleRequest cr = new ConsoleRequest ();
			string json, encodedJSON, url, resp;
			ConsoleResponseDTO dto;
			try {
				if (companyID == 0) {
					cr.requestType = ConsoleRequest.PLATFORM_USERS_ALL;					
				} else {
					cr.requestType = ConsoleRequest.PLATFORM_USERS_COMPANY;
					cr.companyID = companyID;
				}
				cr.platformID = platform.platformID;
				json = JsonConvert.SerializeObject (cr);
				encodedJSON = HttpUtility.UrlEncode (json);
				url = Tools.CONSOLE_URL + encodedJSON;
				resp = TalkToServer.getData (url);
				dto = (ConsoleResponseDTO)JsonConvert.DeserializeObject (resp, typeof(ConsoleResponseDTO));
				
				if (dto.users.Count == 0) {
					btncount.Title = " Users: " + 0;
					//new UIAlertView ("No Data", "No users are provisioned on " + platform.platformName, null, "OK").Show ();
				} else {
					list = dto.users;
					btncount.Title = " Users: " + list.Count;
					tableView.Delegate = new TableViewDelegate (list, this);
					tableView.DataSource = new TableViewDataSource (list);
					tableView.ReloadData ();
					
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
		private class TableViewDelegate : UITableViewDelegate
		{
			private List<UserDTO> list;
			static private UIViewController controller;

			public TableViewDelegate (List<UserDTO> list, UIViewController c)
			{
				this.list = list;
				controller = c;
			}

			public override void RowSelected (
                UITableView tableView, NSIndexPath indexPath)
			{
				UserDTO user = list [indexPath.Row];
				var actionSheet = new UIActionSheet (user.userName, null, "eMail", "SMS", null);
				actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
					if (b.ButtonIndex == 0) {
						new UIAlertView("SMS", "Sending sms text ", null, "Send SMS").Show();
					} else {
						new UIAlertView("eMail", "Sending email text ", null, "Send eMail").Show();	
					}
					
				};
				actionSheet.ShowInView(controller.View);

				
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
					cell.Accessory = UITableViewCellAccessory.Checkmark;
				}
				
				//set up image
				
				UIImage img = UIImage.FromFile ("Images/253-person.png");
				//UIImage img = UIImage.FromFile ("Images/32-iphone.png");
				if (platform.platformName.Contains ("iPhone") || platform.platformName.Contains ("iPad")) {
					img = UIImage.FromFile ("Images/apple48-blue.png");	
				}
				if (platform.platformName.Contains ("Android")) {
					img = UIImage.FromFile ("Images/android48-round.png");	
				}
				if (platform.platformName.Contains ("Blackberry")) {
					img = UIImage.FromFile ("Images/blackberry.png");	
				}
				if (platform.platformName.Contains ("Windows")) {
					img = UIImage.FromFile ("Images/windows.png");	
				}

				cell.TextLabel.Text = list [indexPath.Row].userName;
				cell.ImageView.Image = img;
				if (companyID == 0) {
					cell.DetailTextLabel.Text = list [indexPath.Row].company.companyName;
				} else {
					cell.DetailTextLabel.Text = list [indexPath.Row].email;
				}
				//cell.DetailTextLabel.TextColor = UIColor.Red;
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

