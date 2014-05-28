using System;
using System.Collections.Generic;

namespace GhostAdmin
{
	public class ConsoleResponseDTO
	{
		public ConsoleResponseDTO ()
		{
		}

		public int responseCode { get; set; }

		public String responseMessage { get; set; }
    
		public List<ActivityLogDTO> activityLogs { get; set; }

		public List<ActivityTypeDTO> activityTypes { get; set; }

		public List<AppDTO> apps { get; set; }

		public List<CompanyDTO> companies { get; set; }

		public List<PlatformDTO> platforms { get; set; }

		public List<UserDTO> users { get; set; }
		
		
		public List<SummaryDTO> summaries { get; set; }
		public List<PlatformSummaryDTO> platformSummaries { get; set; }
	}
}

