using System;

namespace GhostPractice
{
	public class GetReportResponseDTO
	{
		public GetReportResponseDTO ()
		{
		}
		
		public	 int resultCode { get; set; }

		public	String resultMessage { get; set; }

		public	FeeTargetProgressReport feeTargetProgressReport { get; set; }

		public	MatterAnalysisByOwnerReport matterAnalysisByOwnerReport { get; set; }

		public	FinancialStatusReport financialStatusReport { get; set; }
	}
}

