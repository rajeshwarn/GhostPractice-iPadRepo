using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Dialog;
using GhostPracticeLibrary;

namespace GhostPractice
{
	public partial class MatterAnalysisBranchOwnerController : DialogViewController
	{
		MatterAnalysisByOwnerReport report;
		float contentWidth;

		public MatterAnalysisBranchOwnerController (MatterAnalysisByOwnerReport report) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = false;
			this.report = report;
			contentWidth = View.Frame.Width;
			buildReport ();
		}

		public static FinanceElement getElement (double amount, string caption)
		{
			var element = new FinanceElement (caption, amount);
			return element;
		}

		public void buildReport ()
		{
			Root = new RootElement ("Owner Matter Analysis");	
			Stripper.SetReportHeader (Root, "Owner Matter Analysis", null, contentWidth);
			
			//NumberFormatInfo nfi = new CultureInfo ("en-US", false).NumberFormat;
			for (int i = 0; i < report.branches.Count; i++) {
				Branch branch = report.branches [i];
				TitleElement s = new TitleElement (branch.name, TitleElement.MEDIUM_FONT);
				var sec = new Section ();
				sec.Add (s);
				Root.Add (sec);
				
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
					seco.Add (getElement (o.matterBalances.business, S.GetText (S.BUSINESS) + ":  "));
					seco.Add (getElement (o.matterBalances.trust, S.GetText (S.TRUST) + ":  "));
					seco.Add (getElement (o.matterBalances.investment, S.GetText (S.INVESTMENTS) + ":  "));
					seco.Add (getElement (o.matterBalances.unbilled, "Unbilled:  "));					
					seco.Add (getElement (o.matterBalances.pendingDisbursements, "Pending Disb:  "));
					
					
					Root.Add (seco);
					
				}
				
			}			
			
		}
	}
}
