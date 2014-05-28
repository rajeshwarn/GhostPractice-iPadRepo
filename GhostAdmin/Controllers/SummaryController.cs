using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace GhostAdmin
{
	partial class SummaryController : UITableViewController
	{
		protected TableSource tableSource;
		protected List<SummaryDTO> list;
		#region Constructors
		
		public SummaryController(List<SummaryDTO> list) 
		{
			this.list = list;
			
		}
		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public SummaryController (IntPtr handle) : base(handle)
		{
			
		}

		[Export("initWithCoder:")]
		public SummaryController (NSCoder coder) : base(coder)
		{
		}

		public SummaryController () : base (UITableViewStyle.Grouped)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Activity (90 Days)";
			this.TableView.RowHeight = 70;
			this.CreateTableItems ();
			this.TableView.Source = tableSource;

		}

		// Creates a set of table items.
		protected void CreateTableItems ()
		{
			List<TableItemGroup> tableItems = new List<TableItemGroup> ();

			// declare vars
			TableItemGroup tGroup, tGroup1, tGroup2;

			// 
			tGroup = new TableItemGroup() { Name = "Provisioning" };			
			tGroup.Items.Add (list[2]); 
			
			//
			tGroup1 = new TableItemGroup() { Name = "Postings" };			
			tGroup1.Items.Add (list[4]); 
			tGroup1.Items.Add (list[6]); 
			tGroup1.Items.Add (list[5]); 
			
			//
			tGroup2 = new TableItemGroup() { Name = "Search" };			
			tGroup2.Items.Add (list[0]); 
			tGroup2.Items.Add (list[3]);
			tGroup2.Items.Add (list[7]);
			tGroup2.Items.Add (list[1]);
			
			
			tableItems.Add (tGroup1);
			tableItems.Add (tGroup2);
			tableItems.Add (tGroup);


			tableSource = new TableSource(tableItems);
		}
	}
}

