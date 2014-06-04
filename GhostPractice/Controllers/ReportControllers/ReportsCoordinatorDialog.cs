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

namespace GhostPractice
{
	public partial class ReportsCoordinatorDialog : DialogViewController
	{
		StyledStringElement btnFinance, btnFeeTarget, btnMatterAnalysis, btnBackToMatterSearch;
		FinancialStatusReport financeReport;
		MatterAnalysisByOwnerReport matterAnalysisReport;
		FeeTargetProgressReport feeTargetReport;
		public const int FINANCIAL_STATUS = 1;
		public const int FEE_TARGET = 2;
		public const int MATTER_ANALYSIS = 3;
		public int reportType;
		DateTime start, end, financeTime, feeTargetTime, matterAnalysisTime;

		public ReportsCoordinatorDialog () : base (UITableViewStyle.Grouped, null, true)
		{
			//this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Bordered, delegate(object sender, EventArgs e) {   
			//	NavigationController.PopViewControllerAnimated (true);
			//});
			BuildInterface ();
		}
		//
		public void BuildInterface ()
		{
			
			Root = new RootElement ("Reports");
			var headerLabel = new UILabel (new RectangleF (10, 10, 300, 40)) {
				Font = UIFont.BoldSystemFontOfSize (20),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = "Practice Reports"
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 10, 300, 40); 
			view.Add (headerLabel);
			var topSection = new Section (view);
			Root.Add (topSection);	
			
			Root.Add (new Section (NSUserDefaults.StandardUserDefaults.StringForKey ("userName")));			
			var getReportsSection = new Section ("Request Reports");			
			
			btnFinance = 
			new StyledStringElement ("Financial Status Report",
				null,
				UITableViewCellStyle.Subtitle) {
				DetailColor = UIColor.Blue
			};
			btnFinance.Tapped += delegate() {
				CheckIfBusy (FINANCIAL_STATUS);
			};
			btnFinance.AccessoryTapped += delegate() {
				CheckIfBusy (FINANCIAL_STATUS);				
			};
			getReportsSection.Add (btnFinance);
			//
			btnFeeTarget = 
			new StyledStringElement ("Fee Target Progress Report",
				null,
				UITableViewCellStyle.Subtitle) {
				DetailColor = UIColor.Gray
			};
			btnFeeTarget.Tapped += delegate() {				
				CheckIfBusy (FEE_TARGET);
			};
			btnFeeTarget.AccessoryTapped += delegate() {
				CheckIfBusy (FEE_TARGET);
			};
			getReportsSection.Add (btnFeeTarget);
			//
			btnMatterAnalysis = new StyledStringElement ("Matter Analysis Report");
			btnMatterAnalysis = 
			new StyledStringElement ("Matter Analysis Report",
				null,
				UITableViewCellStyle.Subtitle) {
				DetailColor = UIColor.Gray
			};
			btnMatterAnalysis.Tapped += delegate() {
				CheckIfBusy (MATTER_ANALYSIS);
			};
			btnMatterAnalysis.AccessoryTapped += delegate() {
				CheckIfBusy (MATTER_ANALYSIS);
			};
			getReportsSection.Add (btnMatterAnalysis);

			
			Root.Add (getReportsSection);
			Root.Add (new Section ("  "));

				
		}

		bool isBusy;

		private void navigateToReport ()
		{
			int page = 0;
			switch (reportType) {
			case FINANCIAL_STATUS:
				page = 1;
				break;
			case FEE_TARGET:
				if (financeReport == null) {
					page = 1;
				} else {
					page = 3;
				}
				break;
			case MATTER_ANALYSIS:
				if (financeReport == null && feeTargetReport == null) {
					page = 1;
				} else {
					if (financeReport == null && feeTargetReport != null) {
						page = 3;
					} else {
						page = 6;
					}
				}
				break;
			}
			PagesConductor cont = new PagesConductor (
				                      financeReport,
				                      matterAnalysisReport,
				                      feeTargetReport,
				                      reportType, 
				                      page
			                      );				
			this.NavigationController.PushViewController (cont, true);
		}

