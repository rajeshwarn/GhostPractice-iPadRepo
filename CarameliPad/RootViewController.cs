using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CarameliPad
{
	public partial class RootViewController : UITableViewController
	{
		static List<Property> propertyList;

		public event EventHandler<RowClickedEventArgs> RowClicked;

		public RootViewController () : base ("RootViewController", null)
		{
			this.Title = NSBundle.MainBundle.LocalizedString ("Listings", "Listings");
			this.ClearsSelectionOnViewWillAppear = false;
			this.ContentSizeForViewInPopover = new SizeF (320f, 600f);
			
			// Custom initialization
		}
		public class RowClickedEventArgs : EventArgs
		{
			public Property Item { get; set; }
                     
			public RowClickedEventArgs (Property property) : base()
			{
				this.Item = property;
				Console.WriteLine ("RootViewController: RowClickedEventArgs ....");
			}
		
		
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.TableView.RowHeight = 90;
			
			this.TableView.Source = new DataSource (this);
			this.TableView.SelectRow (NSIndexPath.FromRowSection (0, 0), false, UITableViewScrollPosition.Middle);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return true;
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}

		public void Talk (Property property)
		{
			if (RowClicked != null) {			
				RowClicked (this, new RowClickedEventArgs (property));
				
			} else {
				Console.WriteLine ("What the fuck makes the event fire?????");
			}
		}
		
		class DataSource : UITableViewSource
		{
			RootViewController controller;

			public DataSource (RootViewController controller)
			{
				this.controller = controller;
				Random rand = new Random ();
				propertyList = new List<Property> ();
				for (int i = 0; i < 16; i++) {
					Property p = new Property ();
					p.city = "Pretoria";
					p.suburb = "Hatfield";
					p.streetName = "Street Numero " + (i + 1);
					p.streetNo = "" + (rand.Next () / 10000000);
					p.postalCode = "02010";
					propertyList.Add (p);
				}
			}
			
			// Customize the number of sections in the table view.
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				return propertyList.Count;
			}
			
			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				string cellIdentifier = "Cell";
				var cell = tableView.DequeueReusableCell (cellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellIdentifier);
				}
				
				// Configure the cell.
				//set up image
				
				string path = "Images/99.png";
				UIImage img = UIImage.FromFile (path);
				

				cell.TextLabel.Text = propertyList [indexPath.Row].streetNo + " " + propertyList [indexPath.Row].streetName;
				cell.DetailTextLabel.Text = "This is a really nice house";
				cell.ImageView.Image = img;
				return cell;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				Console.WriteLine ("Property clicked: " + propertyList [indexPath.Row].streetName);
				if (controller.RowClicked != null) {
					controller.RowClicked (this, new RowClickedEventArgs (propertyList [indexPath.Row]));
				} else {
					Console.WriteLine ("When the fuck is RowClicked NOT fucking NULL?");
				}
				
				
			}
		}
		
	}

}