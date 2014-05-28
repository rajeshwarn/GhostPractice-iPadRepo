
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace GPMobilePad
{
	public class GPDatePicker
	{
		UIViewController _vc;
		UIPopoverController _pop;
		DateTime _dateTime;
		UIDatePicker _dp = null;
		
		public GPDatePicker (Element element)
		{
			_vc = new UIViewController ();
			_pop = new UIPopoverController (_vc);
			_pop.DidDismiss += delegate {
				OnDone ();
			};
			
			_vc.View.Frame = new RectangleF (0, 0, 340, 300);
			
			// Create Date Picker
			_dp = new UIDatePicker (new RectangleF (0, 0, 320, 300)); 
			_dp.Date = DateTime.Now;
			_dp.Mode = UIDatePickerMode.Date;
			_dp.ValueChanged += delegate {
				OnValueChanged ();
			};
			
			// Add to view
			_vc.View.AddSubview (_dp);
			
			// Present
			var activecell = element.GetActiveCell ();
			_pop.SetPopoverContentSize (new SizeF (_dp.Bounds.Width, _dp.Bounds.Height), true);
			_pop.PresentFromRect (activecell.Frame, activecell.Superview, UIPopoverArrowDirection.Left, true);
			
		}
		
		public void OnDone ()
		{
			
		}
		
		public void OnValueChanged ()
		{
		}
		
	}
}
