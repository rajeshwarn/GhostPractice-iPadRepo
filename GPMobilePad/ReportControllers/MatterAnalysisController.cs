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
	public partial class MatterAnalysisController : DialogViewController
	{
		MatterAnalysisByOwnerReport report;

		public MatterAnalysisController (MatterAnalysisByOwnerReport report) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = false;
			this.report = report;
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
			//NumberFormatInfo nfi = new CultureInfo ("en-US", false).NumberFormat;
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
				Text = "Practice Matter Analysis"
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 10, 800, 48);
			view.Add (headerLabel);
			Root.Add (new Section (view));

			
			PracticeTotals totals = this.report.practiceTotals;
			
			var totSection = new Section ("Practice Totals");
			Root.Add (totSection);
			
			
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
				matterBalancesSection.Add (getElement (totals.matterBalances.business, "Business: "));
				matterBalancesSection.Add (getElement (totals.matterBalances.trust, "Trust Balance: "));
				matterBalancesSection.Add (getElement (totals.matterBalances.investment, "Investments: "));
				matterBalancesSection.Add (getElement (totals.matterBalances.unbilled, "Unbilled: "));				
				matterBalancesSection.Add (getElement (totals.matterBalances.pendingDisbursements, "Pending Disb.: "));
				Root.Add (matterBalancesSection);
			}

			for (var i = 0; i < 10; i++) {
				Root.Add (new Section (" "));
			}
		}
	}
}
