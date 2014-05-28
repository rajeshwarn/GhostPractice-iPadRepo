// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace GhostAdmin
{
	[Register ("CompanyListController")]
	partial class CompanyListController
	{
		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnSummaryReport { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnActivityLogs { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnRefresh { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnSummaryReport != null) {
				btnSummaryReport.Dispose ();
				btnSummaryReport = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (btnActivityLogs != null) {
				btnActivityLogs.Dispose ();
				btnActivityLogs = null;
			}

			if (btnRefresh != null) {
				btnRefresh.Dispose ();
				btnRefresh = null;
			}
		}
	}
}
