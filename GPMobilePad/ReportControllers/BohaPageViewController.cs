using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using GhostPractice;
using GhostPracticeLibrary;

namespace GPMobilePad
{
	[Register ("BohaPageViewController")]
	public  class BohaPageViewController : UIViewController
	{
		FinancialStatusReport fsReport;
		MatterAnalysisByOwnerReport maReport;
		FeeTargetProgressReport ftReport;

		public BohaPageViewController () : base ("BohaPageViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		List<UIViewController> controllers;
		ViewDataSource dataSource;
		UIPageViewController pageViewController;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Console.WriteLine ("BohaPageViewController - ViewDidLoad");

			controllers = new List<UIViewController> ();
			pageViewController = new UIPageViewController (UIPageViewControllerTransitionStyle.PageCurl, 
				UIPageViewControllerNavigationOrientation.Horizontal, UIPageViewControllerSpineLocation.Min);

		}

		public void NavigateToPage (int page)
		{
		}

		public void SetReportPages (FinancialStatusReport fsReport, FeeTargetProgressReport ftReport, MatterAnalysisByOwnerReport maReport)
		{
			this.fsReport = fsReport;
			this.ftReport = ftReport;
			this.maReport = maReport;

			controllers.Clear ();

			if (fsReport != null)
				foreach (var branch in fsReport.branches) {
					controllers.Add (new FinancialStatusByBranchController (branch));
				}
			if (ftReport != null) {
				controllers.Add (new FeeTargetProgressController (ftReport));
				controllers.Add (new FeeTargetProgressBranchController (ftReport));
				controllers.Add (new FeeTargetBranchOwnerController (ftReport));
			}
			if (maReport != null) {
				controllers.Add (new MatterAnalysisController (maReport));
				controllers.Add (new MatterAnalysisBranchController (maReport));
				controllers.Add (new MatterAnalysisBranchOwnerController (maReport));
			}
			dataSource = new ViewDataSource (controllers);
			pageViewController.DataSource = dataSource;
			//ViewDataSource has an indexer that returns it's controllers
			pageViewController.SetViewControllers (new[] { controllers [0] }, UIPageViewControllerNavigationDirection.Forward, false, null);
			pageViewController.View.Frame = this.View.Bounds;
			View.AddSubview (pageViewController.View);
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}

	public class ViewDataSource : UIPageViewControllerDataSource
	{
		private LinkedListNode<UIViewController> current;
		LinkedList<UIViewController> controllerList;

		public ViewDataSource (IEnumerable<UIViewController> controllers)
		{
			controllerList = new LinkedList<UIViewController> (controllers);
			current = controllerList.First;
		}

		public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{

			var __c = controllerList.Find (referenceViewController);

			if (__c.Previous != null)
				return __c.Previous.Value;
			else
				return null;
		}

		public override UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var __c = controllerList.Find (referenceViewController);

			if (__c.Next != null)
				return __c.Next.Value;
			else
				return null;
		}
	}
}
