using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using GhostPractice;
using GhostPracticeLibrary;

namespace GPMobilePad
{
	public partial class MatterAnalysisBranchController : DialogViewController
	{
		MatterAnalysisByOwnerReport report;

		public MatterAnalysisBranchController (MatterAnalysisByOwnerReport report) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = false;
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
				Text = "Branch Matter Analysis"
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 20, 800, 48);
			view.Add (headerLabel);
			Root.Add (new Section (view));
			//
			for (int i = 0; i < report.branches.Count; i++) {
				Branch branch = report.branches [i];
				
				Section s = new Section (branch.name + " Branch Totals");				
				Root.Add (s);
				//
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
					matterActivitySection.Add (new NumberElement (branch.branchTotals.matterActivity.workedOn, "Worked On: "));

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

			for (var i = 0; i < 10; i++) {
				Root.Add (new Section (" "));
			}
		}
	}
} 