using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GPMobilePad
{
	public class PickerDataModel: UIPickerViewModel
	{
		public PickerDataModel ()
		{
		}
		
		public event EventHandler<EventArgs> ValueChanged;
        
		public List<string> Items {
			get { return this._items; }
			set { this._items = value; }
		}

		List<string> _items = new List<string> ();
		
		public string SelectedItem {
			get { return this._items [this._selectedIndex]; }
		}

		protected int _selectedIndex = 0;
		
		public int SelectedIndex {
			
			get { return this._selectedIndex; }
		}

		public override int GetRowsInComponent (UIPickerView picker, int component)
		{
			return this._items.Count;
		}

		public override string GetTitle (UIPickerView picker, int row, int component)
		{
			return this._items [row];
		}

		public override int GetComponentCount (UIPickerView picker)
		{
			return 1;
		}
		
		public override void Selected (UIPickerView picker, int row, int component)
		{
			Console.WriteLine ("Picker selected row: " + row);
			this._selectedIndex = row;
			if (this.ValueChanged != null) {
				Console.WriteLine ("Picker value changed: " + this.ValueChanged);
				this.ValueChanged (this, new EventArgs ());
			}
		}

	}
}

