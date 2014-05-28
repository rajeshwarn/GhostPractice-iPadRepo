using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace GPMobilePad
{
	public partial class FeeTargetProgressBranchController : DialogViewController
	{
		FeeTargetProgressReport report;

		public FeeTargetProgressBranchController (FeeTargetProgressReport report) : base (UITableViewStyle.Grouped, null)
		{
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
				Text = "Branch Fee Target Progress"
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 20, 800, 48);
			view.Add (headerLabel);
			Root.Add (new Section (view));
			
			for (int i = 0; i < report.branches.Count; i++) {
				Branch branch = report.branches [i];

				Section s = new Section (branch.name);
				
				var t2 = new BigFinanceElement ("Invoiced MTD Total:  ", branch.branchTotals.InvoicedMTDTotal);
				s.Add (t2);
				//
				Section s2 = new Section (branch.name + " Recorded MTD");
				s2.Add (getElement (branch.branchTotals.recordedMTD.achieved, "Achieved:  "));
				s2.Add (getElement (branch.branchTotals.recordedMTD.estimatedTarget, "Estimated Target:  "));
				s2.Add (getElement (branch.branchTotals.recordedMTD.invoicedDebits, "Invoiced Debits:  "));
				s2.Add (getElement (branch.branchTotals.recordedMTD.unbilled, "Unbilled:  "));
				s2.Add (getElement (branch.branchTotals.recordedMTD.total, "Total:  "));
				//
				Section s21 = new Section (branch.name + " Recorded YTD");
				s21.Add (getElement (branch.branchTotals.recordedYTD.achieved, "Achieved:  "));
				s21.Add (getElement (branch.branchTotals.recordedYTD.estimatedTarget, "Estimated Target:  "));
				s21.Add (getElement (branch.branchTotals.recordedYTD.invoiced, "Invoiced:  "));
				s21.Add (getElement (branch.branchTotals.recordedYTD.unbilled, "Unbilled:  "));
				s21.Add (getElement (branch.branchTotals.recordedYTD.total, "Total:  "));
				
				
				Root.Add (s);
				Root.Add (s2);
				Root.Add (s21);
				
				
			}
			for (var i = 0; i < 10; i++) {
				Root.Add (new Section (" "));
			}
		}
	}
} 