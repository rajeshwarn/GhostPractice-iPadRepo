using System;

namespace GhostAdmin
{
	public class ConsoleRequest
	{
		public ConsoleRequest ()
		{
		}

		public int requestType { get; set; }

		public long fromDate { get; set; }

		public long toDate { get; set; }

		public int companyID { get; set; }

		public int userID { get; set; }

		public int appID { get; set; }

		public int platformID { get; set; } 
		
		//constants
		public const int COMPANY_LIST = 1;
		public const int COMPANY_USER_LIST = 2;
		public const int COMPANY_APP_LIST = 3;
		//
		public const int ACTIVITY_LOGS_BY_COMPANY = 4;
		public const int ACTIVITY_LOGS_BY_USER = 5;
		public const int ACTIVITY_LOGS_BY_APP = 6;
		//
		public const int ACTIVITY_LOGS_BY_PLATFORM = 7;
		public const int ACTIVITY_LOGS_ALL = 8;
		//
		public const int ACTIVITY_SUMMARY_ALL = 9;
		public const int ACTIVITY_SUMMARY_COMPANY = 10;
		public const int ACTIVITY_SUMMARY_USER = 11;
		//

		public const int PLATFORM_SUMMARY_ALL = 12;
		public const int PLATFORM_SUMMARY_COMPANY = 13;
		//
		//
		public const int PLATFORM_USERS_ALL = 14;
		public const int PLATFORM_USERS_COMPANY = 15;
	}
}

