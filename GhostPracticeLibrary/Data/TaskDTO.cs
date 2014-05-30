using System;

namespace GhostPractice
{
	public class TaskDTO
	{
		public String taskDescription { get; set; }
		public int userID { get; set; }
		public string matterID  { get; set; }
		public bool notifyWhenComplete { get; set; }
		public long dueDate { get; set; }
	}
}

