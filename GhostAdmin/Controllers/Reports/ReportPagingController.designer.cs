// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace GhostAdmin
{
	[Register ("ReportPagingController")]
	partial class ReportPagingController
	{
		[Outlet]
		MonoTouch.UIKit.UIScrollView scrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIPageControl pageControl { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (scrollView != null) {
				scrollView.Dispose ();
				scrollView = null;
			}

			if (pageControl != null) {
				pageControl.Dispose ();
				pageControl = null;
			}
		}
	}
}
