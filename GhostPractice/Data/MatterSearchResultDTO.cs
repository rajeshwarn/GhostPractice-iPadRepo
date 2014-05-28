using System;

namespace GhostPractice
{
	public class MatterSearchResultDTO
	{
		public MatterSearchResultDTO ()
		{
		}
		
		public string matterID { get; set; }

		public string matterName { get; set; }

		public string matterLegacyAccount { get; set; }

		public string clientName { get; set; }

		public string currentOwner { get; set; }
	}
}

