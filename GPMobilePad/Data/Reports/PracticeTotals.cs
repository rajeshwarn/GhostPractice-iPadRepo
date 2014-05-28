using System;

namespace GPMobilePad
{
	public class PracticeTotals
	{
		public PracticeTotals ()
		{
		}

		public	 RecordedMTD recordedMTD { get; set; }

		public	double invoicedMTDTotal { get; set; }

		public	RecordedYTD recordedYTD { get; set; }

		public	MatterActivity matterActivity { get; set; }

		public	MatterBalances matterBalances { get; set; }
	}
}

