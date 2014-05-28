using System;

namespace GhostAdmin
{
	public class PlatformSummaryDTO
	{
		public PlatformSummaryDTO ()
		{
		}

		public PlatformDTO platform { get; set; }

		public int numberOfUsers { get; set; }

		public int numberActive { get; set; }

		public int numberInactive { get; set; }
	}
}

