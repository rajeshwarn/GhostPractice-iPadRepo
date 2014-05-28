using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
namespace GhostPractice
{
	public class MoneyElement: Element
	{
		string caption { get; set;}
		double amount { get; set;}
		public MoneyElement (string caption, double amount) : base (null)
		{
			this.amount = amount;
			this.caption = caption;
		}
		// To release any heavy resources that you might have
    protected override void Dispose (bool disposing) {
			
		}

    // To retrieve the UITableViewCell for your element
    // you would need to prepare the cell to be reused, in the
    // same way that UITableView expects reusable cells to work
    public override UITableViewCell GetCell (UITableView tv)
		{
			
			var cell = base.GetCell (tv);
			var row = new MoneyRow (caption, amount);
			cell.AddSubview (row);
			return cell;
		}

    // To retrieve a "summary" that can be used with
    // a root element to render a summary one level up. 
    public override string Summary ()
		{
			
			return "Summary";
		}
    // To detect when the user has tapped on the cell
    public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path) {
			
			
		}
    // If you support search, to probe if the cell matches the user input
    public override bool Matches (string text)
		{
			
			return false;
	}
}

}