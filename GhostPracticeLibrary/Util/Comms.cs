using System;
using MonoTouch.Foundation;
using System.Collections.Generic;
using MonoTouch.UIKit;

namespace GhostPractice
{
	public class Comms:NSUrlConnection
	{

		private static Dictionary<string, Comms> Connections = new Dictionary<string, Comms> ();

		public static void KillAllConnections ()
		{

			foreach (Comms c in Connections.Values) {
				c.Cancel ();
			}
			Connections.Clear ();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		protected static void KillConnection (string name)
		{
			Connections [name].Cancel ();
			Connections.Remove (name);
		}

		public static void ConnectionEnded (string name)
		{
			Connections.Remove (name);
		}

		public static bool IsDownloading (string name)
		{
			return Connections.ContainsKey (name);
		}
		//Constructor
		public Comms (string name, NSUrlRequest request, Action<string> c):base(request, new CommsDelegate(name, c), true)
		{
			if (Connections.ContainsKey (name)) {
				KillConnection (name);
			}
			Connections.Add (name, this);
		}

		public Comms (string name, NSUrlRequest request, Action<string> success, Action failure):base(request, new CommsDelegate(name, success, failure), true)
		{
			if (Connections.ContainsKey (name)) {
				KillConnection (name);
			}
			Connections.Add (name, this);
		}
	}

	public class CommsDelegate : NSUrlConnectionDelegate
	{
		Action<string> callback;
		Action _failure;
		NSMutableData data;
		string _name;

		public CommsDelegate (string name, Action<string> success)
		{
			_name = name;
			callback = success;
			data = new NSMutableData ();
		}

		public CommsDelegate (string name, Action<string> success, Action failure)
		{
			_name = name;
			callback = success;
			_failure = failure;
			data = new NSMutableData ();
		}

		public override void ReceivedData (NSUrlConnection connection, NSData d)
		{
			data.AppendData (d);
		}

		public override bool CanAuthenticateAgainstProtectionSpace (NSUrlConnection connection, NSUrlProtectionSpace protectionSpace)
		{
			return true;
		}

		bool showError = true;

		public override void ReceivedAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge)
		{
//			if (challenge.PreviousFailureCount > 0) {
//				showError = false;
//				challenge.Sender.CancelAuthenticationChallenge (challenge);
//				Application.AuthenticationFailure ();
//				return;
//			}
//
//			if (challenge.ProtectionSpace.AuthenticationMethod == "NSURLAuthenticationMethodServerTrust")
//				challenge.Sender.UseCredentials (NSUrlCredential.FromTrust (challenge.ProtectionSpace.ServerTrust), challenge);
//
//			if (challenge.ProtectionSpace.AuthenticationMethod == "NSURLAuthenticationMethodDefault" &&
//				Application.Account != null && Application.Account.Login != null && Application.Account.Password != null) {
//								challenge.Sender.UseCredentials (NSUrlCredential.FromUserPasswordPersistance (
//				Application.Account.Login, Application.Account.Password, NSUrlCredentialPersistence.None), challenge);

//			}
		}

		public override void FailedWithError (NSUrlConnection connection, NSError error)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			if (showError)
				//Application.ShowNetworkError (error.LocalizedDescription);
				new UIAlertView("Network Error", "Communications problem", null, "Close").Show();

			if (_failure != null)
				_failure ();
		}

		public override void FinishedLoading (NSUrlConnection connection)
		{
			Comms.ConnectionEnded (_name);
			callback (data.ToString ());
		}
	}
}