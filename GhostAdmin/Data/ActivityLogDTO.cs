using System;

namespace GhostAdmin
{
	public class ActivityLogDTO
	{
		public ActivityLogDTO ()
		{
		}

		public int activityLogID { get; set; }

		public int successFlag { get; set; }

		public long dateStamp { get; set; }

		public AppDTO app { get; set; }

		public UserDTO user { get; set; }

		public ActivityTypeDTO activityType  { get; set; } 
	}
}

