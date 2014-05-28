using System;
using GhostPractice.FeeTargetFix;

namespace GhostPractice
{
	public class Owner
	{
		public Owner ()
		{
		}

		public	string name { get; set; }

		public	string UserCode { get; set; }

		public	RecordedMTD recordedMTD { get; set; }

		public double invoicedMTDTotal { get; set; }
		
		public	RecordedYTD recordedYTD { get; set; }

		public	MatterActivity matterActivity { get; set; }

		public	MatterBalances matterBalances { get; set; }
	}
}

