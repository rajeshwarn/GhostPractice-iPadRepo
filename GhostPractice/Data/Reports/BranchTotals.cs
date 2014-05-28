using System;

namespace GhostPractice
{
	public class BranchTotals
	{
		public BranchTotals ()
		{
		}

		public 	 RecordedMTD recordedMTD { get; set; }

		public 	double InvoicedMTDTotal { get; set; }

		public 	RecordedYTD recordedYTD { get; set; }

		public 	MatterActivity matterActivity { get; set; }

		public 	MatterBalances matterBalances { get; set; }
	}
}

