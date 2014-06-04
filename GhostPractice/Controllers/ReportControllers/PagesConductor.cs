using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GhostPractice
{
	public partial class PagesConductor : UIViewController
	{
		List<UIViewController> _controllers;
		//reports
		FinancialStatusReport financeReport;
		MatterAnalysisByOwnerReport matterAnalysisReport;
		FeeTargetProgressReport feeTargetReport;

		int reportType { get; set; }
		//Branch branch;
		public PagesConductor () : base ("PagesConductor", null)
		{
		}

		public PagesConductor (FinancialStatusReport financeReport, MatterAnalysisByOwnerReport matterAnalysisReport, 
		                       FeeTargetProgressReport feeTargetReport, int reportType, int page)
		{
			this.reportType = reportType;
			this.financeReport = financeReport;
			this.feeTargetReport = feeTargetReport;
			this.matterAnalysisReport = matterAnalysisReport;
			this.pageNumber = page;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			this.NavigationController.SetToolbarHidden (true, false); //-- show the bottom toolbar
			base.ViewWillAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Search", UIBarButtonItemStyle.Bordered, delegate(object sender, EventArgs e) {   
				var con = new Finder ();
				NavigationController.PushViewController (con, true);
			});
			btnBack.Title = "back Reports";
			btnBack.Clicked += delegate {
				this.NavigationController.PopViewControllerAnimated (true);
			};
			btnBack.Title = "";
			btnBack.Width = 0.1f;
			UIView v = View.Subviews [1];
			v.RemoveFromSuperview ();
			//this.NavigationController.SetNavigationBarHidden (true, true); //-- hide the navigation bar
			this.NavigationController.SetToolbarHidden (true, false); //-- show the bottom toolbar
			this.NavigationController.ToolbarHidden = true;

			//pageControl.BackgroundColor = ColorHelper.GetGPLightPurple ();
			//btnBack.TintColor = ColorHelper.GetGPPurple ();

			//---- wire up our pager and scroll view event handlers
			this.pageControl.BackgroundColor = UIColor.Cyan;
			this.pageControl.ValueChanged += HandlePgrMainValueChanged;
			this.scrollView.Scrolled += HandleScrlMainScrolled;

			this.scrollView.Frame = new RectangleF (0, 0, 320, 480);

			LoadControllers ();
			Title = "Page " + (pageNumber + 1) + " of " + this._controllers.Count;
			//setCurrentPage ();
			// Perform any additional setup after loading the view, typically from a nib.
		}


		protected void HandlePgrMainValueChanged (object sender, EventArgs e)
		{
			//---- it moves page by page. we scroll right to the next controller
			this.scrollView.ScrollRectToVisible (
				this._controllers [(sender as UIPageControl).CurrentPage].View.Frame,
				true);
		}


		protected void HandleScrlMainScrolled (object sender, EventArgs e)
		{
			//---- calculate the page number
			int pageNumber = (int)(Math.Floor (
				                 (this.scrollView.ContentOffset.X - this.scrollView.Frame.Width / 2)
				                 / this.scrollView.Frame.Width
			                 ) + 1);

			//---- if it's a valid page
			if (pageNumber >= 0 && pageNumber < this._controllers.Count) {
				//---- Set the current page on the pager control
				this.pageControl.CurrentPage = pageNumber;
				Title = "Page " + (pageNumber + 1) + " of " + this._controllers.Count;
			}
		}

		private void SetTitlePageNumber ()
		{

		}

		public const int FINANCIAL_STATUS = 1;
		public const int FEE_TARGET = 2;
		public const int MATTER_ANALYSIS = 3;

		private void setCurrentPage ()
		{
			int cnt = 0;
			switch (reportType) {
			case FINANCIAL_STATUS:
				cnt = 0;
				break;
			case FEE_TARGET:
				if (financeReport != null) {
					cnt += financeReport.branches.Count;
				}
				break;
			case MATTER_ANALYSIS:
				
				if (financeReport != null) {
					cnt += financeReport.branches.Count;
				}
				if (feeTargetReport != null) {
					cnt += 3;
				}
				break;
			}
			//---- calculate the page number
			pageNumber = cnt;

			//---- if it's a valid page
			if (pageNumber >= 0 && pageNumber < this._controllers.Count) {
				//---- Set the current page on the pager control
				//this.pageControl.CurrentPage = pageNumber - 1;
				//make scrollview show the page using pageNumber...
				float f = scrollView.Frame.Width * (pageNumber);
				PointF point = new PointF (f, 0);
				this.scrollView.SetContentOffset (point, true);
			}
			Title = "Page " + (pageNumber + 1) + " of " + this._controllers.Count;
		}

		int pageNumber;

		protected void LoadControllers ()
		{
			//---- instantiate and add the controllers to our page list
			this._controllers = new List<UIViewController> ();
			
			int pages = 0;
			if (financeReport != null) {
				for (var i = 0; i < financeReport.branches.Count; i++) {
					FinancialStatusByBranchController fsc = new FinancialStatusByBranchController (financeReport.branches [i]);
					this._controllers.Add (fsc);
					pages++;
				}
			}
			//fee target
			if (feeTargetReport != null) {
				FeeTargetProgressController ftp = new FeeTargetProgressController (feeTargetReport);
				this._controllers.Add (ftp);
				pages++;
				FeeTargetProgressBranchController ftpb = new FeeTargetProgressBranchController (feeTargetReport);
				this._controllers.Add (ftpb);
				pages++;
				FeeTargetBranchOwnerController ftpc = new FeeTargetBranchOwnerController (feeTargetReport);
				this._controllers.Add (ftpc);
				pages++;
			}
			//fee target
			if (matterAnalysisReport != null) {
				MatterAnalysisController ftp = new MatterAnalysisController (matterAnalysisReport);
				this._controllers.Add (ftp);
				pages++;
				MatterAnalysisBranchController ftpb = new MatterAnalysisBranchController (matterAnalysisReport);
				this._controllers.Add (ftpb);
				pages++;
				MatterAnalysisBranchOwnerController ftpc = new MatterAnalysisBranchOwnerController (matterAnalysisReport);
				this._controllers.Add (ftpc);
				pages++;
			}
			
			pageControl.Pages = pages;
			
			scrollView.PagingEnabled = true;
			//---- loop through each one
			for (int i = 0; i < this._controllers.Count; i++) {
				//---- set their location and size, each one is moved to the
				// right by the width of the previous
				RectangleF viewFrame = new RectangleF (
					                       this.scrollView.Frame.Width * i
                        , this.scrollView.Frame.Y
                        , this.scrollView.Frame.Width
                        , this.scrollView.Frame.Height);
				this._controllers [i].View.Frame = viewFrame;

				//---- add the view to the scrollview
				this.scrollView.AddSubview (this._controllers [i].View);
			}

			//---- set our scroll view content size (width = number of pages * scroll view
			// width)
			this.scrollView.ContentSize = new SizeF (
				this.scrollView.Frame.Width * this._controllers.Count,
				this.scrollView.Frame.Height);

			setCurrentPage ();
		}
	}
}

