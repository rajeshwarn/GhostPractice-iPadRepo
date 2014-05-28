using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace GPMobilePad
{
	public class CustomImageStringElement : ImageStringElement
	{
		UIImage currentImage;
		UITableViewCell currentCell;

		public CustomImageStringElement (string caption, UIImage image) : base (caption, image)
		{
			currentImage = image;
		}

		public override UITableViewCell GetCell (UITableView tableView)
		{
			var cell = base.GetCell (tableView);
			cell.ImageView.Image = currentImage;
			currentCell = cell;
			return currentCell;
		}

		public void SetImage (UIImage image)
		{
			currentImage = image;
			if (currentCell != null)
				currentCell.ImageView.Image = currentImage;
		}

		public void SetBackgroundColor (UIColor color)
		{
			if (currentCell != null)
				currentCell.ContentView.BackgroundColor = color;
		}
	}
}

