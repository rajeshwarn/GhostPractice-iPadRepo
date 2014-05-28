using System;
using System.Net;
using System.IO;
using System.Text;
using System.Drawing;
using MonoTouch.CoreGraphics;
using System.Text.RegularExpressions;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using MonoTouch.Dialog;
using System.Globalization;
using System.Threading;

namespace GPMobilePad
{
	public partial class PostFee : DialogViewController
	{
		MatterDTO matter;
		MatterDetail matterDetail;
		bool isUnbillable;
		bool isTimeBased = true, isBusy;
		DateTime start, end;
		EntryElement feeAmount;
		EntryElement narration;
		TitleElement tariffName;
		IPostingComplete listener;
		StyledStringElement btnSend, btnCalculate;
		double amount;
		int hours, minutes;
		List<MobileTariffCodeDTO> tariffList, cachedTimeTariffList, cachedTariffList;

		public PostFee (IPostingComplete listener, MatterDetail matterDetail, MatterDTO matter, bool isUnbillable) : base (UITableViewStyle.Grouped, null)
		{
			this.matter = matter;
			this.matterDetail = matterDetail;
			this.isUnbillable = isUnbillable;
			this.listener = listener;
			BuildInterface ();
			GetTariffCodes (100, DataUtil.TARIFF_CODE_TYPE_FEES);
			
		}

		private void Busy ()
		{
			headerLabel.Text = "Busy...Loading.....";
			Console.WriteLine ("##Boolean: comms are busy, slow down!");

		}

		private void CalculateAmount ()
		{	
			amount = 0;
			int duration = (hours * 60) + minutes;
			GetCalculatedFee (duration, selectedTariff.id);
		}

		private void BuildTimeBasedFields ()
		{
			if (isTimeBased) {
				//add 2 btns and result string
				var durSection = new Section ("Task Duration");
				var btnHours = new StyledStringElement ("Pick Hours");
				var btnMin = new StyledStringElement ("Pick Minutes");
				btnHours.Alignment = UITextAlignment.Center;
				btnMin.Alignment = UITextAlignment.Center;
				//btnHours.TextColor = ColorHelper.GetGPLightPurple ();
				//btnMin.TextColor = ColorHelper.GetGPLightPurple ();
				
				var dur = new TitleElement ("" + hours + " hours and " + minutes + " minutes");
				var sb = new StringBuilder (); 
				if (hours > 0) {
					if (hours == 1) {
						sb.Append ("" + hours + " hour ");	
					} else {
						sb.Append ("" + hours + " hours ");
					}
					
					if (minutes > 0) {
						if (minutes == 1) {
							sb.Append (" and " + minutes + " minute");
						} else {
							sb.Append ("" + minutes + " minutes");
						}
					}
				} else {
					if (minutes > 0) {
						if (minutes == 1) {
							sb.Append ("" + minutes + " minute");
						} else {
							sb.Append ("" + minutes + " minutes");
						}
					}
				}
				dur = new TitleElement (sb.ToString ());
				btnHours.Tapped += delegate {
					string[] btns = new string[24];
					for (int i = 0; i < 24; i++) {
						if (i == 0) {
							btns [i] = "" + i;
						} else {
							if (i == 1) {
								btns [i] = "" + i + " hour";
							} else {
								btns [i] = "" + i + " hours";
							}
						}												
					
					}
					var actionSheet = new UIActionSheet (
						                  "Duration in Hours",
						                  null,
						                  "Cancel",
						                  null,
						                  btns
					                  ){ Style = UIActionSheetStyle.Default };
					actionSheet.Clicked += delegate (object sender, UIButtonEventArgs args) {
						if (args.ButtonIndex == 24) {
							//ignore - Cancel
						} else {
							hours = args.ButtonIndex;
							Console.WriteLine ("" + hours + " hours and " + minutes + " minutes");
							BuildInterface ();
							
						}
						
					};

					actionSheet.ShowInView (View);
				};
				btnMin.Tapped += delegate {
					string[] btns = new string[60];
					for (int i = 0; i < 60; i++) {
						if (i == 0) {
							btns [i] = "" + i;
						} else {
							if (i == 1) {
								btns [i] = "" + i + " minute";
							} else {
								btns [i] = "" + i + " minutes";
							}
						}	
					}
					var actionSheet = new UIActionSheet (
						                  "Duration in Minutes",
						                  null,
						                  "Cancel",
						                  null,
						                  btns
					                  ){ Style = UIActionSheetStyle.Default };
					actionSheet.Clicked += delegate (object sender, UIButtonEventArgs args) {
						if (args.ButtonIndex == 60) {
							//ignore - Cancel
						} else {
							minutes = args.ButtonIndex;						
							Console.WriteLine ("" + hours + " hours and " + minutes + " minutes");
							BuildInterface ();
						}
						
					};

					actionSheet.ShowInView (View);
				};
				
				durSection.Add (dur);
				durSection.Add (btnHours);
				durSection.Add (btnMin);
				
				Root.Add (durSection);
			}
		}

