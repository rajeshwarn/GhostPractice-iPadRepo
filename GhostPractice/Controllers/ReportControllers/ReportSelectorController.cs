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
	public partial class ReportSelectorController : DialogViewController
	{
		StyledStringElement btnFinance, btnFeeTarget, btnMatterAnalysis;
		FinancialStatusReport financeReport;
		MatterAnalysisByOwnerReport matterAnalysisReport;
		FeeTargetProgressReport feeTargetReport;
		public const int FINANCIAL_STATUS = 1;
		public const int FEE_TARGET = 2;
		public const int MATTER_ANALYSIS = 3;
		public int reportType;
		DateTime start, end, financeTime, feeTargetTime, matterAnalysisTime;

		public PagedViewController pageController { get; set; }

		private ReportsDataSource reportSource;
		//		UIBarButtonItem btnClose;
		//		bool isStartingUp;
		//
		public ReportSelectorController (PagedViewController pageController, bool getFinanceReport) : base (UITableViewStyle.Grouped, null)
		{
			this.pageController = pageController;
			reportSource = (ReportsDataSource)pageController.PagedViewDataSource;
			if (reportSource == null) {
				reportSource = new ReportsDataSource ();
			}
			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Bordered, delegate(object sender, EventArgs e) {   
				this.NavigationController.PopViewControllerAnimated (true);
			});
			Root = new RootElement ("");

			Autorotate = true;
			BuildInterface ();
			//get finance report upfront
			if (getFinanceReport) {
				reportType = FINANCIAL_STATUS;
				getAsyncReportData ();
			}



		}

		public void BuildInterface ()
		{
			Root.Clear ();

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
			Root.Add (new Section (" "));
			addBusySection ();
			//
			Root.Add (new Section (NSUserDefaults.StandardUserDefaults.StringForKey ("userName")));			
			var getReportsSection = new Section ("Report Selection");			
			
			btnFinance = 
				new StyledStringElement ("Financial Status Report",
				null,
				UITableViewCellStyle.Subtitle) {
				DetailColor = UIColor.Gray
				//Accessory = UITableViewCellAccessory.DetailDisclosureButton
			};
			btnFinance.Tapped += delegate() {
				CheckIfBusy (FINANCIAL_STATUS);
			};
			btnFinance.AccessoryTapped += delegate() {
				CheckIfBusy (FINANCIAL_STATUS);				
			};
			getReportsSection.Add (btnFinance);
			//getReportsSection.Add (new StringElement (" "));
			//
			btnFeeTarget = 
				new StyledStringElement ("Fee Target Progress Report",
				null,
				UITableViewCellStyle.Subtitle) {
				DetailColor = UIColor.Gray
				//Accessory = UITableViewCellAccessory.DetailDisclosureButton
			};

			btnFeeTarget.Tapped += delegate() {				
				CheckIfBusy (FEE_TARGET);
			};
			btnFeeTarget.AccessoryTapped += delegate() {
				CheckIfBusy (FEE_TARGET);
			};
			getReportsSection.Add (btnFeeTarget);
			//getReportsSection.Add (new StringElement (" "));
			//
			btnMatterAnalysis = new StyledStringElement ("Matter Analysis Report");
			btnMatterAnalysis = 
				new StyledStringElement ("Matter Analysis Report",
				null,
				UITableViewCellStyle.Subtitle) {
				DetailColor = UIColor.Gray
				//Accessory = UITableViewCellAccessory.DetailDisclosureButton
			};
			btnMatterAnalysis.Tapped += delegate() {
				CheckIfBusy (MATTER_ANALYSIS);
			};
			btnMatterAnalysis.AccessoryTapped += delegate() {
				CheckIfBusy (MATTER_ANALYSIS);
			};
			getReportsSection.Add (btnMatterAnalysis);
			//getReportsSection.Add (new StringElement (" "));
			//
			Root.Add (getReportsSection);
			addImageSection ();
		
			
			
		}

		void addImageSection ()
		{
			var img = new UIImage ("Images/launch_small.png");
			var v = new UIImageView (new RectangleF (10f, 10f, 280f, 400f));
			v.Image = img;
			var sec = new Section (v);
			Root.Add (sec);
		}

		void addBusySection ()
		{
			if (isBusy) {
				var lv = new UIActivityIndicatorView (new RectangleF (10f, 10f, 20f, 20f));
				lv.Color = ColorHelper.GetGPLightPurple ();
				lv.StartAnimating ();
				var sv = new Section (lv);
				Root.Add (sv);
			}
		}

		bool isBusy;

		private void navigateToReport (int type)
		{
			Console.WriteLine ("ReportSelectorController - navigateToReport()...type: " + type);
			if (feeTargetReport != null)
				reportSource.setFeeTargetReportData (feeTargetReport);
			if (financeReport != null)
				reportSource.setFinanceReportData (financeReport);
			if (matterAnalysisReport != null)
				reportSource.setMatterAnalysisReportData (matterAnalysisReport);
			//
			pageController.PagedViewDataSource = reportSource;
			pageController.ReloadPages ();

			switch (type) {
			case FINANCIAL_STATUS:
				Console.WriteLine ("--- navigate to Financial Status page, pages: " + pageController.NumberOfPages);
				pageController.Page = 0;
				break;
			case FEE_TARGET:
				Console.WriteLine ("--- navigate to Fee Target page, pages: " + pageController.NumberOfPages);
				pageController.Page = 2;
				break;
			case MATTER_ANALYSIS:
				Console.WriteLine ("--- navigate to Matter Analysis page, pages: " + pageController.NumberOfPages);
				switch (pageController.Page) {
				case 0:
					pageController.Page = 2;
					break;
				case 1:
					pageController.Page = 2;
					break;
				case 2:
					pageController.Page = 5;
					break;
				case 3:
					pageController.Page = 5;
					break;
				case 4:
					pageController.Page = 5;
					break;
				case 5:
					pageController.Page = 5;
					break;
				case 6:
					pageController.Page = 5;
					break;
				case 7:
					pageController.Page = 5;
					break;
				}

				break;
			}
			this.NavigationController.PushViewController (pageController, true);
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
				Console.WriteLine ("Funancial - ElapsedSpan seconds: " + elapsedSpan.TotalSeconds);
				if (elapsedSpan.TotalSeconds < (60 * 5)) {
					if (financeReport != null) {
						navigateToReport (type);
						return;
					}					
				}
				break;
			case FEE_TARGET:
				ticks = DateTime.Now.Ticks - feeTargetTime.Ticks;
				elapsedSpan = new TimeSpan (ticks);
				if (elapsedSpan.TotalSeconds < (60 * 5)) {
					if (feeTargetReport != null) {
						navigateToReport (type);
						return;
					}					
				}
				break;
			case MATTER_ANALYSIS:
				ticks = DateTime.Now.Ticks - matterAnalysisTime.Ticks;
				elapsedSpan = new TimeSpan (ticks);
				if (elapsedSpan.TotalSeconds < (60 * 5)) {
					if (matterAnalysisReport != null) {
						navigateToReport (type);
						return;
					}					
				}
				break;
			}
			//check if comms busy
			if (isBusy) {
//				new UIAlertView ("Busy", "Report request service is busy. Please wait until it completes.",
//				                 null, "OK").Show ();
				Console.WriteLine ("### Busy now.... fuck off!");
			} else {
				reportType = type;
				getAsyncReportData ();
			}	
			
			
		}

		public void getAsyncReportData ()
		{			
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			isBusy = true;
			BuildInterface ();
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
			Console.WriteLine ("Async JSON = " + json);
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
				Console.WriteLine ("## ASYNCResponse stream received.\n" + resp);
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
					BuildInterface ();
					if (dto.responseCode > 0) {	
						Console.WriteLine ("###  ------ Report selector response code: " + dto.responseCode);
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
						isBusy = false;
						Console.WriteLine ("### ------ about to navigateToReport"); 


						break;
						
					}
					navigateToReport (FINANCIAL_STATUS);

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

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return false;
		}
	}
}
