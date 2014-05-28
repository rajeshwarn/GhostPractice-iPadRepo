using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace GhostPractice
{
	/// <summary>
	/// Combined DataSource and Delegate for our UITableView
	/// </summary>
	public class GenericTDSource : UITableViewSource
	{
		//---- declare vars
		protected List<GenericTDGroup> tableItems;
		protected string cellIdentifier = "TableCell";

		public GenericTDSource (List<GenericTDGroup> items)
		{
			tableItems = items;
		}

		#region data binding/display methods

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override int NumberOfSections (UITableView tableView)
		{
			return 4;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems [section].Items.Count;
		}

		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
		public override string TitleForHeader (UITableView tableView, int section)
		{
			return tableItems [section].Name;
		}

		/// <summary>
		/// Called by the TableView to retrieve the footer text for the particular section(group)
		/// </summary>
		public override string TitleForFooter (UITableView tableView, int section)
		{
			return tableItems [section].Footer;
		}

		#endregion	

		#region user interaction methods

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
//			new UIAlertView ("Row Selected"
//				, tableItems [indexPath.Section].Items [indexPath.Row].activityType.activityTypeName, null, "OK", null).Show ();
		}

		public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
		{
			Console.WriteLine ("Row " + indexPath.Row.ToString () + " deselected");	
		}

		#endregion	

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		/*
		 * public override UITableViewCell GetCell 
   (UITableView tableView, NSIndexPath indexPath)
{
   TDBadgedCell cell = new TDBadgedCell (UITableViewCellStyle.Subtitle, "Cell");
   cell.TextLabel.Text = contents[indexPath.Row].Title;
   cell.TextLabel.Font = UIFont.BoldSystemFontOfSize (14);
 
   cell.DetailTextLabel.Text = contents[indexPath.Row].Detail;
   cell.DetailTextLabel.Font = UIFont.SystemFontOfSize (13);
 
   cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
   cell.BadgeNumber = Convert.ToInt32 (contents[indexPath.Row].Badge);
 
   if (indexPath.Row == 1)
      cell.BadgeColor = UIColor.FromRGBA (1.000f, 0.397f, 0.419f, 1.000f);
   if (indexPath.Row == 2)
      cell.BadgeColor = UIColor.FromWhiteAlpha (0.783f, 1.000f);
   return cell;
}
		 */ 
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			Console.WriteLine ("Calling Get Cell, isEditing:" + tableView.Editing);

			//---- declare vars
			TDBadgedCell cell = (TDBadgedCell)tableView.DequeueReusableCell (cellIdentifier);

			//---- if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new TDBadgedCell (UITableViewCellStyle.Subtitle, cellIdentifier);
			
			//cell.AddSubview

//			//---- set the item text
//			cell.TextLabel.Text = tableItems [indexPath.Section].Items [indexPath.Row].activityType.activityTypeName;
//			DateTime dd = Tools.ConvertJavaMiliSecondToDateTime (tableItems [indexPath.Section].Items [indexPath.Row].lastDate);
//			
//			if (tableItems [indexPath.Section].Items [indexPath.Row].lastDate > 0) {
//				cell.DetailTextLabel.Text = dd.ToLongDateString () + " - " + dd.ToShortTimeString();
//				cell.DetailTextLabel.Font = UIFont.SystemFontOfSize (11);
//				cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
//				
//			}
//			UIImage img = UIImage.FromFile ("Images/258-checkmark.png");
//			cell.ImageView.Image = img;
//			cell.BadgeNumber = Convert.ToInt32 (tableItems [indexPath.Section].Items [indexPath.Row].total);
// 
//			if (indexPath.Row == 1)
//				cell.BadgeColor = UIColor.FromRGBA (1.000f, 0.397f, 0.419f, 1.000f);
//			if (indexPath.Row == 2)
//				cell.BadgeColor = UIColor.FromWhiteAlpha (0.783f, 1.000f);
//			if (indexPath.Row == 3)
//				cell.BadgeColor = UIColor.FromRGBA (1.000f, 0.333f, 0.666f, 1.000f);

			return cell;
		}
	}
}
