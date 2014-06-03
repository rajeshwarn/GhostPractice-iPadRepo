using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using GhostPracticeLibrary;

namespace GhostPractice
{
	public partial class MatterAnalysisBranchController : DialogViewController
	{
		MatterAnalysisByOwnerReport report;
		float contentWidth;

		public MatterAnalysisBranchController (MatterAnalysisByOwnerReport report) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = false;
			this.report = report;
			contentWidth = View.Frame.Width + 20;
			;
			buildReport ();
		}

		public static FinanceElement getElement (double amount, string caption)
		{
			var e = new FinanceElement (caption, amount);
			return e;
		}

		public void buildReport ()
		{
			Root = new RootElement ("Branch Matter Analysis");	
			Stripper.SetReportHeader (Root, "Branch Matter Analysis", null, contentWidth);

			//
			for (int i = 0; i < report.branches.Count; i++) {
				Branch branch = report.branches [i];				
				var s = new TitleElement (branch.name + " Branch Totals");	
				var sec = new Section ();
				sec.Add (s);
				Root.Add (sec);
				if (branch.branchTotals.matterActivity != null) {
					var matterActivitySection = new Section ();
					var mTitle = new TitleElement ("Matter Activity");
					matterActivitySection.Add (mTitle);
					matterActivitySection.Add (new NumberElement (branch.branchTotals.matterActivity.active, "Active Matters: "));
					matterActivitySection.Add (new NumberElement (
						branch.branchTotals.matterActivity.deactivated,
						"Deactivated Matters: "
					));
					matterActivitySection.Add (new NumberElement (branch.branchTotals.matterActivity.newWork, "New Work: "));
					matterActivitySection.Add (new NumberElement (branch.branchTotals.matterActivity.noActivity, "No Activity: "));
					matterActivitySection.Add (new StringElement ("No Activity Duration: " + branch.branchTotals.matterActivity.noActivityDuration));
					Root.Add (matterActivitySection);
				}
				//
				if (branch.branchTotals.matterBalances != null) {
					var matterBalancesSection = new Section ();
					var mTitle = new TitleElement ("Matter Balances");
					matterBalancesSection.Add (mTitle);
					matterBalancesSection.Add (getElement (branch.branchTotals.matterBalances.business, S.GetText (S.BUSINESS) + ": "));
					matterBalancesSection.Add (getElement (branch.branchTotals.matterBalances.trust, S.GetText (S.TRUST_BALANCE) + ": "));
					matterBalancesSection.Add (getElement (branch.branchTotals.matterBalances.investment, S.GetText (S.INVESTMENTS) + ": "));
					matterBalancesSection.Add (getElement (branch.branchTotals.matterBalances.unbilled, "Unbilled: "));
					matterBalancesSection.Add (getElement (branch.branchTotals.matterBalances.pendingDisbursements, "Pending Disb.: "));
					Root.Add (matterBalancesSection);
				}
				
				
			}
		}
	}
} 