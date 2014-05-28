using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace GhostPractice
{
	public class LoadingAlertView : UIAlertView
	{
		public LoadingAlertView (string title) : base(title,String.Empty,null,null,null)
		{
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();


			UIActivityIndicatorView indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge);
			indicator.Center = new System.Drawing.PointF (this.Bounds.Size.Width / 2, 
				                                             this.Bounds.Size.Height - 50);
			indicator.StartAnimating ();

			this.AddSubview (indicator);
		}
	}
}