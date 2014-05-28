using System;
using System.Collections.Generic;


namespace GhostPractice
{
	public class WebServiceResponseDTO
	{
		public WebServiceResponseDTO ()
		{
		}

		public int responseCode { get; set; }
		
		public double fee { get; set; }
		
		public int activityID { get; set; }

		public long elapsedSeconds { get; set; }

		public String responseMessage { get; set; }
		
		public string deviceID { get; set; }

		public UserDTO user { get; set; }

		public AppDTO app { get; set; }

		public PlatformDTO platform { get; set; }
    
		public MatterDTO matter { get; set; }

		public List<MobileTariffCodeDTO> mobileTariffCodeList  { get; set; }

		public List<MobileUser> mobileUsers  { get; set; }

		public List<MatterSearchResultDTO> matterSearchList  { get; set; }
		
		public FeeTargetProgressReport feeTargetProgressReport { get; set; }
				
		public FinancialStatusReport financialStatusReport { get; set; }
		
		public MatterAnalysisByOwnerReport matterAnalysisByOwnerReport { get; set; }

		public bool taskCreated { get; set; }
		
		/*
		 * private  FeeTargetProgressReport feeTargetProgressReport;
    private MatterAnalysisByOwnerReport matterAnalysisByOwnerReport;
    private FinancialStatusReport financialStatusReport;
		 */ 
	}
}

