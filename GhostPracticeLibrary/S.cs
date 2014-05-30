using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GhostPracticeLibrary
{
	/*
	 * 
	 * Control text string dictionaries to support country differences
	 */
	public class S
	{
		public S ()
		{
			Initialize ();
		}

		private static Dictionary<string, string> en_ZA;
		private static Dictionary<string, string> en_CA;
		private static Dictionary<string, string> en_GB;

		/**
		 * Return appropriate country string based on key string
		 * 
		 **/ 
		public static String GetText (string key)
		{
			if (en_ZA == null) {
				Initialize ();
			}
			var xx = Thread.CurrentThread.CurrentCulture;

			String value = "ERROR, needs setup";
			if (xx.ToString () == "en-ZA") {
				var OK = en_ZA.TryGetValue (key, out value);
				if (OK) {
					return value;
				}
			}
			if (xx.ToString () == "en-CA") {
				var OK = en_CA.TryGetValue (key, out value);
				if (OK) {
					return value;
				}
			}
			if (xx.ToString () == "en-GB") {
				var OK = en_GB.TryGetValue (key, out value);
				if (OK) {
					return value;
				}
			}
			return value;
		}

		public const string MATTER_SEARCH = "matter_srch",
			SEARCH = "search", BUSINESS_BALANCE = "bus_balance", CURRENT_BALANCE = "curr_balance",
			TRUST_BALANCE = "trstBal", 
			INVESTMENT_BALANCE = "invBal",
			RESERVE_TRUST = "rsvTrust", TIME_BASED_ACTIVITY = "tba",
			DURATION = "dur", BUSINESS_DEBTORS = "bdebt", 

			BUSINESS_CREDITORS = "bcred",
			VAT = "vat", TRUST_STATUS = "trStat", TRUST = "trsut", TRUST_CREDITORS = "trustCred",
			INVESTMENTS = "invs", SELECT_ACTIVITY_CODE = "selact", ACTIVITY_CODES = "actcodes";

		/**
		 * Initialize and load text Dictionaries
		 **/ 
		private static void Initialize ()
		{
			Console.WriteLine ("###### initialize Dictionaries");
			en_ZA = new Dictionary<string, string> ();
			en_ZA.Add (MATTER_SEARCH, "Matter Search");
			en_ZA.Add (SEARCH, "Search");
			en_ZA.Add (BUSINESS_BALANCE, "Business Balance");
			en_ZA.Add (TRUST_BALANCE, "Trust Balance");
			en_ZA.Add (INVESTMENT_BALANCE, "Investment Balance");
			en_ZA.Add (RESERVE_TRUST, "Reserve Trust");
			en_ZA.Add (TIME_BASED_ACTIVITY, "Time Based Activity");
			en_ZA.Add (DURATION, "Duration");
			en_ZA.Add (BUSINESS_DEBTORS, "Business Debtors");
			en_ZA.Add (BUSINESS_CREDITORS, "Business Creditors");

			en_ZA.Add (VAT, "VAT");
			en_ZA.Add (TRUST_STATUS, "Trust Status");
			en_ZA.Add (TRUST, "Trust");
			en_ZA.Add (TRUST_CREDITORS, "Trust Creditors");
			en_ZA.Add (INVESTMENTS, "Investments");
			en_ZA.Add (SELECT_ACTIVITY_CODE, "Select Activity Code");
			en_ZA.Add (ACTIVITY_CODES, "Activity Codes");
			en_ZA.Add (CURRENT_BALANCE, "Current Balance");

			//Canada
			en_CA = new Dictionary<string, string> ();
			en_CA.Add (MATTER_SEARCH, "Matter Search");
			en_CA.Add (SEARCH, "Search");
			en_CA.Add (BUSINESS_BALANCE, "A/R Balance");
			en_CA.Add (TRUST_BALANCE, "Trust Balance");
			en_CA.Add (INVESTMENT_BALANCE, "Investment Balance");
			en_CA.Add (RESERVE_TRUST, " ");
			en_CA.Add (TIME_BASED_ACTIVITY, "Time Based Activity");
			en_CA.Add (DURATION, "Duration");
			en_CA.Add (BUSINESS_DEBTORS, "Accounts Receivable");
			en_CA.Add (BUSINESS_CREDITORS, "Accounts Payable");
			en_CA.Add (SELECT_ACTIVITY_CODE, "Select Activity Code");
			en_CA.Add (ACTIVITY_CODES, "Activity Codes");

			en_CA.Add (VAT, "Tax");
			en_CA.Add (TRUST_STATUS, "Trust Status");
			en_CA.Add (TRUST, "Trust");
			en_CA.Add (TRUST_CREDITORS, "Trust Creditors");
			en_CA.Add (INVESTMENTS, "Investments");
			en_CA.Add (CURRENT_BALANCE, "Current Balance");

			//UK
			en_GB = new Dictionary<string, string> ();
			en_GB.Add (MATTER_SEARCH, "Matter Search");
			en_GB.Add (SEARCH, "Search");
			en_GB.Add (BUSINESS_BALANCE, "Office Account");
			en_GB.Add (TRUST_BALANCE, "Client Account");
			en_GB.Add (INVESTMENT_BALANCE, "Deposit Account");
			en_GB.Add (RESERVE_TRUST, "Reserve Client Limit");
			en_GB.Add (TIME_BASED_ACTIVITY, "Time Based Activity");
			en_GB.Add (DURATION, "Duration");
			en_GB.Add (BUSINESS_DEBTORS, "Office Debtors");
			en_GB.Add (BUSINESS_CREDITORS, "Office Creditors");

			en_GB.Add (VAT, "VAT");
			en_GB.Add (TRUST_STATUS, "Client Status");
			en_GB.Add (TRUST, "Client");
			en_GB.Add (TRUST_CREDITORS, "Client Creditors");
			en_GB.Add (INVESTMENTS, "Deposits");
			en_GB.Add (SELECT_ACTIVITY_CODE, "Select Activity Code");
			en_GB.Add (ACTIVITY_CODES, "Activity Codes");
			en_GB.Add (CURRENT_BALANCE, "Current Balance");
		}
	}
}

