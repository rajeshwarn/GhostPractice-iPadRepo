// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace GhostPractice
{
	[Register ("LoginController")]
	partial class LoginController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView logo { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnLogin { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtActivation { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (logo != null) {
				logo.Dispose ();
				logo = null;
			}

			if (btnLogin != null) {
				btnLogin.Dispose ();
				btnLogin = null;
			}

			if (txtActivation != null) {
				txtActivation.Dispose ();
				txtActivation = null;
			}
		}
	}
}
