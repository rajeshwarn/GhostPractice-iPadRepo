using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace GhostPractice
{
	public class MoneyRow: UIView
	{
		public MoneyRow (string caption, double amount)
		{
			SetMoneyRow (caption, amount);
		}
		
		private void SetMoneyRow (string caption, double amount)
		{
			this.Frame = new RectangleF (0, 0, 300, 30);
			RectangleF rectangle1 = new RectangleF (5, 50, 130, 20);
			var view1 = new UILabel (rectangle1);
			view1.TextColor = UIColor.Black;
			view1.Font = UIFont.SystemFontOfSize (14);
			view1.Text = caption;
			AddSubview (view1);
            
			RectangleF rectangle2 = new RectangleF (135, 50, 160, 20);
			var view2 = new UILabel (rectangle2);
			view2.Font = UIFont.BoldSystemFontOfSize (16);
			view2.Text = amount.ToString ("N2");
			view2.TextAlignment = UITextAlignment.Right;
			AddSubview (view2);
		}
	}
}

