using System;

namespace GPMobilePad
{
	public class MatterDTO
	{
		public MatterDTO ()
		{
		}
		public string id { get; set; }
		public string matterName { get; set; }
		
		public double businessBalance { get; set; }
		public double trustBalance { get; set; }
		public double unbilledBalance { get; set; }
		public double reserveTrust { get; set; }
		
		public double investmentTrustBalance { get; set; }
		public double pendingDisbursementBalance { get; set; }
		public double currentBalance { get; set; }
		
		public double surchargeFactor { get; set; }
		public string ourReference { get; set; }
		
		public string yourReference { get; set; }
		public string legacyAccount { get; set; }
		public int clientNumber { get; set; }
		
	
	}
}
