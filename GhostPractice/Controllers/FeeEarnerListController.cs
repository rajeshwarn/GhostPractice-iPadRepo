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
	public partial class FeeEarnerListController : UIViewController
	{
		static MatterSearchResultDTO matter;
		public FeeEarnerListController (MatterSearchResultDTO m) : base ("FeeEarnerListController", null)
		{
			matter = m;
		}
		static NSString cellIdentifier = new NSString ("CellIdFEList");
		static List<MobileUser> list;
		DateTime start, end;
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			barButtonClose.TintColor = ColorHelper.GetGPPurple ();
			barButtonClose.Clicked += delegate {
				this.NavigationController.PopViewControllerAnimated (true);
			};
			tableView.RowHeight = 50;
			getAsyncData ();
		}


		//
		// Asynchronous HTTP request
		//
		public void getAsyncData ()
		{
			start = DateTime.Now;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
			
			cr.requestType = GhostRequestDTO.GET_FEE_EARNERS;
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
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			list = new List<MobileUser> ();
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
						list = dto.mobileUsers;
						if (list != null && list.Count == 0) {
							//btnCount.Title = "Matters: " + 0;
							new UIAlertView ("Search Result", "No fee earners were found", null, "OK").Show ();	
						}
					} else {
						new UIAlertView ("Search Result", dto.responseMessage, null, "OK").Show ();
					}
					//btnCount.Title = "Matters: " + list.Count;
					//btnCount.TintColor = UIColor.White;
					//
					tableView.Delegate = new TableViewDelegate (list, this);
					tableView.DataSource = new TableViewDataSource (list);
					tableView.ReloadData ();
				}
				);
				
				
				
			} catch (Exception ex) {
				Console.WriteLine ("### ERROR: " + ex.Message);
				InvokeOnMainThread (delegate {
					new UIAlertView ("Network Error", "Problem communicating with server.\nPlease try later or call GhostPractice Support", null, "Close").Show ();
				}
				);
				
			}
		}
		private class TableViewDelegate : UITableViewDelegate
		{
			private List<MobileUser> list;
			private UIViewController controller;
			
			public TableViewDelegate (List<MobileUser> list, UIViewController controller)
			{
				this.list = list;
				this.controller = controller;
			}
			
			public override void RowSelected (
				UITableView tableView, NSIndexPath indexPath)
			{

				FeeEarnerTaskController cont = new FeeEarnerTaskController (matter, list [indexPath.Row]);
				controller.NavigationController.PushViewController (cont, true);
			}
			public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
			{
				Console.WriteLine ("## Accessory tapped: " + list [indexPath.Row].firstNames);
				FeeEarnerTaskController cont = new FeeEarnerTaskController (matter, list [indexPath.Row]);
				controller.NavigationController.PushViewController (cont, true);
			}
		}
		
		private class TableViewDataSource : UITableViewDataSource
		{
			static NSString kCellIdentifier =
				new NSString ("MyIdentifier");
			private List<MobileUser> list;
			
			public TableViewDataSource (List<MobileUser> list)
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
				
				//set up image find user image....
				
				UIImage img = UIImage.FromFile ("Images/user_library.png");
				
				cell.TextLabel.Text = list [indexPath.Row].firstNames + " " + list [indexPath.Row].lastName;
				cell.ImageView.Image = img;
				cell.DetailTextLabel.Text = list [indexPath.Row].userName;
				
				
				return cell;
			}
		}

		

	}
}

