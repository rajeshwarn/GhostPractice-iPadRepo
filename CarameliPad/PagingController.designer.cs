// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CarameliPad
{
	[Register ("PagingController")]
	partial class PagingController
	{
		[Outlet]
		MonoTouch.UIKit.UIPageControl pageControl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView scrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnUpload { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolBar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (pageControl != null) {
				pageControl.Dispose ();
				pageControl = null;
			}

			if (scrollView != null) {
				scrollView.Dispose ();
				scrollView = null;
			}

			if (btnUpload != null) {
				btnUpload.Dispose ();
				btnUpload = null;
			}

			if (toolBar != null) {
				toolBar.Dispose ();
				toolBar = null;
			}
		}
	}
}
