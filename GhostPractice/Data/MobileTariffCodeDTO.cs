using System;

namespace GhostPractice
{
	public class MobileTariffCodeDTO
	{
		public MobileTariffCodeDTO ()
		{
		}
		public int id {get; set;}
		public int tariffType {get; set;}
		public string name {get; set;}
		public string narration {get; set;}
		public double amount {get; set;}
		public bool timeBasedCode {get; set;}
		public double units {get; set;}
		public bool surchargeApplies {get; set;}
		
	
	}
}
