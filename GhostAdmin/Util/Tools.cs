using System;

namespace GhostAdmin
{
	public class Tools
	{
		public Tools ()
		{
		}
		
		public static DateTime ConvertJavaMiliSecondToDateTime (long javaMS)
		{

			DateTime UTCBaseTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			DateTime dt = UTCBaseTime.Add (new TimeSpan (javaMS * TimeSpan.TicksPerMillisecond)).ToLocalTime ();

			return dt;

		}
		public static long ConvertDateTimeToJavaMS (DateTime date)
		{

			DateTime UTCBaseTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			
			TimeSpan ts= date - UTCBaseTime; 
			long ms = (long)ts.TotalMilliseconds; 

			return ms;

		}
		public static DateTime GetDate(int days) 
		{
			DateTime dt = DateTime.Now.AddDays(- days); 	
			return dt;
		}
		
		public const int NUMBER_OF_DAYS = 90;
		public const string CONSOLE_URL = "http://10.0.0.239:8080/GhostPractice-war/console?json=";
		//public const string CONSOLE_URL = "http://centos5.boha.za.com:8080/GhostPractice-war/console?json=";
	}
}

