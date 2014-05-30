using System;
using MonoTouch.UIKit;
namespace GhostPractice
{
	public class ColorHelper
    {
        
		public static UIColor FromHex(int hexValue)
        {
            return UIColor.FromRGB(
                (((float)((hexValue & 0xFF0000) >> 16))/255.0f),
                (((float)((hexValue & 0xFF00) >> 8))/255.0f),
                (((float)(hexValue & 0xFF))/255.0f)
            );
        }
		public static UIColor GetGPPurple ()
		{
			//return FromHex (0x4e2da3);
			//return FromHex (0x4720aa);
			return GetGPLightPurple ();
		}
		public static UIColor GetGPLightPurple ()
		{
			
			return FromHex (0x743db4);
		}
		public static UIColor GetGPSlateGray ()
		{
			
			return FromHex (0xc2dfff);
		}
		public static UIColor GetGPSilver ()
		{
			
			return FromHex (0xc0c0c0);
		}
		public static UIColor GetGPSkyBlue ()
		{
			
			return FromHex (0xa0cfec);
		}
		public static UIColor GetGPSomeColor ()
		{
			
			return FromHex (0xfff8c6);
		}
    }
}

/*
 * new UIColor().FromHex(0x4F6176);
 */ 
