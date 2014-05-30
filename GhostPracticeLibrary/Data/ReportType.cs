using System;

namespace GhostPractice
{
	public class ReportType
	{
		public ReportType ()
		{
		}
		public  const int FINANCIAL_STATUS = 13;
		public const int MATTER_ANALYSIS_BY_OWNER = 202;
		public const int FEE_TARGET_PROGRESS = 205;
		
		public static string getType(int reportType) 
		{
			string name = null;
			switch(reportType) 
			{
			case FINANCIAL_STATUS:
				return "Financial Status";
			case MATTER_ANALYSIS_BY_OWNER:
				return "Matter Analysis By Owner";
			case FEE_TARGET_PROGRESS:
				return "Fee Target Progress";
				
			}
			return name;
		}
	}
}
