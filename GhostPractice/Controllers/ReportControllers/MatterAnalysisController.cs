using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using GhostPracticeLibrary;

namespace GhostPractice
{
	public partial class MatterAnalysisController : DialogViewController
	{
		MatterAnalysisByOwnerReport report;
		float contentWidth;

		public MatterAnalysisController (MatterAnalysisByOwnerReport report) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = false;
			this.report = report;
			contentWidth = View.Frame.Width;
			buildFeeTargetReport ();
		}

		public static FinanceElement getElement (double amount, string caption)
		{
			var element = new FinanceElement (caption, amount);
			return element;
		}
		/*
		 * "practiceTotals":{"invoicedMTDTotal":0.0,
	"matterActivity":{"active":581,"workedOn":529,"newWork":581,"deactivated":0,"noActivity":0,"noActivityDuration":"6 Months"},
	"matterBalances":{"unbilled":107253.15,"pendingDisbursements":500.0,"investment":-1000.0,"trust":-5852921.0,"business":704265.82}}}}
		 */
		public void buildFeeTargetReport ()
		{
			Root = new RootElement ("Practice Matter Analysis");	
			Stripper.SetReportHeader (Root, "Practice Matter Analysis", null, contentWidth);
			
			PracticeTotals totals = this.report.practiceTotals;
			
			//var totSection = new Section ("Practice Totals");
			//Root.Add (totSection);
			
			
			if (totals.matterActivity != null) {
				var matterActivitySection = new Section ();
				var mTitle = new TitleElement ("Matter Activity");
				matterActivitySection.Add (mTitle);
				matterActivitySection.Add (new NumberElement (totals.matterActivity.active, "Active:  "));
				matterActivitySection.Add (new NumberElement (totals.matterActivity.deactivated, "Deactivated:  "));
				matterActivitySection.Add (new NumberElement (totals.matterActivity.newWork, "New Work:  "));
					
				matterActivitySection.Add (new NumberElement (totals.matterActivity.workedOn, "Worked On:  "));
				matterActivitySection.Add (new NumberElement (totals.matterActivity.noActivity, "No Activity:  "));
				matterActivitySection.Add (new StringElement ("No Activity Duration:  " + totals.matterActivity.noActivityDuration));
				Root.Add (matterActivitySection);
			}
			//
			if (totals.matterBalances != null) {
				var matterBalancesSection = new Section ();
				var mTitle = new TitleElement ("Matter Balances");
				matterBalancesSection.Add (mTitle);

				matterBalancesSection.Add (getElement (totals.matterBalances.business, S.GetText (S.BUSINESS) + ": "));
				matterBalancesSection.Add (getElement (totals.matterBalances.trust, S.GetText (S.TRUST_BALANCE) + ": "));
				matterBalancesSection.Add (getElement (totals.matterBalances.investment, S.GetText (S.INVESTMENTS) + ": "));
				matterBalancesSection.Add (getElement (totals.matterBalances.unbilled, "Unbilled: "));				
				matterBalancesSection.Add (getElement (totals.matterBalances.pendingDisbursements, "Pending Disb.: "));
				Root.Add (matterBalancesSection);
			}
		
		}
	}
}
