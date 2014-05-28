using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace GPMobilePad
{
	[Register ("PagedViewController")]
	public class PagedViewController : UIViewController
	{
		static float contentWidth = 700f;
		static float contentHeight = 700f;
		static int pageControlHeight = 30;
		MatterDetail matterDetail;
		Finder finder;

		public ReportSelectorController reportSelector { get; set; }
		//UIPopoverController masterPopoverController;
		UIBarButtonItem btnClose;
		SplitController splitController;
		//private NSObject notificationObserver;
		public IPagedViewDataSource PagedViewDataSource { get; set; }

		int originalPage;

		public void SetCurrentConfiguration (int currentReport, bool isFinance, bool isFeeTarget, bool isMatterAnalysis)
		{
			switch (currentReport) {
			case 0:
				originalPage = 0;
				break;
			case 1:
				originalPage = 2;
				break;
			case 2:
				if (!isFinance) {
					if (!isFeeTarget) {
						originalPage = 0;
					} else {
						originalPage = 3;
					}
				}
				originalPage = 5;
				break;
			}
		}

		readonly UIScrollView _scrollView = new PagedScrollView ();
		readonly UIScrollView _scrollViewP = new PagedScrollViewPortrait ();
		readonly IList<UIViewController> _pages = new List<UIViewController> ();
		readonly UIPageControl _pageControl = new UIPageControl {
			Pages = 0, BackgroundColor = UIColor.DarkGray,
			Frame = new RectangleF (0, 0, contentWidth, pageControlHeight)
		};
		readonly UIPageControl _pageControlP = new UIPageControl {
			Pages = 0, BackgroundColor = UIColor.DarkGray,
			Frame = new RectangleF (0, 0, contentWidth, pageControlHeight)
		};

		public override bool ShouldAutorotate ()
		{
			return false;
		}

		public PagedViewController (IntPtr p) : base (p)
		{
		}
		//custom constructor
		public PagedViewController (SplitController splitController, Finder finder, MatterDetail matterDetail)
		{
			Console.WriteLine ("PagedViewController - loaded custom constructor");
			this.finder = finder;
			this.matterDetail = matterDetail;
			this.splitController = splitController;

			_scrollView.DecelerationEnded += HandleScrollViewDecelerationEnded;
			_pageControl.ValueChanged += HandlePageControlValueChanged;
			//
			_scrollViewP.DecelerationEnded += HandleScrollViewDecelerationEndedPortrait;
			_pageControlP.ValueChanged += HandlePageControlValueChangedPortrait;
		}

		private int _page;

		public int NumberOfPages {
			get { 

				return _pages.Count;
			}
		}

		public int Page {
			get { 
				return _page;
			}
			set {
				_page = value;
				if (contentHeight > 900) {
					_pageControlP.CurrentPage = value;
					_scrollViewP.SetContentOffset (new PointF ((value * contentWidth), 0), true);

				} else {
					_pageControl.CurrentPage = value;
					_scrollView.SetContentOffset (new PointF ((value * contentWidth), 0), true);
				}
				_pages [value].ViewDidAppear (true);
				SetTitle (_page + 1);
			}
		}

		void SetTitle (int num)
		{
			Title = "Reports Page " + num + " of " + this.PagedViewDataSource.Pages;
		}

		void HandleScrollViewDecelerationEnded (object sender, EventArgs e)
		{
			int page = (int)Math.Floor ((_scrollView.ContentOffset.X - _scrollView.Frame.Width / 2) / _scrollView.Frame.Width) + 1;
			_page = page;
			_pageControl.CurrentPage = page;
			_pages [page].ViewDidAppear (true);
			SetTitle (_page + 1);
			//Console.WriteLine ("HandleScrollViewDecelerationEnded " + _page);
		}

		void HandleScrollViewDecelerationEndedPortrait (object sender, EventArgs e)
		{
			int page = (int)Math.Floor ((_scrollViewP.ContentOffset.X - _scrollViewP.Frame.Width / 2) / _scrollViewP.Frame.Width) + 1;
			_page = page;
			_pageControlP.CurrentPage = page;
			_pages [page].ViewDidAppear (true);
			//Console.WriteLine ("HandleScrollViewDecelerationEndedPortrait " + _page);
		}

		void HandlePageControlValueChanged (object sender, EventArgs e)
		{
			Page = _pageControl.CurrentPage;
			Console.WriteLine ("HandlePageControlValueChanged " + Page);
		}

		void HandlePageControlValueChangedPortrait (object sender, EventArgs e)
		{
			Page = _pageControlP.CurrentPage;
			Console.WriteLine ("HandlePageControlValueChangedPortrait " + Page);
		}

		private int savedPageNumber = -1;

		public void ReloadPages ()
		{
			if (PagedViewDataSource == null) {
				Console.WriteLine ("PagedViewDataSource is NULL");
				return;
			}
			if (pop != null) {
				pop.Dismiss (true);
			}
			Console.WriteLine ("Reloading report pages - " + PagedViewDataSource.Pages);

			PagedViewDataSource.Reload ();
             
			//clear collection 
			foreach (var p in _pages)
				p.View.RemoveFromSuperview ();

			int i = 0;
			var numberOfPages = PagedViewDataSource.Pages;


			if (contentHeight > 900) {     //Portrait
				Console.WriteLine ("**** Device is probably in portrait orientation, contentHeight: " + contentHeight);
				_scrollView.RemoveFromSuperview ();
				View.AddSubview (_pageControlP);
				View.AddSubview (_scrollViewP);
				for (i = 0; i < numberOfPages; i++) {
					var pageViewController = PagedViewDataSource.GetPage (i);
					pageViewController.View.Frame = new RectangleF (contentWidth * i, 0, contentWidth, this._scrollViewP.Frame.Height - pageControlHeight);
					_scrollViewP.AddSubview (pageViewController.View);
					_pages.Add (pageViewController);
				}				
				_scrollViewP.ContentSize = new SizeF (contentWidth * (i == 0 ? 1 : i), contentHeight);
				_pageControlP.Pages = numberOfPages;
				if (savedPageNumber > -1) {
					_pageControlP.CurrentPage = savedPageNumber;
					Page = savedPageNumber;
					savedPageNumber = -1;
				} else {
					_pageControlP.CurrentPage = 0;
				}
			} else { //Landscape
				Console.WriteLine ("---- Device is probably in landscape orientation, contentHeight: " + contentHeight);
				_scrollViewP.RemoveFromSuperview ();
				View.AddSubview (_pageControl);
				View.AddSubview (_scrollView);
				for (i = 0; i < numberOfPages; i++) {
					var pageViewController = PagedViewDataSource.GetPage (i);
					pageViewController.View.Frame = new RectangleF (contentWidth * i, 0, contentWidth, this._scrollView.Frame.Height - pageControlHeight);
					_scrollView.AddSubview (pageViewController.View);
					_pages.Add (pageViewController);
				}
				
				_scrollView.ContentSize = new SizeF (contentWidth * (i == 0 ? 1 : i), contentHeight);
				_pageControl.Pages = numberOfPages;
				if (savedPageNumber > -1) {
					_pageControl.CurrentPage = savedPageNumber;
					Page = savedPageNumber;
					savedPageNumber = -1;
				} else {
					_pageControl.CurrentPage = 0;
				}

			}

                 
			PagedViewDataSource.Reload ();
			if (_pages.Count > 0) {
				_pages [0].ViewDidAppear (true);
			}
		}

		public override void ViewDidLoad ()
		{
			Console.WriteLine ("Page Controller view did load");
			//notificationObserver = NSNotificationCenter.DefaultCenter
			//	.AddObserver ("UIDeviceOrientationDidChangeNotification", DeviceRotated);

			Console.WriteLine ("Page Controller - ViewDidLoad before setting Frame, width: " + View.Frame.Width + " height: " + View.Frame.Height);
			Title = "Report Pages";

			this.NavigationController.ToolbarHidden = true;
			this.NavigationController.SetToolbarHidden (true, true);

		}

		public int currentPageType { get; set; }

		public const int FEE_TARGET = 1, MATTER_ANALYSIS = 2, FINANCE = 3;
		UIPopoverController pop;

		private void setButton ()
		{
			btnClose = new UIBarButtonItem ("Close", UIBarButtonItemStyle.Plain, delegate (object sender, EventArgs e) {
				Console.WriteLine ("close reports button clicked");
				//go back to split with finder and matter detail...


				var detailNavigationController = new UINavigationController (matterDetail);
				if (finder == null) {
					finder = new Finder (matterDetail);
				}
				var masterNavigationController = new UINavigationController (finder);
				
				splitController.WeakDelegate = matterDetail;
				splitController.ViewControllers = new UIViewController[] {
					masterNavigationController,
					detailNavigationController
				};
			});		
			
			//btnClose.TintColor = UIColor.Red;			
			UIBarButtonItem[] btns = { btnClose };
			this.NavigationItem.SetRightBarButtonItems (btns, true);


		}

		public override void ViewDidAppear (bool animated)
		{
			Console.WriteLine ("Page Controller view did appear, width: " + View.Frame.Width + " height: " + View.Frame.Height);

			View.Frame = new RectangleF (0, 0, View.Frame.Width, View.Frame.Height);
			View.BackgroundColor = UIColor.White;

			contentWidth = View.Frame.Width - 2;
			contentHeight = 700;
			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications ();
			Console.WriteLine ("Device Orientation: " + UIDevice.CurrentDevice.Orientation + " width: " + View.Frame.Width + " height: " + View.Frame.Height);
			setButton ();
			ReloadPages ();

		}

		private void DeviceRotated (NSNotification notification)
		{
			
			Console.WriteLine ("Device Rotated! orientation: " + UIDevice.CurrentDevice.Orientation + " width: " + View.Frame.Width + " height: " + View.Frame.Height
			+ " _page: " + _page);
			savedPageNumber = _page;
			if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft) {
				if (pop != null) {
					pop.Dismiss (true);
				}
			} 
			contentWidth = View.Frame.Width;
			contentHeight = View.Frame.Height;
			setButton ();
			ReloadPages ();

		}

		[Export ("splitController:willHideViewController:withBarButtonItem:forPopoverController:")]
		public void WillHideViewController (UISplitViewController splitController, UIViewController viewController, UIBarButtonItem barButtonItem, UIPopoverController popoverController)
		{
			barButtonItem.Title = "Find Matter";
			NavigationItem.SetLeftBarButtonItem (barButtonItem, true);
			//masterPopoverController = popoverController;
		}

		[Export ("splitController:willShowViewController:invalidatingBarButtonItem:")]
		public void WillShowViewController (UISplitViewController svc, UIViewController vc, UIBarButtonItem button)
		{
			// Called when the view is shown again in the split view, invalidating the button and popover controller.
			NavigationItem.SetLeftBarButtonItem (null, true);
			//masterPopoverController = null;
		}

		sealed class PagedScrollView : UIScrollView
		{
			public PagedScrollView ()
			{
				ShowsHorizontalScrollIndicator = false;
				ShowsVerticalScrollIndicator = false;
				Bounces = true;
				ContentSize = new SizeF (contentWidth, contentHeight);
				PagingEnabled = true;
				Frame = new RectangleF (0, 30, contentWidth, contentHeight);
			}
		}

		sealed class PagedScrollViewPortrait : UIScrollView
		{
			public PagedScrollViewPortrait ()
			{
				ShowsHorizontalScrollIndicator = false;
				ShowsVerticalScrollIndicator = false;
				Bounces = true;
				ContentSize = new SizeF (contentWidth, contentHeight);
				PagingEnabled = true;
				Frame = new RectangleF (0, 30, contentWidth, contentHeight);
			}
		}
	}
	/*
	 * 
	 */
}