using System;
using System.Text;
using System.Drawing;
using MonoTouch.CoreGraphics;

using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Globalization;

namespace GhostPractice
{
	public class ButtonElement : OwnerDrawnElement
	{
		CGGradient gradient;
		private UIFont captionFont = UIFont.BoldSystemFontOfSize (20.0f);

		public ButtonElement (string caption) : base(UITableViewCellStyle.Default, "buttonElement")
		{
			this.Label = caption;
			SetUp ();
		}
		
		private void SetUp ()
		{
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB ();
			gradient = new CGGradient (
			    colorSpace,
			    new float[] { 0.95f, 0.95f, 0.95f, 1, 
							  0.85f, 0.85f, 0.85f, 1},
				new float[] { 0, 1 });  
		}
		public string Label {
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
			
			//UIColor.Black.SetColor ();
			ColorHelper.GetGPLightPurple ().SetColor ();
			view.DrawString (
				this.Label,
				new RectangleF(10, 10, bounds.Width , 20 ),
				captionFont,
				UILineBreakMode.TailTruncation,
				UITextAlignment.Center
			);
			
			
			
			
		}
		
		public override float Height (RectangleF bounds)
		{
			var height = 35.0f + TextHeight (bounds);
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



