using System;

namespace GhostAdmin
{
	public class SummaryDTO
	{
		public SummaryDTO ()
		{
		}
		
		public ActivityTypeDTO activityType { get; set; }

		public int total { get; set; }

		public long lastDate { get; set; }

		public long fromDate  { get; set; }

		public long  toDate  { get; set; }
	}
}

