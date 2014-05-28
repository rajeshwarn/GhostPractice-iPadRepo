using System;

namespace GhostPractice
{
	public class GhostRequestDTO
	{
		public GhostRequestDTO ()
		{
		}
		
		public int requestType { get; set; }
		
		public int activityID { get; set; }
		
		public int tariffCodeID { get; set; }
		
		public double deviceElapsedSeconds { get; set; }

		public string deviceID { get; set; }

		public string appName  { get; set; }

		public string platformName { get; set; }

		public string searchString { get; set; }

		public string matterID { get; set; }

		public int appID { get; set; }

		public int platformID { get; set; }

		public int userID { get; set; }
		
		public int companyID { get; set; }

		public double amount { get; set; }

		public FeeDTO fee { get; set; }

		public MatterNoteDTO note { get; set; }

		public String activationCode { get; set; }
		
		public double latitude { get; set; }
		
		public double longitude { get; set; }
		
		public int tarrifCodeType { get; set; }
		
		public int reportType { get; set; }
		
		public int duration { get; set; }

		public TaskDTO task { get; set; }
		
		//
		
		public const int PROVISION_NEW_USER = 110;
		public const int PING = 111;
		public const int POST_NOTE = 112;
		public const int POST_FEE = 113;
		public const int POST_UNBILLABLE_FEE = 114;
		public const int FIND_MATTER = 115;
		public const int GET_MATTER_DETAIL = 116;
		public const int GET_TARIFF_CODES = 117;
		public const int GET_APP_PLATFORM_IDS = 109;
		public const int GET_REPORT = 120;
		public const int POST_DEVICE_ELAPSED_TIME = 121;
		public const int CALCULATE_FEE = 122;

		public const int GET_FEE_EARNERS = 123;
		public const int ASSIGN_TASK = 124;
		//
		public const string IPHONE = "iPhone";
		public const string IPAD = "iPad";
		public const string ANDROID_PHONE = "Android Phone";
		public const string ANDROID_TABLET = "Android Tablet";
		public const string BLACKBERRY = "Blackberry";
		public const string BLACKBERRY_TABLET = "Blackberry Tablet";
		public const string WINDOWS_PHONE = "Windows Phone";
	}
}

