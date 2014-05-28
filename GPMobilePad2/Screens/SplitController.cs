using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace GPMobilePad
{
	public class SplitController: UISplitViewController
	{
		Finder masterView;
		MatterDetail matterDetail;

		public SplitController () : base ()
		{
			// create our master and detail views

			matterDetail = new MatterDetail (this, masterView);
			masterView = new Finder (matterDetail);
			// create an array of controllers from them and then
			// assign it to the controllers property
			ViewControllers = new UIViewController[]
			{ masterView, matterDetail }; // order is important

		}

		public void setReportControllers (ReportSelectorController master, PagedViewController pager)
		{
			ViewControllers = new UIViewController[]
			{ master, pager };
		}

		public void setProvisionControllers (LaunchSplash master, ProvisionDialog detail)
		{
			ViewControllers = new UIViewController[]
			{ master, detail };
		}
	}
}

