using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CarameliPad
{
	public class SplitViewContoller : UISplitViewController
	{
		RootViewController masterView;
		DetailViewController detailView;

		public SplitViewContoller () : base()
		{
			// create our master and detail views
			masterView = new RootViewController ();
			detailView = new DetailViewController ();
			// create an array of controllers from them and then
			// assign it to the controllers property
			ViewControllers = new UIViewController[]
                { masterView, detailView }; // order is important
			

			
			masterView.RowClicked += (object sender, RootViewController.RowClickedEventArgs e) => {
				detailView.Update (e.Item);
				Console.WriteLine ("SplitViewController: event trapped and detail updated");
			};
		}

		public override bool ShouldAutorotateToInterfaceOrientation
        (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}

