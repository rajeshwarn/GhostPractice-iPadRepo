using System;
using System.Collections.Generic;

namespace GhostPractice
{
	/// <summary>
	/// A group that contains table items
	/// </summary>
	public class GenericTDGroup
	{
		public string Name { get; set; }

		public string Footer { get; set; }

		public List<GenericTDObject> Items
		{
			get { return items; }
			set { items = value; }
		}
		protected List<GenericTDObject> items = new List<GenericTDObject> ();
		
	}
}
