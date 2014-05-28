using System;

namespace GhostAdmin
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

		public String companyName { get; set; }

		public String email { get; set; }

		public String cellphone { get; set; }

		public long dateRegistered { get; set; } 
	}
}

