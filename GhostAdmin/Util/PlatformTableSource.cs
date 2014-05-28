using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace GhostAdmin
{
	/// <summary>
	/// Combined DataSource and Delegate for our UITableView
	/// </summary>
	public class PlatformTableSource : UITableViewSource
	{
		//---- declare vars
		UIViewController controller;
		protected List<SummaryItemGroup> tableItems;
		protected string cellIdentifier = "TableCell";
		int companyID;

		public PlatformTableSource (UIViewController controller, List<SummaryItemGroup> items, int companyID)
		{
			tableItems = items;
			this.controller = controller;
			this.companyID = companyID;
		}

		#region data binding/display methods

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override int NumberOfSections (UITableView tableView)
		{
			return tableItems.Count;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems [section].PlatFormItems.Count;
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
			//new UIAlertView ("Row Selected"
			//	, tableItems [indexPath.Section].PlatFormItems [indexPath.Row].platform.platformName, null, "OK", null).Show ();
			var cont = new PlatformUserController(tableItems [indexPath.Section].PlatFormItems [indexPath.Row].platform, companyID);
			controller.NavigationController.PushViewController(cont, true);
		}

		public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
		{
			//Console.WriteLine ("Row " + indexPath.Row.ToString () + " deselected");	
		}

		#endregion	

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			//---- declare vars
			TDBadgedCell cell = (TDBadgedCell)tableView.DequeueReusableCell (cellIdentifier);

			//---- if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new TDBadgedCell (UITableViewCellStyle.Subtitle, cellIdentifier);
			
			PlatformSummaryDTO dto = tableItems [indexPath.Section].PlatFormItems [indexPath.Row];
			//---- set the item text
			cell.TextLabel.Text = dto.platform.platformName;
				
			cell.DetailTextLabel.Text = "Inactive Users: " + dto.numberInactive;
			cell.DetailTextLabel.Font = UIFont.SystemFontOfSize (12);
			
			if (dto.numberOfUsers > 0) {
				cell.Accessory = UITableViewCellAccessory.Checkmark;
			}
			
			UIImage img = UIImage.FromFile ("Images/32-iphone.png");
			if (dto.platform.platformName.Contains ("iPhone") || dto.platform.platformName.Contains ("iPad")) {
				img = UIImage.FromFile ("Images/apple48-blue.png");	
			}
			if (dto.platform.platformName.Contains ("Android")) {
				img = UIImage.FromFile ("Images/android48-round.png");	
			}
			if (dto.platform.platformName.Contains ("Blackberry")) {
				img = UIImage.FromFile ("Images/blackberry.png");	
			}
			if (dto.platform.platformName.Contains ("Windows")) {
				img = UIImage.FromFile ("Images/windows.png");	
			}
			
			
			cell.ImageView.Image = img;
			cell.BadgeNumber = Convert.ToInt32 (dto.numberOfUsers);
 
			if (indexPath.Row == 1)
				cell.BadgeColor = UIColor.FromRGBA (1.000f, 0.397f, 0.419f, 1.000f);
			if (indexPath.Row == 2)
				cell.BadgeColor = UIColor.FromWhiteAlpha (0.783f, 1.000f);
			if (indexPath.Row == 3)
				cell.BadgeColor = UIColor.FromRGBA (1.000f, 0.333f, 0.666f, 1.000f);

			return cell;
		}
	}
}
