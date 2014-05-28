using System;
using System.Text;
using System.Drawing;
using MonoTouch.CoreGraphics;

using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Globalization;

namespace GPMobilePad
{
	public class FinanceElement : OwnerDrawnElement
	{
		CGGradient gradient;

		//TODO - dynamic font size for different iPads: mini, mini-retina, iPad 1,2,3,4 and Air

		private UIFont captionFont = UIFont.SystemFontOfSize (16.0f);
		private UIFont amountFont = UIFont.BoldSystemFontOfSize (20.0f);



		public FinanceElement (string caption, double amount) : base(UITableViewCellStyle.Default, "financeElement")
		{
			this.Label = caption;
			this.Amount = amount;
			SetUp ();
		}
		public FinanceElement (string caption, double amount, float captionFontSize, float amountFontSize) : base(UITableViewCellStyle.Default, "financeElement")
		{
			this.Label = caption;
			this.Amount = amount;
			captionFont = UIFont.SystemFontOfSize (captionFontSize);
			amountFont = UIFont.BoldSystemFontOfSize (amountFontSize);
			SetUp ();
		}
		public FinanceElement (string caption, double amount, int deviceType) : base(UITableViewCellStyle.Default, "financeElement")
		{
			this.Label = caption;
			this.Amount = amount;

			switch (deviceType) {
			case IPAD_AIR:
				captionFont = UIFont.SystemFontOfSize (18.0f);
				amountFont = UIFont.BoldSystemFontOfSize (22.0f);
				break;
			case IPAD_4:
				captionFont = UIFont.SystemFontOfSize (16.0f);
				amountFont = UIFont.BoldSystemFontOfSize (20.0f);
				break;
			case IPAD_3:
				captionFont = UIFont.SystemFontOfSize (16.0f);
				amountFont = UIFont.BoldSystemFontOfSize (20.0f);
				break;
			case IPAD_2:
				captionFont = UIFont.SystemFontOfSize (16.0f);
				amountFont = UIFont.BoldSystemFontOfSize (20.0f);
				break;
			case IPAD_1:
				captionFont = UIFont.SystemFontOfSize (15.0f);
				amountFont = UIFont.BoldSystemFontOfSize (19.0f);
				break;
			case IPAD_MINI:
				captionFont = UIFont.SystemFontOfSize (15.0f);
				amountFont = UIFont.BoldSystemFontOfSize (19.0f);
				break;
			case IPAD_MINI_RETINA:
				captionFont = UIFont.SystemFontOfSize (18.0f);
				amountFont = UIFont.BoldSystemFontOfSize (22.0f);
				break;
			
			}

			SetUp ();
		}

		public const int IPAD_AIR = 5,
		 IPAD_1 = 1, IPAD_2 = 2, IPAD_3 = 3, IPAD_4 = 4, IPAD_MINI = 6, IPAD_MINI_RETINA = 7;
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
		
		public double Amount {
			get;
			set; 
		}

		
		
		public override void Draw (RectangleF bounds, CGContext context, UIView view)
		{
//			string name = CultureInfo.CurrentCulture.Name;
//			NumberFormatInfo nfi = new CultureInfo (name, true).NumberFormat;
//			if (name == "en-ZA") {
//				nfi.CurrencyDecimalSeparator = ",";
//				nfi.CurrencyGroupSeparator = " ";
//				
//				nfi.NumberDecimalSeparator = ",";
//				nfi.NumberGroupSeparator = " ";
//			}
			
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
				this.Amount.ToString ("N2"),
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

