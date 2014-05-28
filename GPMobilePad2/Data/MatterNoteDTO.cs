using System;

namespace GPMobilePad
{
	public class MatterNoteDTO
	{
		public MatterNoteDTO ()
		{
		}

		public int matterID { get; set; }

		public string narration { get; set; }

		public long date { get; set; }

		public int tariffCodeID { get; set; }
	}
}

