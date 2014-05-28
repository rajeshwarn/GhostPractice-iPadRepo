using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GhostPractice
{
	public class PagerTestCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("PagerTestCell");

		public PagerTestCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

