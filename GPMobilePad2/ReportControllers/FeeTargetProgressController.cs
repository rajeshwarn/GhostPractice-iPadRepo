using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace GPMobilePad
{
	public partial class FeeTargetProgressController : DialogViewController
	{
		FeeTargetProgressReport report;

		public FeeTargetProgressController (FeeTargetProgressReport report) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = false;
			this.report = report;
			buildFeeTargetReport ();
		}

		public void buildFeeTargetReport ()
		{
			//NumberFormatInfo nfi = new CultureInfo ("en-US", false).NumberFormat;
			Root = new RootElement ("Fee Target Progress");
			var v = new UIView ();
			v.Frame = new RectangleF (10, 10, 600, 10); 
			var dummy = new Section (v);
			Root.Add (dummy);
			var headerLabel = new UILabel (new RectangleF (10, 10, 800, 48)) {
				Font = UIFont.BoldSystemFontOfSize (18),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = "Practice Fee Target Progress"
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 20, 800, 48);
			view.Add (headerLabel);
			Root.Add (new Section (view));
			
			PracticeTotals totals = this.report.practiceTotals;

			var ptLabel = new UILabel (new RectangleF (10, 10, 800, 24)) {
				Font = UIFont.BoldSystemFontOfSize (24),
				TextAlignment = UITextAlignment.Center,
				TextColor = ColorHelper.GetGPPurple (),
				Text = "Practice Totals"
			};
			
			var totSection = new Section ("Practice Totals");
			var tot1 = new BigFinanceElement ("Invoiced MTD Total:  ", totals.invoicedMTDTotal);
			
			totSection.Add (tot1);
			Root.Add (totSection);
			
			if (totals.recordedMTD != null) {
				
				var mtdSection = new Section ("Recorded MTD");
				mtdSection.Add (getElement (totals.recordedMTD.achieved, "Achieved: "));
				mtdSection.Add (getElement (totals.recordedMTD.estimatedTarget, "Estimated Target: "));
				mtdSection.Add (getElement (totals.recordedMTD.invoicedDebits, "Invoiced Debits: "));
				mtdSection.Add (getElement (totals.recordedMTD.unbilled, "Unbilled: "));
				mtdSection.Add (getElement (totals.recordedMTD.total, "Total: "));
				Root.Add (mtdSection);
			}
			if (totals.recordedYTD != null) {
				
				var mtdSection = new Section ("Recorded YTD");
				mtdSection.Add (getElement (totals.recordedYTD.achieved, "Achieved: "));
				mtdSection.Add (getElement (totals.recordedYTD.estimatedTarget, "Estimated Target: "));
				mtdSection.Add (getElement (totals.recordedYTD.invoiced, "Invoiced: "));
				mtdSection.Add (getElement (totals.recordedYTD.unbilled, "Unbilled: "));
				mtdSection.Add (getElement (totals.recordedYTD.total, "Total: "));
				Root.Add (mtdSection);
			}
			if (totals.matterActivity != null) {
				var matterActivitySection = new Section ("Matter Activity");
				var tot2 = new NumberElement (totals.matterActivity.active, "Active Matters: ");
				matterActivitySection.Add (tot2);
				var tot3 = new NumberElement (totals.matterActivity.deactivated, "Deactivated Matters: ");
				matterActivitySection.Add (tot3);
				var tot4 = new NumberElement (totals.matterActivity.newWork, "New Work: ");
				matterActivitySection.Add (tot4);
				var tot5 = new NumberElement (totals.matterActivity.noActivity, "No Activity: ");
				matterActivitySection.Add (tot5);
				var tot6 = new StringElement ("No Activity Duration:   " + totals.matterActivity.noActivityDuration);
				matterActivitySection.Add (tot6);
				Root.Add (matterActivitySection);
			}
			//
			if (totals.matterBalances != null) {
				var matterBalancesSection = new Section ("Matter Balances");
				matterBalancesSection.Add (getElement (totals.matterBalances.business, "Business: "));
				matterBalancesSection.Add (getElement (totals.matterBalances.unbilled, "Unbilled: "));
				matterBalancesSection.Add (getElement (totals.matterBalances.trust, "Trust Balance: "));
				matterBalancesSection.Add (getElement (totals.matterBalances.investment, "Investments: "));
				Root.Add (matterBalancesSection);
			}
			for (var i = 0; i < 10; i++) {
				Root.Add (new Section (" "));
			}
		}

		public static FinanceElement getElement (double amount, string caption)
		{
			var e = new FinanceElement (caption, amount);
			return e;
		}
	}
}