		UILabel headerLabel;

		private void BuildHeaderLabel ()
		{
			string s = "Post Fee";
			if (isUnbillable) {
				s = "Post Unbillable Fee";
			}
			
			headerLabel = new UILabel (new RectangleF (10, 10, 480, 40)) {
				Font = UIFont.BoldSystemFontOfSize (20),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = s
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 10, 480, 40); 
			view.Add (headerLabel);
			var topSection = new Section (view);
			
			Root.Add (topSection);	
			String name = "";
			if (matter != null) {
				name = matter.matterName;
			}
			var sec = new Section (name);
			var timeBased = new BooleanElement ("Time Based Task", isTimeBased);
			
			timeBased.ValueChanged += delegate {
				if (isBusy) {
					Busy ();
					return;
				}
				selectedTariff = null;
				feeAmount.Value = null;
				if (timeBased.Value == true) {
					isTimeBased = true;
					if (IsTariffCacheFilled ()) {
						RefreshFromCache ();
						BuildInterface ();
					} else {
						GetTariffCodes (100, DataUtil.TARIFF_CODE_TYPE_FEES);
					}
					
				} else {
					isTimeBased = false;
					if (IsTariffCacheFilled ()) {
						RefreshFromCache ();
						BuildInterface ();
					} else {
						GetTariffCodes (0, DataUtil.TARIFF_CODE_TYPE_FEES);
					}
				}
			};
			sec.Add (timeBased);
			Root.Add (sec);
		}

		private void BuildTariffList ()
		{
			if (tariffList != null && tariffList.Count > 0) {
				var tariffSection = new Section ("");
				var sel = new StyledStringElement ("Select Activity Code");
				sel.TextColor = ColorHelper.GetGPLightPurple ();
				sel.Alignment = UITextAlignment.Center;
				//sel.BackgroundColor = ColorHelper.GetGPLightPurple ();
				sel.Tapped += delegate {
					if (isBusy) {
						Busy ();
						return;
					}
					if (tariffList == null) {
						if (isTimeBased) {
							GetTariffCodes (100, DataUtil.TARIFF_CODE_TYPE_FEES);
							
						} else {
							GetTariffCodes (0, DataUtil.TARIFF_CODE_TYPE_FEES);
							
						}
					
					} else {
						string[] btns = new string[tariffList.Count];
						for (int i = 0; i < tariffList.Count; i++) {
							btns [i] = tariffList [i].name;
						}
						var actionSheet = new UIActionSheet (
							                  "Activity Codes",
							                  null,
							                  "Cancel",
							                  null,
							                  btns
						                  ){ Style = UIActionSheetStyle.Default };
						actionSheet.Clicked += delegate (object sender, UIButtonEventArgs args) {
							Console.WriteLine ("Clicked on tariff item {0}", args.ButtonIndex);
							if (args.ButtonIndex == tariffList.Count) {
								//ignore - Cancel
							} else {
								narration.Value = tariffList [args.ButtonIndex].narration;
								selectedTariff = tariffList [args.ButtonIndex];
								narrationValue = selectedTariff.narration;
								BuildInterface ();
								
							}
							
						};

						actionSheet.ShowInView (View);
					}
				
				};
				tariffSection.Add (sel);
				Root.Add (tariffSection);
			}
		}

