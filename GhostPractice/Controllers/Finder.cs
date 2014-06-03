using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Web;
using System.Globalization;
using GhostPractice;
using GhostPracticeLibrary;

namespace GhostPractice
{
	public partial class Finder : DialogViewController
	{
		DateTime start, end;
		UISearchBar SearchBar;
		Section resultSection;
		UIBarButtonItem btnReports, btnAbout;

		public Finder () : base (UITableViewStyle.Grouped, null)
		{


			btnReports = new UIBarButtonItem ("Reports", UIBarButtonItemStyle.Plain, delegate (object sender, EventArgs e) {
				var cont = new ReportsCoordinatorDialog ();
				NavigationController.PushViewController (cont, true);
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

			UIBarButtonItem[] btns = { btnAbout, btnReports };
			this.NavigationItem.SetRightBarButtonItems (btns, true);



			BuildInterface ();
		}

		public void BuildInterface ()
		{
			Root = new RootElement (S.GetText (S.MATTER_SEARCH));
			var rect = new RectangleF (100, 200, 400, 50);
			SearchBar = new UISearchBar (rect);

			var searchSec = new Section (SearchBar);
			resultSection = new Section ("Matter Search Results");

			Root.Add (searchSec);
			Root.Add (resultSection);

			SearchBar.SearchButtonClicked += delegate {
				SearchBar.ResignFirstResponder ();
				if (isBusy) {
					new UIAlertView (S.GetText (S.SEARCH), "Searching is continuing, cannot start new search", null, "OK").Show ();
					cnt++;
					if (cnt > 1) {
						isBusy = false;
						cnt = 0;
					}
					return;
				}
				string s = SearchBar.Text.Trim ();
				if (s == "" || s.Length == 0) {
					new UIAlertView (S.GetText (S.SEARCH), "Please enter search text", null, "OK").Show ();
					return;
				} else {
					getAsyncData ();
				}
			};	

			SearchBar.BookmarkButtonClicked += delegate {
				Console.WriteLine (" search bar cancel pressed");
				SearchBar.ResignFirstResponder ();
			};
			SearchBar.CancelButtonClicked += delegate {
				Console.WriteLine (" search bar cancel pressed");
				SearchBar.ResignFirstResponder ();
			};

			string srch = NSUserDefaults.StandardUserDefaults.StringForKey ("search");
			if (srch != null && srch != "") {
				SearchBar.Text = srch;
				getAsyncData ();
			}
		}

		static List<MatterSearchResultDTO> searchResults;
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
			cr.searchString = SearchBar.Text;
			NSUserDefaults.StandardUserDefaults.SetString (cr.searchString, "search");
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.userID = NSUserDefaults.StandardUserDefaults.IntForKey ("userID");
			cr.companyID = NSUserDefaults.StandardUserDefaults.IntForKey ("companyID");
			cr.deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");

			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("MatterFind JSON = " + json);
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
		// Invoked when we get the stream back from server
		//
		void DataDownloaded (IAsyncResult result)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			isBusy = false;
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			searchResults = new List<MatterSearchResultDTO> ();
			try {
				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (result);
				Stream receiveStream = response.GetResponseStream ();
				StreamReader readStream = new StreamReader (receiveStream);
				string resp = readStream.ReadToEnd ();
				Console.WriteLine ("## MatterFind Response stream received.\n" + resp);
				response.Close ();
				readStream.Close ();

				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (resp, typeof(WebServiceResponseDTO));

				end = DateTime.Now;	
				Tools.SendElapsedTime (start, end, dto.activityID);

				InvokeOnMainThread (delegate {
					if (dto.responseCode == 0) {
						searchResults = dto.matterSearchList;
						if (searchResults != null && searchResults.Count == 0) {
							resultSection.Clear ();
							NSUserDefaults.StandardUserDefaults.SetString ("", "search");
							new UIAlertView ("Search Result", "No matters found for this search", null, "OK").Show ();	
						} else {
							//new UIAlertView ("Search Result", "Matters found for this search: " + searchResults.Count, null, "OK").Show ();	
						}
					} else {
						new UIAlertView ("Search Result", dto.responseMessage + "\nStatus Code: " + dto.responseCode, null, "OK").Show ();
						return;
					}
					if (searchResults.Count > 0) {
						Console.WriteLine ("......found results: " + searchResults.Count);
						resultSection.Clear ();
						UIImage img = UIImage.FromFile ("Images/matter.png");
						foreach (var item in searchResults) {
							var x = new CustomImageStringElement (item.matterName, img);
							resultSection.Add (x);
							x.Tapped += delegate {
								Console.WriteLine ("...selected - " + item.matterName);
								x.SetBackgroundColor (UIColor.Cyan);
								MatterDetailsDialog diag = new MatterDetailsDialog (item);
								this.NavigationController.PushViewController (diag, true);

								//TODO - loop thru result section and change the other backgrounds to white
							
							};
						}
					}

				});



			} catch (Exception ex) {
				Console.WriteLine ("### ERROR: " + ex.Message);
				InvokeOnMainThread (delegate {
					isBusy = false;
					new UIAlertView ("Network Error", "Problem communicating with server.\nPlease try again or call GhostPractice Support", null, "Close").Show ();
				}
				);

			}
		}

		public event EventHandler<RowClickedEventArgs> RowClicked;

		public class RowClickedEventArgs : EventArgs
		{
			public MatterSearchResultDTO Item { get; set; }

			public RowClickedEventArgs (MatterSearchResultDTO item) : base ()
			{
				this.Item = item;
			}
		}
	}
}
