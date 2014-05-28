using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace GhostPractice
{
	public partial class FeeTargetProgressBranchController : DialogViewController
	{
		FeeTargetProgressReport report;
		float contentWidth, contentHeight;

		public FeeTargetProgressBranchController (FeeTargetProgressReport report) : base (UITableViewStyle.Grouped, null)
		{
			this.report = report;
			contentWidth = View.Frame.Width + 20;
			contentHeight = View.Frame.Height;
			buildReport ();
		}

		public static FinanceElement getElement (double amount, string caption)
		{
			var e = new FinanceElement (caption, amount);
			return e;
		}

		public void buildReport ()
		{

			Root = new RootElement ("Branch Fee Target Progress");	
			Stripper.SetReportHeader (Root, "Branch Fee Target Progress", null, contentWidth);
			
			for (int i = 0; i < report.branches.Count; i++) {
				Branch branch = report.branches [i];

				Section s = new Section ();
				var nm = new TitleElement (branch.name);
				s.Add (nm);
				
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