		void SetupAndPost ()
		{
			var feeDTO = new FeeDTO ();
			try {
				feeDTO.tariffCodeID = selectedTariff.id;
				feeDTO.matterID = Convert.ToInt16 (matter.id);
				if (isTimeBased) {
					feeDTO.duration = (hours * 60) + minutes;
					if (feeDTO.duration == 0) {
						new UIAlertView (
							"Task Duration Error",
							"Please enter the task duration for the fee before posting",
							null,
							"OK"
						).Show ();
						return;
					}
				}

				Console.WriteLine ("FeeAmount.Value: " + feeAmount.Value);
				string output = Stripper.StripByRegionFormat (feeAmount.Value);
				Console.WriteLine ("`setupAndPost - stripped amt: " + output);

				amount = double.Parse (output.Trim (), 
					CultureInfo.InvariantCulture);
				Console.WriteLine ("`setupAndPost - parsed amount: " + amount);
				feeDTO.amount = amount;
				feeDTO.date = Tools.ConvertDateTimeToJavaMS (DateTime.Now);
				if (amount == 0) {
					new UIAlertView (
						"Fee Amount Error",
						"Please enter or calculate the amount for the fee before posting",
						null,
						"OK"
					).Show ();
					return;
				}
				if (narration.Value.Trim ().Length == 0) {
					new UIAlertView (
						"Narration Error",
						"Please enter a proper narration for the fee before posting",
						null,
						"OK"
					).Show ();
					return;
				}
				feeDTO.narration = narration.Value;
				PostFeeToOffice (feeDTO);
			} catch (Exception e) {
				Console.WriteLine ("### ERROR: " + e.Message);
				new UIAlertView (
					"Amount Error",
					"Please enter a proper amount for the fee before posting",
					null,
					"OK"
				).Show ();
				return;
			}
		}

		private void BuildButtons ()
		{

			var sec3 = new Section ("");			
			btnSend = null;
			
			btnSend = new StyledStringElement ("Send to Office");
			btnSend.Alignment = UITextAlignment.Center;
			btnSend.TextColor = ColorHelper.GetGPLightPurple ();
			btnSend.Tapped += delegate {
				if (isBusy) {
					Busy ();
					return;
				}
//				if (selectedTariff == null) {
//					new UIAlertView (
//						"Activity Code",
//						"Please select an Activity code before posting",
//						null,
//						"OK"
//					).Show ();
//					return;
//				}
				SetupAndPost ();
					
			};
			
			
			btnCalculate = new StyledStringElement ("Calculate Fee");
			btnCalculate.Alignment = UITextAlignment.Center;
			btnCalculate.Tapped += delegate {
				if (isBusy) {
					Busy ();
					return;
				}
				if (selectedTariff == null) {
					new UIAlertView (
						"Activity Code",
						"Please select an Activity code before requesting Calculation",
						null,
						"OK"
					).Show ();
					return;
				}
				if (isTimeBased) {
					if (hours == 0 && minutes == 0) {
						new UIAlertView (
							"Tariff Code",
							"Please enter task duration before requesting Calculation",
							null,
							"OK"
						).Show ();
						return;
					}
				}
				CalculateAmount ();
			};
			
			

			
			sec3.Add (btnCalculate);
			sec3.Add (btnSend);
			Root.Add (sec3);
		}

		string narrationValue;

