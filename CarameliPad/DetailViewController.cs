using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CarameliPad
{
	public partial class DetailViewController : UIViewController
	{
		public UIPopoverController popoverController;
		object detailItem;
		List<UIViewController> _controllers;
		
		public DetailViewController () : base ("DetailViewController", null)
		{
			this.Title = NSBundle.MainBundle.LocalizedString ("Detail", "Detail");
		}
		
		public void Update (Property property)
		{
			Console.WriteLine ("DetailViewController: Update");
			//do something with property
			
			// dismiss the popover if currently visible
			if (popoverController != null)
				popoverController.Dismiss (true);
		}

		public void SetDetailItem (object newDetailItem)
		{
			Console.WriteLine ("In DetailView SetDetailItem...");
			if (detailItem != newDetailItem) {
				detailItem = newDetailItem;
				
				// Update the view
				ConfigureView ();
			}
			
			if (this.popoverController != null)
				this.popoverController.Dismiss (true);
		}
		
		void ConfigureView ()
		{
			Console.WriteLine ("In DetailView ConfigureView...");
			// Update the user interface for the detail item
			if (detailItem != null) {
			}
			//this.detailDescriptionLabel.Text = detailItem.ToString ();
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
			pageControl.Pages = 5;
			//---- wire up our pager and scroll view event handlers
			this.pageControl.ValueChanged += HandlePgrMainValueChanged;
			this.scrollView.Scrolled += HandleScrlMainScrolled;

			LoadControllers ();
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
			return (toInterfaceOrientation == UIInterfaceOrientation.Portrait || toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown);
		}
		
		[Export("splitViewController:willHideViewController:withBarButtonItem:forPopoverController:")]
		public void WillHideViewController (UISplitViewController svc, UIViewController vc,
			UIBarButtonItem barButtonItem, UIPopoverController pc)
		{
			barButtonItem.Title = "Property List";
			var items = new List<UIBarButtonItem> ();
			items.Add (barButtonItem);
			items.AddRange (toolbar.Items);
			toolbar.SetItems (items.ToArray (), true);
			popoverController = pc;
		}
		
		[Export("splitViewController:willShowViewController:invalidatingBarButtonItem:")]
		public void WillShowViewController (UISplitViewController svc, UIViewController vc,
			UIBarButtonItem button)
		{
			// Called when the view is shown again in the split view, invalidating the button and popover controller.
			var items = new List<UIBarButtonItem> (toolbar.Items);
			items.RemoveAt (0);
			toolbar.SetItems (items.ToArray (), true);
			popoverController = null;
		}
		/// <summary>
		/// Runs when a dot on the pager is clicked. Scrolls the scroll view to the appropriate
		/// page, based on which one was clicked
		/// </summary>
		protected void HandlePgrMainValueChanged (object sender, EventArgs e)
		{
			//---- it moves page by page. we scroll right to the next controller
			this.scrollView.ScrollRectToVisible (
                this._controllers [(sender as UIPageControl).CurrentPage].View.Frame,
                true);
		}
		/// <summary>
		/// Runs when the scroll view is scrolled. Updates the pager control so that it's
		/// current, based on the page
		/// </summary>
		protected void HandleScrlMainScrolled (object sender, EventArgs e)
		{
			//---- calculate the page number
			int pageNumber = (int)(Math.Floor (
                (this.scrollView.ContentOffset.X - this.scrollView.Frame.Width / 2)
                / this.scrollView.Frame.Width) + 1);

			//---- if it's a valid page
			if (pageNumber >= 0 && pageNumber < this._controllers.Count) {
				//---- Set the current page on the pager control
				this.pageControl.CurrentPage = pageNumber;
			}
		}

		protected void LoadControllers ()
		{
			//---- instantiate and add the controllers to our page list
			this._controllers = new List<UIViewController> ();
			
			int pages = 5;
			
			RegistrationController rCont = new RegistrationController ();
			var contLand = new OwnerAddressLandscapeController ();
			var contPort = new OwnerAddressPortraitController ();
			OwnerAndAddressDataController oCont = new OwnerAndAddressDataController (contLand, contLand, contPort);
			GPSMapController gCont = new GPSMapController ();
			PropertyAttributesDataController pCont = new PropertyAttributesDataController ();
			PhotoGalleryController fCont = new PhotoGalleryController ();
			this._controllers.Add (rCont);
			this._controllers.Add (oCont);
			this._controllers.Add (pCont);
			this._controllers.Add (fCont);
			this._controllers.Add (gCont);
			
			pageControl.Pages = pages;
			
			scrollView.PagingEnabled = true;
			//---- loop through each one
			for (int i = 0; i < this._controllers.Count; i++) {
				//---- set their location and size, each one is moved to the
				// right by the width of the previous
				RectangleF viewFrame = new RectangleF (
                        this.scrollView.Frame.Width * i
                        , this.scrollView.Frame.Y
                        , this.scrollView.Frame.Width
                        , this.scrollView.Frame.Height);
				this._controllers [i].View.Frame = viewFrame;

				//---- add the view to the scrollview
				this.scrollView.AddSubview (this._controllers [i].View);
			}

			//---- set our scroll view content size (width = number of pages * scroll view
			// width)
			this.scrollView.ContentSize = new SizeF (
                this.scrollView.Frame.Width * this._controllers.Count,
                this.scrollView.Frame.Height);
			
		}
	}
}

