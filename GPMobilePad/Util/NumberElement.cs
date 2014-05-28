using System;
using System.Text;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GPMobilePad
{
	public class NumberElement : OwnerDrawnElement
	{
		CGGradient gradient;
		private UIFont captionFont = UIFont.SystemFontOfSize (16.0f);
		private UIFont amountFont = UIFont.BoldSystemFontOfSize (20.0f);

		public NumberElement (int number, string caption) : base (UITableViewCellStyle.Default, "numberElement")
		{
			this.Label = caption;
			this.Number = number;
			SetUp ();
		}

		private void SetUp ()
		{
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB ();
			gradient = new CGGradient (
				colorSpace,
				new float[] { 0.95f, 0.95f, 0.95f, 1, 
					0.85f, 0.85f, 0.85f, 1
				},
				new float[] { 0, 1 });  
		}

		public string Label {
			get;
			set; 
		}

		public int Number {
			get;
			set; 
		}

		public override void Draw (RectangleF bounds, CGContext context, UIView view)
		{
			UIColor.White.SetFill ();
			context.FillRect (bounds);
			
			context.DrawLinearGradient (
				gradient,
				new PointF (bounds.Left, bounds.Top),
				new PointF (bounds.Left, bounds.Bottom),
				CGGradientDrawingOptions.DrawsAfterEndLocation
			);
			
			UIColor.DarkGray.SetColor ();
			view.DrawString (
				this.Label,
				new RectangleF (10, 10, bounds.Width / 2, 10),
				captionFont,
				UILineBreakMode.TailTruncation
			);
			
			UIColor.Black.SetColor ();
			view.DrawString (
				this.Number.ToString (),
				new RectangleF (bounds.Width / 2, 10, (bounds.Width / 2) - 10, 10),
				amountFont,
				UILineBreakMode.TailTruncation,
				UITextAlignment.Right
			);
			
			
		}

		public override float Height (RectangleF bounds)
		{
			var height = 25.0f + TextHeight (bounds);
			return height;
		}

		private float TextHeight (RectangleF bounds)
		{
			SizeF size;
			using (NSString str = new NSString (this.Label)) {
				size = str.StringSize (
					captionFont,
					new SizeF (bounds.Width - 20, 1000),
					UILineBreakMode.WordWrap
				);
			}			
			return size.Height;
		}

		public override string ToString ()
		{
			return string.Format (Label);
		}
	}
}