		private void BuildPostingFields ()
		{
			//add amount and narration
			var narrSection = new Section ("Activity Code");
			if (selectedTariff != null) {	

				narration = new EntryElement (
					"Narration",
					"Narration",
					narrationValue
				);
				tariffName = new TitleElement (selectedTariff.name);
				narrSection.Add (tariffName);
				
			} else {
				narration = new EntryElement ("Narration", "Narration", null);
			}	
		
			narration.EntryEnded += (object sender, EventArgs e) => {
				Console.WriteLine ("## Narration edit ended... to save data value: " + narration.Value);
				narrationValue = narration.Value;
			};
			narrSection.Add (narration);
			Root.Add (narrSection);
			//
			var amtSection = new Section ("");
			if (amount == 0) {
				feeAmount = new EntryElement ("Fee Amount", "0.00", null);
			} else {
				feeAmount = new EntryElement ("Fee Amount", "0.00", amount.ToString ("N"));
			}
			
			feeAmount.KeyboardType = UIKeyboardType.NumbersAndPunctuation;
			amtSection.Add (feeAmount);
			Root.Add (amtSection);
		}

		public void BuildInterface ()
		{
			if (Root == null) {
				Root = new RootElement ("");
			}
			Root.Clear ();			
			BuildHeaderLabel ();	
			addBusySection ();
			//
			BuildTimeBasedFields ();
			BuildTariffList ();
			BuildPostingFields ();
			//
			BuildButtons ();
		}

		void addBusySection ()
		{
			if (isBusy) {
				var lv = new UIActivityIndicatorView (new RectangleF (10f, 10f, 50f, 50f));
				lv.Color = ColorHelper.GetGPLightPurple ();
				lv.StartAnimating ();
				var sv = new Section (lv);
				Root.Add (sv);
			}
		}

		MobileTariffCodeDTO selectedTariff;

