using System;
using System.Collections.Generic;

namespace GhostAdmin
{
	/// <summary>
	/// A group that contains table items
	/// </summary>
	public class SummaryItemGroup
	{
		public string Name { get; set; }

		public string Footer { get; set; }

		//
		public List<PlatformSummaryDTO> PlatFormItems
		{
			get { return pItems; }
			set { pItems = value; }
		}
		protected List<PlatformSummaryDTO> pItems = new List<PlatformSummaryDTO> ();
	}
}
