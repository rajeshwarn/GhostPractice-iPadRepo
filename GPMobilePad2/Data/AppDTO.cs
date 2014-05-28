using System;

namespace GPMobilePad
{
	public class AppDTO
	{
		public AppDTO ()
		{
		}
		
		public int appID { get; set; }

		public int numberUsers { get; set; }

		public string appName { get; set; }

		public string version { get; set; }

		public long dateRegistered { get; set; }
	}
}

