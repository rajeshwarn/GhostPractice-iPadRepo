using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Dialog;

namespace GhostPractice
{
	public partial class FeeTargetBranchOwnerController : DialogViewController
	{
		FeeTargetProgressReport report;
		float contentWidth;

		public FeeTargetBranchOwnerController (FeeTargetProgressReport report) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = false;
			this.report = report;
			contentWidth = View.Frame.Width + 5;
			buildReport ();
		}

		public static FinanceElement getElement (double amount, string caption)
		{
			var e = new FinanceElement (caption, amount);
			return e;
		}

		public void buildReport ()
		{
			Root = new RootElement ("Owner Fee Target Progress");	
			Stripper.SetReportHeader (Root, "Owner Fee Target Progress", null, contentWidth);

			for (int i = 0; i < report.branches.Count; i++) {
				Branch branch = report.branches [i];
				var s = new Section ();
				var t1 = new TitleElement (branch.name + " Branch Totals");
				s.Add (t1);
				Root.Add (s);
				
				for (int j = 0; j < branch.owners.Count; j++) {
					Owner o = branch.owners [j];					
					var ownerSection = new Section (); 
					var nm = new TitleElement (o.name);
					ownerSection.Add (nm);
					ownerSection.Add (new BigFinanceElement ("Invoiced MTD Total:  ", o.invoicedMTDTotal));
					var recMTD = new StringElement ("Recorded MTD");
					ownerSection.Add (recMTD);
					ownerSection.Add (getElement (o.recordedMTD.achieved, "Achieved:  "));
					ownerSection.Add (getElement (o.recordedMTD.estimatedTarget, "Estimated Target:  "));
					ownerSection.Add (getElement (o.recordedMTD.invoicedDebits, "Invoiced:  "));
					ownerSection.Add (getElement (o.recordedMTD.unbilled, "Unbilled:  "));
					ownerSection.Add (getElement (o.recordedMTD.total, "Total:  "));
					//
					var recYTD = new StringElement ("Recorded YTD");

					ownerSection.Add (recYTD);
					ownerSection.Add (getElement (o.recordedYTD.achieved, "Achieved:  "));
					ownerSection.Add (getElement (o.recordedYTD.estimatedTarget, "Estimated Target:  "));
					ownerSection.Add (getElement (o.recordedYTD.invoiced, "Invoiced:  "));
					ownerSection.Add (getElement (o.recordedYTD.unbilled, "Unbilled:  "));
					ownerSection.Add (getElement (o.recordedYTD.total, "Total:  "));
					
					Root.Add (ownerSection);
					
				}				
			}			
			
		}
	}
}
