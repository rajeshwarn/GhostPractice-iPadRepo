using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CarameliPad
{
	public partial class OwnerAndAddressDataController : UIViewController
	{
		public UIViewController LandscapeLeftViewController {get;set;}
		public UIViewController LandscapeRightViewController {get;set;}
		public UIViewController PortraitViewController {get;set;}

		private NSObject notificationObserver;
		
//		public OwnerAndAddressDataController () : base ("OwnerAndAddressDataController", null)
//		{
//		}
//		
		public OwnerAndAddressDataController (UIViewController LandscapeLeftViewController, UIViewController LandscapeRightViewController, UIViewController PortraitViewController)
		{
			this.LandscapeLeftViewController = LandscapeLeftViewController;
			this.LandscapeRightViewController = LandscapeRightViewController;
			this.PortraitViewController = PortraitViewController;
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
			
			notificationObserver  = NSNotificationCenter.DefaultCenter
					.AddObserver("UIDeviceOrientationDidChangeNotification", DeviceRotated );
		}
		public override void ViewWillAppear (bool animated)
		{
			if (PortraitViewController != null) {
				_showView(PortraitViewController.View);
			}
		}
		private void _showView(UIView view){

			if (this.NavigationController!=null)
				NavigationController.SetNavigationBarHidden(view!=PortraitViewController.View, false);

			_removeAllViews();
			view.Frame = this.View.Frame;
			View.AddSubview(view);

		}
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return true;
		}
		public override void ViewDidAppear (bool animated)
		{
			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
		}

		public override void ViewWillDisappear (bool animated)
		{
			UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
		}


		private void DeviceRotated(NSNotification notification){

			Console.WriteLine("rotated! "+UIDevice.CurrentDevice.Orientation);
			switch (UIDevice.CurrentDevice.Orientation){

				case  UIDeviceOrientation.Portrait:
					_showView(PortraitViewController.View);
					break;

				case UIDeviceOrientation.LandscapeLeft:
					_showView(LandscapeLeftViewController.View);

					break;
				case UIDeviceOrientation.LandscapeRight:
					_showView(LandscapeRightViewController.View);
					break;
			}
		}

		private void _removeAllViews(){
			PortraitViewController.View.RemoveFromSuperview();
			LandscapeLeftViewController.View.RemoveFromSuperview();
			LandscapeRightViewController.View.RemoveFromSuperview();
		}
		protected void OnDeviceRotated(){

		}


		public override void ViewDidDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver (notificationObserver);
		}

	}
}

