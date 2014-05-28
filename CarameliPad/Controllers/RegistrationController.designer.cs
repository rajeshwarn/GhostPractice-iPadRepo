// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CarameliPad
{
	[Register ("RegistrationController")]
	partial class RegistrationController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView imgLogo { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtCellphone { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnRegister { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (imgLogo != null) {
				imgLogo.Dispose ();
				imgLogo = null;
			}

			if (txtCellphone != null) {
				txtCellphone.Dispose ();
				txtCellphone = null;
			}

			if (btnRegister != null) {
				btnRegister.Dispose ();
				btnRegister = null;
			}
		}
	}
}
