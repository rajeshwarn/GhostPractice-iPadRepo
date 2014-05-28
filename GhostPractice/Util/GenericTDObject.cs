using System;

namespace GhostPractice
{
	public class GenericTDObject
	{
		public GenericTDObject ()
		{
		}
		
		public RecordedMTD mtd {get; set;}
		public RecordedYTD ytd {get; set;}
		public MatterActivity matterActivity {get; set;}
		public MatterBalances matterBalances {get; set;}
	}
}

