using System.Drawing;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GhostPractice
{
    public class RightAlignedEntryElement : EntryElement
    {
        private NSString _cellKey = new NSString("RightAlignedEntryElement");

        public RightAlignedEntryElement (string caption, string amount) : base(caption, null, null)
        {
        }

        protected override UITextField CreateTextField(RectangleF frame)
        {
            var textField = base.CreateTextField (frame);

            textField.TextAlignment = UITextAlignment.Right;

            // shrink textfield a little to have some nice border
            textField.Frame = new RectangleF(new PointF(textField.Frame.Location.X, textField.Frame.Location.Y), 
			                                 new SizeF(textField.Frame.Size.Width - 10, textField.Frame.Size.Height));

            return textField;
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell (tv);
            return cell;
        }

        protected override NSString CellKey {
            get {
                return _cellKey;
            }
        }
    }
}