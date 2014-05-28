using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;


namespace GPMobilePad
{
	public class DeviceTypeUtil
	{

		public static int getDeviceType() {
			var model = UIDevice.CurrentDevice.Model.ToString ();
			var name = UIDevice.CurrentDevice.Name.ToString ();
			Console.WriteLine ("FontSizeUtil device", "Name: " + name + " model: " + model + "\n" +  UIDevice.CurrentDevice.LocalizedModel);

			if (model.Equals("iPad Air")) {
				return FinanceElement.IPAD_AIR;
			}

			if (model.Equals("iPad Mini")) {
				return FinanceElement.IPAD_MINI;
			}

			return FinanceElement.IPAD_2;
		}
	}
}

