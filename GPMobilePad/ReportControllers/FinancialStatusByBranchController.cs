using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using GhostPractice;
using GhostPracticeLibrary;

namespace GPMobilePad
{
	public partial class FinancialStatusByBranchController : DialogViewController
	{
		Branch branch;
		NumberFormatInfo nfi = new CultureInfo ("en-ZA", false).NumberFormat;

		public FinancialStatusByBranchController (Branch branch) : base (UITableViewStyle.Grouped, null)
		{
			Autorotate = false;
			this.branch = branch;
			buildReport ();
		}

		public void buildReport ()
		{
			
			
			Root = new RootElement ("");
			var v = new UIView ();
			v.Frame = new RectangleF (10, 10, 600, 10); 
			var dummy = new Section (v);
			Root.Add (dummy);

			var headerLabel = new UILabel (new RectangleF (10, 10, 800, 48)) {
				Font = UIFont.BoldSystemFontOfSize (18),
				BackgroundColor = ColorHelper.GetGPPurple (),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Text = "Branch Financial Status"
			};
			var view = new UIViewBordered ();
			view.Frame = new RectangleF (10, 20, 800, 48); 
			view.Add (headerLabel);
			var topSection = new Section (view);


			Root.Add (topSection);	

			//branch name view
			var branchLabel = new UILabel (new RectangleF (10, 10, 800, 24)) {
				Font = UIFont.BoldSystemFontOfSize (24),
				TextAlignment = UITextAlignment.Center,
				TextColor = ColorHelper.GetGPPurple (),
				Text = branch.name
			};
			var bs = new Section (branch.name);
			Root.Add (bs);
			
			var business = new Section ();
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
			for (var i = 0; i < 10; i++) {
				Root.Add (new Section (" "));
			}



		}

		public static FinanceElement getElement (double amount, string caption)
		{
			var e = new FinanceElement (caption, amount);
			return e;
		}

		public EntryElement getNumberElement (double amount, string caption)
		{
			EntryElement e = new EntryElement (caption, "", amount.ToString ("N", nfi));
			e.ResignFirstResponder (true);
			
			return e;

		}
	}
}
