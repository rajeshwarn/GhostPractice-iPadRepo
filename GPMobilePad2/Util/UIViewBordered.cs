using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
 
namespace GPMobilePad
{
	public partial class UIViewBordered : UIView
	{
		// Constructor used when view is defined in InterfaceBuilder
		public UIViewBordered () : base()
		{
			this.CreateCurveAndShadow ();
		}
 
		// Constructor to use when creating in code
		public UIViewBordered (System.Drawing.RectangleF frame) : base(frame)
		{
			this.CreateCurveAndShadow ();
		}
 
		private void CreateCurveAndShadow ()
		{
			// MasksToBounds == false allows the shadow to appear outside the UIView frame
			this.Layer.MasksToBounds = false;
			this.Layer.CornerRadius = 10;
			this.Layer.ShadowColor = UIColor.DarkGray.CGColor;
			this.Layer.ShadowOpacity = 1.0f;
			this.Layer.ShadowRadius = 6.0f;
			this.Layer.ShadowOffset = new System.Drawing.SizeF (0f, 3f);
		}
	}
}