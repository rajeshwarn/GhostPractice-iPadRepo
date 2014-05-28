using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace GhostAdmin
{
	partial class PlatformController : UITableViewController
	{
		protected PlatformTableSource tableSource;
		protected List<PlatformSummaryDTO> list;
		int companyID;
		#region Constructors
		
		public PlatformController(List<PlatformSummaryDTO> list, int companyID) 
		{
			this.list = list;
			this.companyID = companyID;
		}
		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public PlatformController (IntPtr handle) : base(handle)
		{
			
		}

		[Export("initWithCoder:")]
		public PlatformController (NSCoder coder) : base(coder)
		{
		}

		public PlatformController () : base (UITableViewStyle.Grouped)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "User Platforms";
			this.TableView.RowHeight = 70;
			this.CreateTablePlatFormItems ();
			this.TableView.Source = tableSource;

		}

		// Creates a set of table items.
		protected void CreateTablePlatFormItems ()
		{
			List<SummaryItemGroup> tablePlatFormItems = new List<SummaryItemGroup> ();

			// declare vars
			SummaryItemGroup tGroup, tGroup1, tGroup2, tGroup3;

			// 
			tGroup = new SummaryItemGroup() { Name = "iOS" };			
			tGroup.PlatFormItems.Add (list[4]); 
			tGroup.PlatFormItems.Add (list[5]);
			
			//
			tGroup1 = new SummaryItemGroup() { Name = "Android" };			
			tGroup1.PlatFormItems.Add (list[0]); 
			tGroup1.PlatFormItems.Add (list[1]); 
			
			//
			tGroup2 = new SummaryItemGroup() { Name = "BlackBerry" };			
			tGroup2.PlatFormItems.Add (list[2]); 
			tGroup2.PlatFormItems.Add (list[3]);
			
			//
			tGroup3 = new SummaryItemGroup() { Name = "Windows" };			
			tGroup3.PlatFormItems.Add (list[6]); 
			
			
			tablePlatFormItems.Add (tGroup);
			tablePlatFormItems.Add (tGroup1);
			tablePlatFormItems.Add (tGroup2);
			tablePlatFormItems.Add (tGroup3);


			tableSource = new PlatformTableSource(this,tablePlatFormItems, companyID);
		}
	}
}

