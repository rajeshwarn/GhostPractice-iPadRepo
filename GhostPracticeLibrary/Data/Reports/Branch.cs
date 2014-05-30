using System;
using System.Collections.Generic;


namespace GhostPractice
{
	public class Branch
	{
		public Branch ()
		{
		}

		public string name;

		public List<Owner> owners { get; set; }

		public BranchTotals branchTotals { get; set; }

		public BusinessStatus businessStatus { get; set; }

		public TrustStatus trustStatus { get; set; }
		
		
		public const int BRANCH_TOTALS = 1;
		public const int OWNER = 2;
		public const int BUSINESS_STATUS = 3;
		public const int TRUST_STATUS = 4;
		
		
	}
}