		public void PostFeeToOffice (FeeDTO fee)
		{
			if (isBusy) {
				Console.WriteLine ("##PostFee: comms are busy, slow down!");
				return;
			}
			isBusy = true;
			BuildInterface ();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			start = DateTime.Now;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
			if (isTimeBased) {
				
			}
			if (isUnbillable) {
				cr.requestType = GhostRequestDTO.POST_UNBILLABLE_FEE;
			} else {
				cr.requestType = GhostRequestDTO.POST_FEE;
			}
			cr.fee = fee;
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.userID = NSUserDefaults.StandardUserDefaults.IntForKey ("userID");
			cr.companyID = NSUserDefaults.StandardUserDefaults.IntForKey ("companyID");
			cr.deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
				
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("##PostFee JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;
				
			var request = (HttpWebRequest)WebRequest.Create (url);
			request.BeginGetResponse (PostComplete, request);	
			
			
		}

		void PostComplete (IAsyncResult result)
		{
			isBusy = false;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			end = DateTime.Now;
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			try {
				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (result);
				Stream receiveStream = response.GetResponseStream ();
				StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
				string resp = readStream.ReadToEnd ();
				Console.WriteLine ("## PostFee Response stream received.\n" + resp);
				response.Close ();
				readStream.Close ();
				//get JSON response deserialized
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (
					resp,
					typeof(WebServiceResponseDTO)
				);
				if (dto != null) {
					Tools.SendElapsedTime (start, end, dto.activityID);
				}
					
				
				InvokeOnMainThread (delegate {
					BuildInterface ();
					try {
						if (dto.responseCode == 0) {
							matterDetail.GetMatterDetails ();
							new UIAlertView (
								"Posting Result",
								"Posting has been successfully completed",
								null,
								"OK"
							).Show ();
							//TODO - posting complete ....
//							this.NavigationController.PopViewControllerAnimated (true);
						} else {
							new UIAlertView ("Posting Result", dto.responseMessage, null, "OK").Show ();

						}
					} catch (Exception e) {
						//ignore -
						Console.WriteLine ("### IGNORED: " + e.Message);
					}
					notify ();
				}
				);
				
				
				
			} catch (Exception ex) {
				isBusy = false;
				Console.WriteLine ("### ERROR: " + ex.Message);
				InvokeOnMainThread (delegate {
					new UIAlertView (
						"Network Error",
						"Problem communicating with server.\nPlease try later or call GhostPractice Support",
						null,
						"Close"
					).Show ();
				}
				);
			}
		}

		private void notify ()
		{
			listener.onPostingComplete ();
		}

		public void GetTariffCodes (int duration, int tariffCodeType)
		{
			if (isBusy) {
				Console.WriteLine ("##GetTariffCodes: comms are busy, slow down!");
				return;
			}
			if (matter == null) {
				return;
			}
			isBusy = true;
			BuildInterface ();
			Console.WriteLine ("### tariff codes requested, duration: " + duration);
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			start = DateTime.Now;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
				
			cr.requestType = GhostRequestDTO.GET_TARIFF_CODES;
			cr.matterID = matter.id;
			cr.duration = duration;
			cr.tarrifCodeType = tariffCodeType;
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.userID = NSUserDefaults.StandardUserDefaults.IntForKey ("userID");
			cr.companyID = NSUserDefaults.StandardUserDefaults.IntForKey ("companyID");
			cr.deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
				
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("&& GetTariffCodes JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;
				
			var request = (HttpWebRequest)WebRequest.Create (url);
			try {
				request.BeginGetResponse (DataDownloaded, request);
			} catch (Exception e) {
				Console.WriteLine ("Exception - " + e.Message);
				new UIAlertView (
					"Error",
					"Server Unavailable at this time.\nPlease try later.",
					null,
					"OK"
				).Show ();
			}
			
			
		}

		void DataDownloaded (IAsyncResult result)
		{
			isBusy = false;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			end = DateTime.Now;
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			try {
				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (result);
				Stream receiveStream = response.GetResponseStream ();
				StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
				string resp = readStream.ReadToEnd ();
				Console.WriteLine ("## GetTariffCodes Response stream received.\n" + resp);
				response.Close ();
				readStream.Close ();
				//get JSON response deserialized
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (
					resp,
					typeof(WebServiceResponseDTO)
				);
				
				if (dto != null) {
					Tools.SendElapsedTime (start, end, dto.activityID);
				}	
				
				InvokeOnMainThread (delegate {

					selectedTariff = null;
					feeAmount.Value = null;
					BuildInterface ();
					try {
						if (dto.responseCode == 0) {
							tariffList = dto.mobileTariffCodeList;
							Console.WriteLine ("### tariff codes found: " + tariffList.Count); 
							CreateTariffCache ();
							BuildInterface ();
						
							if (tariffList == null || tariffList.Count == 0) {
								new UIAlertView (
									"Search Result",
									"No Activity codes found for matter",
									null,
									"OK"
								).Show ();	
							}
						} else {						
							new UIAlertView ("Search Result", dto.responseMessage, null, "OK").Show ();
							return;
						}
					} catch (Exception e) {
						//ignore - trapping event this is not there to host pop up
						Console.WriteLine ("### IGNORED: " + e.Message);
					}
						
				}
				);
				
				
				
			} catch (Exception ex) {
				isBusy = false;
				Console.WriteLine ("### ERROR: " + ex.Message);
				InvokeOnMainThread (delegate {
					new UIAlertView (
						"Network Error",
						"Problem communicating with server.\nPlease try later or call GhostPractice Support",
						null,
						"Close"
					).Show ();
				}
				);
			}
		}

		private bool IsTariffCacheFilled ()
		{
			if (isTimeBased) {
				if (cachedTimeTariffList == null) {
					return false;
				} else {
					return true;
				}
				
			} else {
				if (cachedTariffList == null) {
					return false;
				} else {
					return true;
				}
			}			
		}

		private void RefreshFromCache ()
		{
			tariffList = new List<MobileTariffCodeDTO> ();
			if (isTimeBased) {				
				foreach (var t in cachedTimeTariffList) {
					tariffList.Add (t);
				}
			} else {
				foreach (var t in cachedTariffList) {
					tariffList.Add (t);
				}
			}
		}

		private void CreateTariffCache ()
		{
			if (isTimeBased) {
				cachedTimeTariffList = new List<MobileTariffCodeDTO> ();
				foreach (var t in tariffList) {
					var mtc = new MobileTariffCodeDTO ();
					mtc.amount = t.amount;
					mtc.id = t.id;
					mtc.name = t.name;
					mtc.narration = t.narration;
					mtc.surchargeApplies = t.surchargeApplies;
					mtc.tariffType = t.tariffType;
					mtc.timeBasedCode = t.timeBasedCode;
					mtc.units = t.units;
					cachedTimeTariffList.Add (mtc);
				}
			} else {
				cachedTariffList = new List<MobileTariffCodeDTO> ();
				foreach (var t in tariffList) {
					var mtc = new MobileTariffCodeDTO ();
					mtc.amount = t.amount;
					mtc.id = t.id;
					mtc.name = t.name;
					mtc.narration = t.narration;
					mtc.surchargeApplies = t.surchargeApplies;
					mtc.tariffType = t.tariffType;
					mtc.timeBasedCode = t.timeBasedCode;
					mtc.units = t.units;
					cachedTariffList.Add (mtc);
				}
			}
		}

		public void GetCalculatedFee (int duration, int tariffCodeID)
		{
			if (isBusy) {
				Console.WriteLine ("##GetCalculatedFee: comms are busy, slow down!");
				return;
			}
			isBusy = true;
			BuildInterface ();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			start = DateTime.Now;
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
				
			cr.requestType = GhostRequestDTO.CALCULATE_FEE;
			cr.matterID = matter.id;
			cr.duration = duration;
			cr.tariffCodeID = tariffCodeID;
			cr.appID = NSUserDefaults.StandardUserDefaults.IntForKey ("appID");
			cr.userID = NSUserDefaults.StandardUserDefaults.IntForKey ("userID");
			cr.companyID = NSUserDefaults.StandardUserDefaults.IntForKey ("companyID");
			cr.deviceID = NSUserDefaults.StandardUserDefaults.StringForKey ("deviceID");
				
			json = JsonConvert.SerializeObject (cr);
			Console.WriteLine ("Async JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;
				
			var request = (HttpWebRequest)WebRequest.Create (url);
			request.BeginGetResponse (FeeDownloaded, request);
			
		}

		void FeeDownloaded (IAsyncResult result)
		{
			isBusy = false;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			end = DateTime.Now;
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			try {
				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (result);
				Stream receiveStream = response.GetResponseStream ();
				StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
				string resp = readStream.ReadToEnd ();
				Console.WriteLine ("## ASYNCResponse stream received.\n" + resp);
				response.Close ();
				readStream.Close ();
				//get JSON response deserialized
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (
					resp,
					typeof(WebServiceResponseDTO)
				);
				
				if (dto != null) {
					Tools.SendElapsedTime (start, end, dto.activityID);
				}	
				
				InvokeOnMainThread (delegate {
					BuildInterface ();
					feeAmount.Value = null;
					try {
						if (dto.responseCode == 0) {
							if (dto.fee == 0) {
								amount = 0;
								feeAmount.Value = null;
								new UIAlertView ("Fee Calculation", "Web Service returned 0.00 fee", null, "OK").Show ();
							} else {
								Console.WriteLine ("Fee calculated: " + dto.fee);
								amount = dto.fee;
								feeAmount.Value = dto.fee.ToString ("#0.00");
								Console.WriteLine ("value from feeAmmount element: " + feeAmount.Value);
							}
							BuildInterface ();
						} else {						
							new UIAlertView ("Fee Calculation", dto.responseMessage, null, "OK").Show ();
							return;
						}
					} catch (Exception e) {
						//ignore - trapping event this is not there to host pop up
						Console.WriteLine ("### IGNORED: " + e.Message);
					}
						
				}
				);
				
				
				
			} catch (Exception ex) {
				Console.WriteLine ("### ERROR: " + ex.Message);
				isBusy = false;
				InvokeOnMainThread (delegate {
					new UIAlertView (
						"Network Error",
						"Problem communicating with server.\nPlease try later or call GhostPractice Support",
						null,
						"Close"
					).Show ();
				}
				);
			}
		}

		public override bool ShouldAutorotate ()
		{
			return false;
		}
	}
}
