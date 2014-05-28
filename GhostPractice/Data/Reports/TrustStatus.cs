using System;
using System.Collections.Generic;

namespace GhostPractice
{
	public class TrustStatus
	{
		public TrustStatus ()
		{
		}

		public	 double trustCreditors { get; set; }

		public	double banksTotal { get; set; }

		public	List<Bank> banks { get; set; }

		public	double investments { get; set; }
	}
}

