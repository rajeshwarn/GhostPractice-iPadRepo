using System;
using System.Collections.Generic;

namespace GhostAdmin
{
	/// <summary>
	/// A group that contains table items
	/// </summary>
	public class TableItemGroup
	{
		public string Name { get; set; }

		public string Footer { get; set; }

		public List<SummaryDTO> Items
		{
			get { return items; }
			set { items = value; }
		}
		protected List<SummaryDTO> items = new List<SummaryDTO> ();
		
	}
}
