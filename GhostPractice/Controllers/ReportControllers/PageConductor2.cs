using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using XibFree;
using System.Collections.Generic;
using MonoTouch.CoreAnimation;

namespace GhostPractice
{
	public partial class PageConductor2 : UIViewController
	{
		List<UIViewController> _controllers;
		//reports
		FinancialStatusReport financeReport;
		MatterAnalysisByOwnerReport matterAnalysisReport;
		FeeTargetProgressReport feeTargetReport;

		int reportType { get; set; }

		public PageConductor2 () : base ("PageConductor2", null)
		{
		}

		public PageConductor2 (FinancialStatusReport financeReport, MatterAnalysisByOwnerReport matterAnalysisReport, 
		                       FeeTargetProgressReport feeTargetReport, int reportType)
		{
			this.reportType = reportType;
			this.financeReport = financeReport;
			this.feeTargetReport = feeTargetReport;
			this.matterAnalysisReport = matterAnalysisReport;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		LinearLayout layout;
		UIScrollView scrollView;
		UIPageControl pageControl;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			scrollView = new UIScrollView (new RectangleF (0, 0, 320, 460));
			scrollView.BackgroundColor = UIColor.Blue;
			this.View.AddSubview (scrollView);
			pageControl = new UIPageControl (new RectangleF (0, 460, 320, 20));
			pageControl.BackgroundColor = UIColor.Red;
			this.View.AddSubview (pageControl);
						
					
					
			

			// We've now defined our layout, to actually use it we simply create a UILayoutHost control and pass it the layout
			this.View = new XibFree.UILayoutHost (layout);
			this.View.BackgroundColor = UIColor.Gray;

			LoadControllers ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		/// <summary>
		/// Runs when a dot on the pager is clicked. Scrolls the scroll view to the appropriate
		/// page, based on which one was clicked
		/// </summary>
		protected void HandlePgrMainValueChanged (object sender, EventArgs e)
		{
			//---- it moves page by page. we scroll right to the next controller
			this.scrollView.ScrollRectToVisible (
				this._controllers [(sender as UIPageControl).CurrentPage].View.Frame,
				true);
		}

		/// <summary>
		/// Runs when the scroll view is scrolled. Updates the pager control so that it's
		/// current, based on the page
		/// </summary>
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
			}
		}

		public const int FINANCIAL_STATUS = 1;
		public const int FEE_TARGET = 2;
		public const int MATTER_ANALYSIS = 3;

		private void setCurrentPage ()
		{
			int cnt = 0;
			switch (reportType) {
			case FINANCIAL_STATUS:
				this.pageControl.CurrentPage = 1;
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
			int pageNumber = cnt;

			//---- if it's a valid page
			if (pageNumber >= 0 && pageNumber < this._controllers.Count) {
				//---- Set the current page on the pager control
				this.pageControl.CurrentPage = pageNumber - 1;
				//make scrollview show the page using pageNumber...
				float f = scrollView.Frame.Width * (pageNumber);
				PointF point = new PointF (f, 0);
				this.scrollView.SetContentOffset (point, true);
			}

		}

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

