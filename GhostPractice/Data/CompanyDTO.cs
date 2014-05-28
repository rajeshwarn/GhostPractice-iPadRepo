using System;

namespace GhostPractice
{
	public class CompanyDTO
	{
		public CompanyDTO ()
		{
		}
		
		public int companyID { get; set; }

		public int gpID { get; set; }

		public int numberUsers { get; set; }

		public int statusFlag { get; set; }

		public string companyName { get; set; }

		public string email { get; set; }

		public string cellphone { get; set; }

		public long dateRegistered { get; set; }
	}
}

