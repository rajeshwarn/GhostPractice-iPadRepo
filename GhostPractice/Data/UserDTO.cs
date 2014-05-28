using System;

namespace GhostPractice
{
	public class UserDTO
	{
		public UserDTO ()
		{
		}

		public int userID { get; set; }

		public int gpID { get; set; }

		public int numberApps { get; set; }

		public int statusFlag { get; set; }

		public string userName { get; set; }

		public string email { get; set; }

		public string cellphone { get; set; }
		

		public long dateRegistered { get; set; }

		public CompanyDTO company { get; set; }
	}
}

