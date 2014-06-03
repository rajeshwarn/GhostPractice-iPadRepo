using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using GhostPracticeLibrary;

namespace GhostPractice
{
	public partial class FinancialStatusByBranchController : DialogViewController
	{
		Branch branch;
		float contentWidth;

		public FinancialStatusByBranchController (Branch branch) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = false;
			this.branch = branch;
			contentWidth = View.Frame.Width + 20;
			Console.WriteLine ("FinancialStatusByBranchController .... View.Frame.Width aka contentWidth = " + contentWidth);
			buildReport ();
		}

		public void buildReport ()
		{
			Root = new RootElement ("Branch Financial Status");	
			Stripper.SetReportHeader (Root, "Branch Financial Status", branch.name, contentWidth);
			
			var business = new Section ("");
			business.Add (getElement (branch.businessStatus.businessDebtors, S.GetText (S.BUSINESS_DEBTORS) + ":  "));
			business.Add (getElement (branch.businessStatus.businessCreditors, S.GetText (S.BUSINESS_CREDITORS) + ":  "));
			business.Add (getElement (branch.businessStatus.banksTotal, "Banks Total:  "));
			business.Add (getElement (branch.businessStatus.pendingDisbursements, S.GetText (S.PENDING_DISBURSEMENTS) + ":  "));
			business.Add (getElement (branch.businessStatus.unbilled, "Unbilled:  "));
			business.Add (getElement (branch.businessStatus.vat, S.GetText (S.VAT) + ":  "));
			business.Add (getElement (branch.businessStatus.availableForTransfer, "Available for Transfer:  "));
			
			for (var i = 0; i < branch.businessStatus.banks.Count; i++) {
				var bn = new TitleElement (branch.businessStatus.banks [i].name);	
				business.Add (bn);
				Bank bank = branch.businessStatus.banks [i];
				business.Add (getElement (bank.balance, "Balance:  "));
				business.Add (getElement (bank.receiptsForPeriod, "Receipts:  "));
				business.Add (new StringElement ("Date Reconciled:  " + bank.dateReconciled));
				business.Add (getElement (bank.reconciledAmount, "Reconciled Amount:  "));
				
			}
			Root.Add (business);
			//
			var trust = new Section (S.GetText (S.TRUST_STATUS));
			trust.Add (getElement (branch.trustStatus.banksTotal, "Banks Total:  "));
			trust.Add (getElement (branch.trustStatus.trustCreditors, S.GetText (S.TRUST_CREDITORS) + ":  "));
			trust.Add (getElement (branch.trustStatus.investments, S.GetText (S.INVESTMENTS) + ":  "));
			
			for (var i = 0; i < branch.trustStatus.banks.Count; i++) {
				var bn = new TitleElement (branch.trustStatus.banks [i].name);	
				trust.Add (bn);
				Bank bank = branch.trustStatus.banks [i];
				trust.Add (getElement (bank.balance, "Balance:  "));
				trust.Add (getElement (bank.receiptsForPeriod, "Receipts:  "));
				trust.Add (new StringElement ("Date Reconciled:  " + bank.dateReconciled));
				trust.Add (getElement (bank.reconciledAmount, "Reconciled Amount:  "));

			}
			Root.Add (trust);
		}

		public static FinanceElement getElement (double amount, string caption)
		{
			var e = new FinanceElement (caption, amount);
			return e;
		}

		public EntryElement getNumberElement (double amount, string caption)
		{
			EntryElement e = new EntryElement (caption, "", amount.ToString ("N", Stripper.GetNumberFormatInfo ()));
			e.ResignFirstResponder (true);
			
			return e;

		}
	}
}
