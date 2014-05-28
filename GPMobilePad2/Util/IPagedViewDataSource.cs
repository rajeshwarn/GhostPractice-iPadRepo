using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace GPMobilePad
{
	public interface IPagedViewDataSource
	{
		int Pages { get; }
		UIViewController GetPage (int i);
		void Reload ();
	}
}

