using System;
using System.Collections.Generic;

namespace GhostPractice
{
	public class BusinessStatus
	{
		public BusinessStatus ()
		{
		}

		public 	double businessDebtors;
		public 	double businessCreditors;
		public 	double banksTotal;
		public 	List<Bank> banks;
		public 	double vat;
		public 	double unbilled;
		public 	double pendingDisbursements;
		public 	double availableForTransfer;
	}
}

