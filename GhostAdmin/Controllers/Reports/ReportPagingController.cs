using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GhostAdmin
{
	public partial class ReportPagingController : UIViewController
	{
		public List<UIViewController> _controllers;
		public FeeTargetReportController feeTargetController;
		public FinancialAnalysisReportController financeController;
		public MatterAnalysisReportController matterAnalysisController;
		
		public ReportPagingController () : base ("ReportPagingController", null)
		{
			Title = "Report Paging Control";
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
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		protected void LoadControllers ()
		{
			//---- instantiate and add the controllers to our list
			feeTargetController = new FeeTargetReportController();
			financeController = new FinancialAnalysisReportController();
			matterAnalysisController = new MatterAnalysisReportController();
			this._controllers.Add (financeController);
			this._controllers.Add (feeTargetController);
			this._controllers.Add (matterAnalysisController);

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

