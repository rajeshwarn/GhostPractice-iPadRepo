using System;
using System.Net;
using System.IO;
using System.Text;

using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Web;

namespace GhostAdmin
{
	public class TalkToServer
	{
		public TalkToServer ()
		{
		}
		
//		public static string postWeb (string url, string parms)
//		{
//			string res = null;
//			try {
//				WebClient client = new WebClient ();
//				client.Headers ["Content-type"] = "application/json; charset+utf-8";
//				//byte[]  buffer = Encoding.ASCII.GetBytes( parms );
//				 Uri uri = new Uri(url);
//				 res = client.UploadString (uri, parms);
//			} catch (WebException webExcp) {
//				// If you reach this point, an exception has been caught.
//				Console.WriteLine ("A WebException has been caught.");
//				// Write out the WebException message.
//				Console.WriteLine (webExcp.ToString ());
//				// Get the WebException status code.
//				WebExceptionStatus status = webExcp.Status;
//				// If status is WebExceptionStatus.ProtocolError, 
//				//   there has been a protocol error and a WebResponse 
//				//   should exist. Display the protocol error.
//				if (status == WebExceptionStatus.ProtocolError) {
//					Console.Write ("The server returned protocol error ");
//					// Get HttpWebResponse so that you can check the HTTP status code.
//					HttpWebResponse httpResponse = (HttpWebResponse)webExcp.Response;
//					Console.WriteLine ((int)httpResponse.StatusCode + " - " + httpResponse.StatusCode);
//				}
//			}
//			return res;
//			
//		}
//
//		public static string postJSONData (string url, string parms)
//		{
//			//Our postvars
//			byte[] buffer = Encoding.ASCII.GetBytes (parms);
//			//Initialization, we use localhost, change if applicable
//			HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create (url);
//		    
//			WebReq.Method = "POST";
//			WebReq.ContentType = "application/json; charset=utf-8";
//			WebReq.ContentLength = buffer.Length;
//			
//			//POST the data.
//			StreamWriter requestWriter;
//			using (requestWriter = new StreamWriter(WebReq.GetRequestStream())) {
//				requestWriter.Write (parms);
//			}
//			
//			
////		    Stream PostData = WebReq.GetRequestStream();
////		    PostData.Write(buffer, 0, buffer.Length);
////		    PostData.Close();
//			
//			WebReq.GetRequestStream ().Close ();
//			//Get the response handle, we have no true response yet!
//			HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse ();
//			Console.WriteLine ("Status Code: " + WebResp.StatusCode);
//			Console.WriteLine ("Server: " + WebResp.Server);
//		           
//			//Now, we read the response (the string), and output it.
//			Stream Answer = WebResp.GetResponseStream ();
//			StreamReader _Answer = new StreamReader (Answer);
//			string resp = _Answer.ReadToEnd ();
//			Console.WriteLine (resp);
//		
//			return resp;
//		}

		public static string getData (string url)
		{
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);

			// Set some reasonable limits on resources used by this request
			request.MaximumAutomaticRedirections = 4;
			request.MaximumResponseHeadersLength = 4;
			// Set credentials to use for this request.
			request.Credentials = CredentialCache.DefaultCredentials;
			HttpWebResponse response = (HttpWebResponse)request.GetResponse ();

			Console.WriteLine ("Content length is {0}", response.ContentLength);
			Console.WriteLine ("Content type is {0}", response.ContentType);

			// Get the stream associated with the response.
			Stream receiveStream = response.GetResponseStream ();

			// Pipes the stream to a higher level stream reader with the required encoding format. 
			StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
			string resp = readStream.ReadToEnd ();
			Console.WriteLine ("Response stream received.");
			Console.WriteLine (resp);
			response.Close ();
			readStream.Close ();	
			
			return resp;
		}
		//
		// Asynchronous HTTP request
		//
		public static string  getAsyncData (string url)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			var request = WebRequest.Create (url);
			IAsyncResult result = request.BeginGetResponse (null, request);
			WebResponse x = request.EndGetResponse (result);
			// Get the stream associated with the response.
			Stream receiveStream = x.GetResponseStream ();

			// Pipes the stream to a higher level stream reader with the required encoding format. 
			StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
			string resp = readStream.ReadToEnd ();
			Console.WriteLine ("AsyncXXX Response stream : \n");
			Console.WriteLine (resp);
			
			return resp;
		}
		
		//
		// Invoked when we get the stream back from the twitter feed
		// We parse the RSS feed and push the data into a 
		// table.
		//
		static void DataDownloaded (IAsyncResult result)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			var request = result.AsyncState as HttpWebRequest;
			
			
			try {
				var response = request.EndGetResponse (result);
				// Get the stream associated with the response.
				Stream receiveStream = response.GetResponseStream ();

				// Pipes the stream to a higher level stream reader with the required encoding format. 
				StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
				string resp = readStream.ReadToEnd ();
				Console.WriteLine ("Async Response stream received.");
				Console.WriteLine (resp);
				
			} catch (Exception ex) {
				Console.WriteLine ("shit " + ex.ToString ());				
			}
		}
	}
}

