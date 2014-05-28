// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace GhostPractice
{
	[Register ("MatterFindController")]
	partial class MatterFindController
	{
		[Outlet]
		MonoTouch.UIKit.UISearchBar searchBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnReports { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnCount { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (searchBar != null) {
				searchBar.Dispose ();
				searchBar = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (btnReports != null) {
				btnReports.Dispose ();
				btnReports = null;
			}

			if (btnCount != null) {
				btnCount.Dispose ();
				btnCount = null;
			}
		}
	}
}