		private void CheckIfBusy (int type)
		{
			//check if report already available
			long ticks = 0;
			TimeSpan elapsedSpan;
			switch (type) {
			case FINANCIAL_STATUS:
				ticks = DateTime.Now.Ticks - financeTime.Ticks;
				elapsedSpan = new TimeSpan (ticks);
				if (elapsedSpan.TotalSeconds < (60 * 5)) {
					if (financeReport != null) {
						reportType = type;
						navigateToReport ();
						return;
					}					
				}
				break;
			case FEE_TARGET:
				ticks = DateTime.Now.Ticks - feeTargetTime.Ticks;
				elapsedSpan = new TimeSpan (ticks);
				if (elapsedSpan.TotalSeconds < (60 * 5)) {
					if (feeTargetReport != null) {
						reportType = type;
						navigateToReport ();
						return;
					}					
				}
				break;
			case MATTER_ANALYSIS:
				ticks = DateTime.Now.Ticks - matterAnalysisTime.Ticks;
				elapsedSpan = new TimeSpan (ticks);
				if (elapsedSpan.TotalSeconds < (60 * 5)) {
					if (matterAnalysisReport != null) {
						reportType = type;
						navigateToReport ();
						return;
					}					
				}
				break;
			}
			//check if comms busy
			if (isBusy) {
				new UIAlertView ("Busy", "Report request service is busy. Please wait until it completes.",
					null, "OK").Show ();
				Console.WriteLine ("### Busy now..fuck off!");
			} else {
				reportType = type;
				getAsyncReportData ();
			}	
			
			
		}

		public void getAsyncReportData ()
		{			
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			isBusy = true;
			start = DateTime.Now;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
				
			cr.requestType = GhostRequestDTO.GET_REPORT;
			cr.reportType = reportType;
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.userID = NSUserDefaults.StandardUserDefaults.IntForKey ("userID");
			cr.companyID = NSUserDefaults.StandardUserDefaults.IntForKey ("companyID");
			cr.deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("##GetReport JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;
			
			var request = (HttpWebRequest)WebRequest.Create (url);
			request.BeginGetResponse (DataDownloaded, request);
			
		}

		void DataDownloaded (IAsyncResult result)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			isBusy = false;
			end = DateTime.Now;
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			try {
				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (result);
				Stream receiveStream = response.GetResponseStream ();
				StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
				string resp = readStream.ReadToEnd ();
				Console.WriteLine ("## GetReport Response stream received.\n" + resp);
				response.Close ();
				readStream.Close ();
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (
					resp,
					typeof(WebServiceResponseDTO)
				);
				if (dto != null) {
					Tools.SendElapsedTime (start, end, dto.activityID);
				}
				InvokeOnMainThread (delegate {
					if (dto.responseCode > 0) {						
						new UIAlertView ("Report Request", dto.responseMessage, null, "OK").Show ();
						return;
					}

					switch (reportType) {
					case FINANCIAL_STATUS:
						financeReport = dto.financialStatusReport;
						financeTime = DateTime.Now;
						break;
					case FEE_TARGET:
						feeTargetReport = dto.feeTargetProgressReport;
						feeTargetTime = DateTime.Now;
						break;
					case MATTER_ANALYSIS:
						matterAnalysisReport = dto.matterAnalysisByOwnerReport;
						matterAnalysisTime = DateTime.Now;
						break;
						
					}
					int xpage = 0;
					switch (reportType) {
					case FINANCIAL_STATUS:
						xpage = 0;
						break;
					case FEE_TARGET:
						xpage = 2;
						break;
					case MATTER_ANALYSIS:
						switch (xpage) {
						case 0:
							xpage = 2;
							break;
						case 1:
							xpage = 2;
							break;
						case 2:
							xpage = 5;
							break;
						case 3:
							xpage = 5;
							break;
						case 4:
							xpage = 5;
							break;
						case 5:
							xpage = 5;
							break;
						case 6:
							xpage = 5;
							break;
						case 7:
							xpage = 5;
							break;
						}

						break;
					}
					
					PagesConductor cont = new PagesConductor (
						                      financeReport,
						                      matterAnalysisReport,
						                      feeTargetReport,
						                      reportType, xpage
					                      );				
					this.NavigationController.PushViewController (cont, true);

					isBusy = false;
				}
				);
				
			} catch (Exception ex) {
				Console.WriteLine ("shit " + ex.ToString ());
				isBusy = false;
				InvokeOnMainThread (delegate {
					isBusy = false;
					new UIAlertView ("Network Error", "Problem communicating with server, \n\nCheck your network connections and try again later", null, "Close"
					).Show ();
				
				}
				);
			}
		}
		//		public override void LoadView ()
		//	    {
		//	        base.LoadView ();
		//	        TableView.BackgroundColor = UIColor.Clear;
		//	        UIImage background = UIImage.FromFile ("Images/iphone.png");
		//	        ParentViewController.View.BackgroundColor = UIColor.FromPatternImage (background);
		//	    }
	}
}
