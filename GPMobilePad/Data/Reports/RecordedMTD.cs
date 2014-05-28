using System;

namespace GPMobilePad
{
	public class RecordedMTD
	{
		public RecordedMTD ()
		{
		}
		
		public	 double unbilled { get; set; }

		public	 double invoicedDebits { get; set; }

		public	double total { get; set; }

		public	double estimatedTarget { get; set; }

		public	double achieved { get; set; }
	}
}

