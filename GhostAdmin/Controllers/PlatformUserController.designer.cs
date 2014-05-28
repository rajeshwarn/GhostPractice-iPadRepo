// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace GhostAdmin
{
	[Register ("PlatformUserController")]
	partial class PlatformUserController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btncount { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnSMS { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnEmail { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (btncount != null) {
				btncount.Dispose ();
				btncount = null;
			}

			if (btnSMS != null) {
				btnSMS.Dispose ();
				btnSMS = null;
			}

			if (btnEmail != null) {
				btnEmail.Dispose ();
				btnEmail = null;
			}
		}
	}
}
