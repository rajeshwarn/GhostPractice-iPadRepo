using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GhostPractice
{
	public class ActivityIndicator : IDisposable
	{
		UIAlertView _alert;
		UIActivityIndicatorView _ai;

		public ActivityIndicator (String title)
		{
			_alert = new UIAlertView (title, String.Empty, null, null, null);
			_ai = new UIActivityIndicatorView ();
			_ai.Frame = new System.Drawing.RectangleF (125, 50, 40, 40);
			_ai.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
			_alert.AddSubview (_ai);
			_ai.StartAnimating ();

			_alert.Show ();
		}

		#region IDisposable implementation

		void IDisposable.Dispose ()
		{
			_alert.DismissWithClickedButtonIndex (0, true);
		}

		#endregion

	}
}

