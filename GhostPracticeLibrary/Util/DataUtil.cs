using System;
using System.Collections.Generic;
//using MonoTouch.CoreLocation;
using System.Threading;
using MonoTouch.Foundation;
using Newtonsoft.Json;
using System.Web;
using MonoTouch.UIKit;

namespace GhostPractice
{
	public class DataUtil
	{
		//static CLLocationCoordinate2D coordinate;
		public DataUtil ()
		{
		}
		
		
		public static int getDuration(int index) {
			int duration = 10 * (index + 1);	
			return duration;
		}
		public static PickerDataModel GetTimePickerModel() {
			PickerDataModel model = new PickerDataModel ();
			model.Items.Add ("10 minutes");
			model.Items.Add ("20 minutes");
			model.Items.Add ("30 minutes");
			model.Items.Add ("40 minutes");
			model.Items.Add ("50 minutes");
			model.Items.Add ("1 hour");
			model.Items.Add ("1 hour 10 minutes");
			model.Items.Add ("1 hour 20 minutes");
			model.Items.Add ("1 hour 30 minutes");
			model.Items.Add ("1 hour 40 minutes");
			model.Items.Add ("1 hour 50 minutes");
			model.Items.Add ("2 hours");
			model.Items.Add ("2 hours 10 minutes");
			model.Items.Add ("2 hours 20 minutes");
			model.Items.Add ("2 hours 30 minutes");
			model.Items.Add ("2 hours 40 minutes");
			model.Items.Add ("2 hours 50 minutes");
			model.Items.Add ("3 hours");
			model.Items.Add ("3 hours 10 minutes");
			model.Items.Add ("3 hours 20 minutes");
			model.Items.Add ("3 hours 30 minutes");
			model.Items.Add ("3 hours 40 minutes");
			model.Items.Add ("3 hours 50 minutes");
			model.Items.Add ("4 hours");
			model.Items.Add ("4 hours 10 minutes");
			model.Items.Add ("4 hours 20 minutes");
			model.Items.Add ("4 hours 30 minutes");
			model.Items.Add ("4 hours 40 minutes");
			model.Items.Add ("4 hours 50 minutes");
			model.Items.Add ("5 hours");
			model.Items.Add ("5 hours 10 minutes");
			model.Items.Add ("5 hours 20 minutes");
			model.Items.Add ("5 hours 30 minutes");
			model.Items.Add ("5 hours 40 minutes");
			model.Items.Add ("5 hours 50 minutes");
			model.Items.Add ("6 hours");
			model.Items.Add ("6 hours 10 minutes");
			model.Items.Add ("6 hours 20 minutes");
			model.Items.Add ("6 hours 30 minutes");
			model.Items.Add ("6 hours 40 minutes");
			model.Items.Add ("6 hours 50 minutes");
			model.Items.Add ("7 hours");
			model.Items.Add ("7 hours 10 minutes");
			model.Items.Add ("7 hours 20 minutes");
			model.Items.Add ("7 hours 30 minutes");
			model.Items.Add ("7 hours 40 minutes");
			model.Items.Add ("7 hours 50 minutes");
			model.Items.Add ("8 hours");
			
			return model;
			
		}
		
		public static bool ping() 
		{
			return true;
		}
		public const int TARIFF_CODE_TYPE_FEES = 0;
		public const int TARIFF_CODE_TYPE_NOTES = 1;
		
		public const int REPORT_FINANCIAL_STATUS = 1;
		public const int REPORT_FEE_TARGET = 2;
		public const int REPORT_MATTER_ANALYSIS = 3;
	}
}

/*
 * 
 * GetReport
Generate the report data for a specific report from the database.
4.8.1 Input Parameters
Param name Type Description
reportType ReportType The ReportType that must be generated
4.8.2 Returns
XML containing the report data (String)


4.9 GetMatterDetails
Get more information about a matter
4.9.1 Input Parameters
Param name Type Description
matterID Int ID indicating which matter’s details to load.
4.9.2 Returns
MobileMatter
 * 
 * 
 * 
 * PostNote
Post a note to the database, given the fileID.
4.7.1 Input Parameters
Param name Type Description
matterID Int The matter identifier to post the note for
narration String The narration for the posted note
date DateTime The date that should be associated with the posted
note.
tariffCodeID Int The fee code that should be associated with this
note.
4.7.2 Returns
Exception if posting fails.
Copyright
 * 
 * PostUnbillableFee
Post an unbillable fee to the database, given the code, amount and fileID.
4.6.1 Input Parameters
Param name Type Description
matterID Int The matter identifier to post the fee for
duration Int (nullable) Optional parameter specifying the duration that the
fee should be logged for
amount Decimal The total amount for the fee that should be posted.
narration String The narration for the posted fee
tariffCodeID Int The fee code that should be associated with this
fee.
date DateTime The date that should be associated with the posted
fee.
4.6.2 Returns
Exception if posting fails.
 * 
 * 
 * 
 * 
 * 4.4 PostFee
Post an unbilled (WIP) fee to the database, given the code, amount and fileID.
4.4.1 Input Parameters
Param name Type Description
matterID Int The matter identifier to post the fee for
duration Int (nullable) Optional parameter specifying the duration that the
fee should be logged for
amount Decimal The total amount for the fee that should be posted.
narration String The narration for the posted fee
tariffCodeID Int The fee code that should be associated with this
fee.
date DateTime The date that should be associated with the posted
fee.
4.4.2 Returns
Exception if posting fails.
 * 
 * 
 * 
 * 
 * GetCodesForMatter
Load all applicable fee codes for a specific matter along with the relevant rate.
The codes may be filtered by the system so as to provide a smaller subset of relevant codes. If the
user specifies a time value, the system will e.g. only return time-based codes as the others would not
be relevant.
4.3.1 Input Parameters
Param name Type Description
matterID Int The matter identifier to load codes for
codeType Int 0 = fees, 1 = notes
duration Int (nullable) Optional parameter – if specified only time-based fee
codes will be loaded.
4.3.2 Returns
List<MobileTariffCode>
Copyright
 * 
 */ 