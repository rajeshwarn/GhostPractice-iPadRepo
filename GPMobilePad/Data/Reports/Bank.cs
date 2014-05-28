using System;

namespace GPMobilePad
{
	public class Bank
	{
		public Bank ()
		{
		}

		public string name { get; set; }

		public double balance { get; set; }

		public string dateReconciled { get; set; }

		public double reconciledAmount { get; set; }

		public double receiptsForPeriod { get; set; }
	}
}

