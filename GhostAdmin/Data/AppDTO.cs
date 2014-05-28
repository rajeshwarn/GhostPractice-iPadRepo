using System;

namespace GhostAdmin
{
	public class AppDTO
	{
		public AppDTO ()
		{
		}

		public int appID { get; set; }

		public int numberUsers { get; set; }

		public  String appName { get; set; }

		public  String version { get; set; }

		public  String serviceURL { get; set; }

		public long dateRegistered { get; set; }
	}
}

