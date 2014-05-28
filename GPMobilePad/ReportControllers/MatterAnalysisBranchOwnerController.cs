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
	public partial class MatterAnalysisBranchOwnerController : DialogViewController
	{
		MatterAnalysisByOwnerReport report;

		public MatterAnalysisBranchOwnerController (MatterAnalysisByOwnerReport report) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = false;
			this.report = report;
			buildReport ();
		}

		public static FinanceElement getElement (double amount, string caption)
		{
			var element = new FinanceElement (caption, amount);
			return element;
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
				Text = "Owner Matter Analysis"
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 10, 800, 48);
			view.Add (headerLabel);
			Root.Add (new Section (view));
			
			//NumberFormatInfo nfi = new CultureInfo ("en-US", false).NumberFormat;
			for (int i = 0; i < report.branches.Count; i++) {
				Branch branch = report.branches [i];
				Section s = new Section (branch.name);
				Root.Add (s);
				
				for (int j = 0; j < branch.owners.Count; j++) {
					Owner o = branch.owners [j];
					Section seco = new Section (o.name); 

					var recMTD = new TitleElement ("Matter Activity");
					seco.Add (recMTD);
					seco.Add (new NumberElement (o.matterActivity.active, "Active:  "));
					seco.Add (new NumberElement (o.matterActivity.deactivated, "Deactivated:  "));
					seco.Add (new NumberElement (o.matterActivity.newWork, "New Work:  "));					
					seco.Add (new NumberElement (o.matterActivity.workedOn, "Worked On:  "));
					seco.Add (new NumberElement (o.matterActivity.noActivity, "No Activity:  "));
					seco.Add (new StringElement ("No Activity Duration:  " + o.matterActivity.noActivityDuration));
					//
					var recYTD = new TitleElement ("Matter Balances");					
					seco.Add (recYTD);
					seco.Add (getElement (o.matterBalances.business, "Business:  "));
					seco.Add (getElement (o.matterBalances.trust, "Trust:  "));
					seco.Add (getElement (o.matterBalances.investment, "Investment:  "));
					seco.Add (getElement (o.matterBalances.unbilled, "Unbilled:  "));					
					seco.Add (getElement (o.matterBalances.pendingDisbursements, "Pending Disb:  "));
					
					
					Root.Add (seco);
					
				}

			}	

			for (var i = 0; i < 10; i++) {
				Root.Add (new Section (" "));
			}
		}
	}
}
