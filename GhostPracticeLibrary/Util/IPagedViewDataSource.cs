using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace GhostPractice
{
	public interface IPagedViewDataSource
	{
		int Pages { get; }

		UIViewController GetPage (int i);

		void Reload ();
	}
}

