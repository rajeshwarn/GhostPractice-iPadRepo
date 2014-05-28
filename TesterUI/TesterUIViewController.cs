using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TesterUI
{
	public partial class TesterUIViewController : UIViewController
	{
		public TesterUIViewController () : base ("TesterUIViewController", null)
		{
		}

		UIScrollView scrollView;
		UIPageControl pageControl;

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			scrollView = new UIScrollView (new RectangleF (0, 0, 320, 460));
			scrollView.BackgroundColor = UIColor.Blue;
			this.View.AddSubview (scrollView);
			pageControl = new UIPageControl (new RectangleF (0, 460, 320, 20));
			pageControl.BackgroundColor = UIColor.Red;
			this.View.AddSubview (pageControl);
			// Perform any additional setup after loading the view, typically from a nib.
		}
	}
}

