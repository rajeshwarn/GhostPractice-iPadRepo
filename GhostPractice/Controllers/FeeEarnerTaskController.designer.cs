// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace GhostPractice
{
	[Register ("FeeEarnerTaskController")]
	partial class FeeEarnerTaskController
	{
		[Outlet]
		MonoTouch.UIKit.UINavigationBar navigationBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel feeEarnerName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField taskDescription { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnCancel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnAssignTask { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView img { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel labelMatterName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIDatePicker datePicker { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch switchNotify { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel labelDueDate { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (navigationBar != null) {
				navigationBar.Dispose ();
				navigationBar = null;
			}

			if (feeEarnerName != null) {
				feeEarnerName.Dispose ();
				feeEarnerName = null;
			}

			if (taskDescription != null) {
				taskDescription.Dispose ();
				taskDescription = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnAssignTask != null) {
				btnAssignTask.Dispose ();
				btnAssignTask = null;
			}

			if (img != null) {
				img.Dispose ();
				img = null;
			}

			if (labelMatterName != null) {
				labelMatterName.Dispose ();
				labelMatterName = null;
			}

			if (datePicker != null) {
				datePicker.Dispose ();
				datePicker = null;
			}

			if (switchNotify != null) {
				switchNotify.Dispose ();
				switchNotify = null;
			}

			if (labelDueDate != null) {
				labelDueDate.Dispose ();
				labelDueDate = null;
			}
		}
	}
}
