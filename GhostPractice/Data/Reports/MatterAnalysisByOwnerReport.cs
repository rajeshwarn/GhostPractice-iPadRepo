using System;
using System.Collections.Generic;

namespace GhostPractice
{
	public class MatterAnalysisByOwnerReport
	{
		public MatterAnalysisByOwnerReport ()
		{
		}
		
		public List<Branch> branches { get; set; }
		public PracticeTotals practiceTotals {get; set;}
	}
}

