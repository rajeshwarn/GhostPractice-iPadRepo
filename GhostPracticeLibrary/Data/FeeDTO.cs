using System;

namespace GhostPractice
{
	public class FeeDTO
	{
		public FeeDTO ()
		{
		}
		
		public int matterID { get; set; }

		public int duration { get; set; }

		public double amount { get; set; }

		public string narration { get; set; }

		public int tariffCodeID { get; set; }

		public long date { get; set; }
	}
}

