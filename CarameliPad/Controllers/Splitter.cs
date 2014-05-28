using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CarameliPad
{
	partial class Splitter : UISplitViewController
	{
		public UIViewController masterView, detailView;

		public Splitter () : base()
		{
			// create our master and detail views
			masterView = new RootViewController ();
			detailView = new DetailViewController ();
			// create an array of controllers from them and then
			// assign it to the controllers property
			ViewControllers = new UIViewController[]
                { masterView, detailView }; // order is important
			
			ShouldHideViewController = (svc, viewController, inOrientation) => {
				return false; // default behaviour is true
			};
		}
		
		public Splitter (UIViewController masterView, UIViewController detailView)
		{
			this.masterView = masterView;
			this.detailView = detailView;
			// assign it to the controllers property
			ViewControllers = new UIViewController[]
                { masterView, detailView }; // order is important
			
		}

		public override bool ShouldAutorotateToInterfaceOrientation
        (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}

