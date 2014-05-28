using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Dialog;

namespace GPMobilePad
{
	public partial class FeeTargetBranchOwnerController : DialogViewController
	{
		FeeTargetProgressReport report;

		public FeeTargetBranchOwnerController (FeeTargetProgressReport report) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = true;
			this.report = report;
			buildReport ();
		}

		public static FinanceElement getElement (double amount, string caption)
		{
			var e = new FinanceElement (caption, amount);
			return e;
		}

		public void buildReport ()
		{
			Root = new RootElement ("");
			var v = new UIView ();
			v.Frame = new RectangleF (10, 10, 600, 10); 
			var dummy = new Section (v);
			Root.Add (dummy);
			var headerLabel = new UILabel (new RectangleF (10, 10, 800, 48)) {
				Font = UIFont.BoldSystemFontOfSize (18),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = "Owner Fee Target Progress"
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 20, 800, 48);
			view.Add (headerLabel);
			Root.Add (new Section (view));
			
//			NumberFormatInfo nfi = new CultureInfo ("en-US", false).NumberFormat;

			for (int i = 0; i < report.branches.Count; i++) {
				Branch branch = report.branches [i];
				Section s = new Section (branch.name);
				//var t1 = new StringElement(branch.name + " Branch Totals");
				//s.Add(t1);
				Root.Add (s);
				
				for (int j = 0; j < branch.owners.Count; j++) {
					Owner o = branch.owners [j];					
					Section ownerSection = new Section (o.name); 
					ownerSection.Add (new BigFinanceElement ("Invoiced MTD Total:  ", o.invoicedMTDTotal));
					StyledStringElement recMTD = new StyledStringElement ("Recorded MTD");
					recMTD.BackgroundColor = UIColor.LightGray;
					recMTD.TextColor = UIColor.White;
					ownerSection.Add (recMTD);
					ownerSection.Add (getElement (o.recordedMTD.achieved, "Achieved:  "));
					ownerSection.Add (getElement (o.recordedMTD.estimatedTarget, "Estimated Target:  "));
					ownerSection.Add (getElement (o.recordedMTD.invoicedDebits, "Invoiced Debits:  "));
					ownerSection.Add (getElement (o.recordedMTD.unbilled, "Unbilled:  "));
					ownerSection.Add (getElement (o.recordedMTD.total, "Total:  "));
					//
					StyledStringElement recYTD = new StyledStringElement ("Recorded YTD");
					recYTD.BackgroundColor = UIColor.LightGray;
					recYTD.TextColor = UIColor.White;
					ownerSection.Add (recYTD);
					ownerSection.Add (getElement (o.recordedYTD.achieved, "Achieved:  "));
					ownerSection.Add (getElement (o.recordedYTD.estimatedTarget, "Estimated Target:  "));
					ownerSection.Add (getElement (o.recordedYTD.invoiced, "Invoiced:  "));
					ownerSection.Add (getElement (o.recordedYTD.unbilled, "Unbilled:  "));
					ownerSection.Add (getElement (o.recordedYTD.total, "Total:  "));
					
					Root.Add (ownerSection);
					
				}				
			}
			for (var i = 0; i < 10; i++) {
				Root.Add (new Section (" "));
			}
			
		}
	}
}
