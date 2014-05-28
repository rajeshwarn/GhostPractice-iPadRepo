using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace GhostPractice
{
	public class BranchHelper
	{
		public BranchHelper ()
		{
		}
	}
	//
// I create a view that renders my data, as this allows me to reuse
// the data rendering outside of a UITableViewCell context as well
//
	public class BranchView : UIView
	{
		Branch branch;

		public BranchView (Branch branch)
		{
			Update (branch);
		}

		// Public method, that allows the code to externally update
		// what we are rendering.   
		public void Update (Branch branch)
		{
			this.branch = branch;
			SetNeedsDisplay ();
		}
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			UILabel label = new UILabel();
			label.Text = "Active matters: " + branch.branchTotals.matterActivity.active;
			this.AddSubview(label);
		}
	}

//
// This is the actual UITableViewCell class
//
	public class BranchCell : UITableViewCell
	{
		BranchView branchView;

		public BranchCell (Branch branch, NSString identKey) : base (UITableViewCellStyle.Default, identKey)
		{
			// Configure your cell here: selection style, colors, properties
			branchView = new BranchView (branch);
			ContentView.Add (branchView);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			branchView.Frame = ContentView.Bounds;
			
			branchView.SetNeedsDisplay ();
		}

		// Called by our client code when we get new data.
		public void UpdateCell (Branch branch)
		{
			branchView.Update (branch);
		}
	}
}

