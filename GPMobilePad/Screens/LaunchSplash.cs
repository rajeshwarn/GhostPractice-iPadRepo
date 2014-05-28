using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;

namespace GPMobilePad
{
	public partial class LaunchSplash : DialogViewController
	{
		public LaunchSplash () : base (UITableViewStyle.Grouped, null)
		{
			Root = new RootElement ("");
			UIImage image = UIImage.FromFile ("Images/launch_small.png");
			UIImageView imgView = new UIImageView (image);
			imgView.Frame = new System.Drawing.RectangleF (0, 10, 320, 640);

			var sec = new Section (imgView);
			Root.Add (sec);
		}
		//		public override void ViewDidLoad ()
		//		{
		//			base.ViewDidLoad ();
		//			TableView.BackgroundColor = UIColor.Clear;
		//			UIImage background = UIImage.FromFile ("Images/launch_small.png");
		//			UIImageView imgView = new UIImageView(background);
		//			imgView.Frame = new System.Drawing.RectangleF(0,0,320,480);
		//		}
	}
}
