using System;

namespace GhostAdmin
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

		public String userName { get; set; }

		public String  email { get; set; }

		public String cellphone { get; set; }

		public long dateRegistered { get; set; }

		public CompanyDTO company { get; set; }
	}
}

