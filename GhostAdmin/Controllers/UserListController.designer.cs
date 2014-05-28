// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace GhostAdmin
{
	[Register ("UserListController")]
	partial class UserListController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnSummaryReport { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnActivityLogs { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnCount { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (btnSummaryReport != null) {
				btnSummaryReport.Dispose ();
				btnSummaryReport = null;
			}

			if (btnActivityLogs != null) {
				btnActivityLogs.Dispose ();
				btnActivityLogs = null;
			}

			if (btnCount != null) {
				btnCount.Dispose ();
				btnCount = null;
			}
		}
	}
}
