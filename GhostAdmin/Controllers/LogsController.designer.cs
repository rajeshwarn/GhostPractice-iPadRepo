// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace GhostAdmin
{
	[Register ("LogsController")]
	partial class LogsController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnSummary { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnLogs { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (btnSummary != null) {
				btnSummary.Dispose ();
				btnSummary = null;
			}

			if (btnLogs != null) {
				btnLogs.Dispose ();
				btnLogs = null;
			}
		}
	}
}
