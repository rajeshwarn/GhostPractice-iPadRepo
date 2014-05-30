using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

namespace GhostPractice
{
	public class Tools
	{
		public Tools ()
		{
		}		
		//public const string CONSOLE_URL = "http://10.0.0.239:8080/GhostPractice-war/ghost?json=";
		//public const string CONSOLE_URL = "http://centos5.boha.za.com:8080/GhostPractice-war/ghost?json=";
		public const string CONSOLE_URL = "http://gpmobile.ghostpractice.com:7180/GhostPractice-war/ghost?json=";
		//
		
		public static DateTime ConvertJavaMiliSecondToDateTime (long javaMS)
		{

			DateTime UTCBaseTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
			DateTime dt = UTCBaseTime.Add (new TimeSpan (javaMS * TimeSpan.TicksPerMillisecond)).ToLocalTime ();
			return dt;

		}

		public static long ConvertDateTimeToJavaMS (DateTime date)
		{
			//Console.WriteLine ("---> Converting date: " + date.ToString ());
			DateTime UTCBaseTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Local);			
			TimeSpan ts = date - UTCBaseTime; 
			long ms = (long)ts.TotalMilliseconds; 
			//account for time zone - 2 hours from GMT
			long twoHours = 1000 * 60 * 60 * 2;

			//Console.WriteLine ("---> ## Re-Conversion: " + ConvertJavaMiliSecondToDateTime (ms).ToString () + " ms: " + ms);
			return ms - twoHours;

		}

		public static DateTime GetDate (int days)
		{
			DateTime dt = DateTime.Now.AddDays (- days); 	
			return dt;
		}
		
		private static double getElapsed (DateTime start, DateTime end)
		{		
			double d = ((double)ConvertDateTimeToJavaMS (end) - (double)ConvertDateTimeToJavaMS (start)) / 1000;
			Console.WriteLine ("### Elapsed Time: " + d.ToString ());
			if (d == 0) {
				d = 1.0;
			}
			return d;
		}
		
		// Asynchronous HTTP request
		//
		public static void SendElapsedTime (DateTime start, DateTime end, int activityID)
		{
			GhostRequestDTO cr = new GhostRequestDTO ();
			string json, encodedJSON, url;
				
			cr.requestType = GhostRequestDTO.POST_DEVICE_ELAPSED_TIME;
			cr.activityID = activityID;
			cr.deviceElapsedSeconds = getElapsed (start, end);
					
			json = JsonConvert.SerializeObject (cr);
			//Console.WriteLine ("SendElapsed JSON = " + json);
			encodedJSON = HttpUtility.UrlEncode (json);
			url = Tools.CONSOLE_URL + encodedJSON;
				
			var request = (HttpWebRequest)WebRequest.Create (url);
			request.BeginGetResponse (PlatformDownloaded, request);
			
		}
		
		static void  PlatformDownloaded (IAsyncResult result)
		{
			var request = result.AsyncState as HttpWebRequest;			
			WebServiceResponseDTO dto;
			
			try {
				HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (result);
				Stream receiveStream = response.GetResponseStream ();
				StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
				string resp = readStream.ReadToEnd ();
				//Console.WriteLine ("## SendElapsed Response stream received.\n" + resp);
				response.Close ();
				readStream.Close ();
				dto = (WebServiceResponseDTO)JsonConvert.DeserializeObject (resp, typeof(WebServiceResponseDTO));	
				if (dto.responseCode > 0) {
					Console.WriteLine ("### ERROR sending elapsed time: ");
				}
				
			} catch (Exception ex) {
				Console.WriteLine ("shit " + ex.ToString ());
			}
		}
	}
}

