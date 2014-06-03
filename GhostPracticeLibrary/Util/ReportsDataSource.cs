using System;
using System.Net;
using System.IO;
using System.Text;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using MonoTouch.Dialog;
using GhostPractice;

namespace GPMobilePad
{
	public class ReportsDataSource: IPagedViewDataSource
	{
		public ReportsDataSource ()
		{
		}

		public const int FINANCIAL = 1;
		public const int FEE_TARGET = 2;
		public const int MATTER_ANALYSIS = 3;

		public FinancialStatusReport financeReport { get; set; }

		public MatterAnalysisByOwnerReport matterAnalysisReport { get; set; }

		public FeeTargetProgressReport feeTargetReport { get; set; }

		private List<UIViewController> controllers = new List<UIViewController> ();

		public void setFinanceReportData (FinancialStatusReport financeReport)
		{
			this.financeReport = financeReport;
			createReports ();
		}

		public void setFeeTargetReportData (FeeTargetProgressReport feeTargetReport)
		{
			this.feeTargetReport = feeTargetReport;
			createReports ();
		}

		public void setMatterAnalysisReportData (MatterAnalysisByOwnerReport matterAnalysisReport)
		{
			this.matterAnalysisReport = matterAnalysisReport;
			createReports ();
		}

		private void createReports ()
		{
			controllers = new List<UIViewController> ();
			Console.WriteLine ("ReportsDataSource - createReports()");
			//controllers.Add (new PlaceHolder ());
			if (financeReport != null) {
				Console.WriteLine ("ReportsDataSource - createReports FINANCE");
				foreach (var branch in financeReport.branches) {
					var rept = new FinancialStatusByBranchController (branch);
					controllers.Add (rept);
				}
			}
			//
			if (feeTargetReport != null) {
				Console.WriteLine ("ReportsDataSource - createReports FEE TARGET");
				var practice = new FeeTargetProgressController (feeTargetReport);
				controllers.Add (practice);
				var branch = new FeeTargetProgressBranchController (feeTargetReport);
				controllers.Add (branch);
				var owner = new FeeTargetBranchOwnerController (feeTargetReport);
				controllers.Add (owner);
			}
			//
			if (matterAnalysisReport != null) {
				Console.WriteLine ("ReportsDataSource - createReports MATTER ANALYSIS");
				var practice = new MatterAnalysisController (matterAnalysisReport);
				controllers.Add (practice);
				var branch = new MatterAnalysisBranchController (matterAnalysisReport);
				controllers.Add (branch);
				var owner = new MatterAnalysisBranchOwnerController (matterAnalysisReport);
				controllers.Add (owner);
			}
		}

		public UIViewController GetPage (int pageNo)
		{
			return controllers [pageNo];
		}

		public void Reload ()
		{
		}

		public int Pages { get { return controllers.Count; } }
	}
}

