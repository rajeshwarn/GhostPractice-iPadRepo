using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
namespace GhostPractice
{
	public class BusyModalScreen: UIViewController
	{
		public BusyModalScreen ()
		{

		}
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			BuildInterface ();
			
		}
		public void BuildInterface ()
		{
			UIView contentView = new UIView ();
			contentView.Frame = new RectangleF (new PointF (0, 0), new SizeF (320, 200));
			contentView.BackgroundColor = UIColor.Clear;
			
			UILabel label = new UILabel ();
			label.Text = "Loading... please wait";
			label.Font = UIFont.BoldSystemFontOfSize (24);
			
			label.Frame = new RectangleF (new PointF (20, 150), new SizeF (300, 48));
			
			contentView.AddSubview (label);
			this.View.AddSubview (contentView);
			this.View.BackgroundColor = UIColor.Clear;
			
			Console.WriteLine ("##### Inside the busy screen...");
						

		}
		//
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			//return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			return false;
		}
	}
}


