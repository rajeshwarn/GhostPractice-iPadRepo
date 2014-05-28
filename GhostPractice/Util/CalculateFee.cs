using System;

namespace GhostPractice
{
	public class CalculateFee
	{
		public CalculateFee ()
		{
		}
		
		public static double getFee (MatterDTO matter, MobileTariffCodeDTO code,
		                            int duration)
		{
			try {
				Console.WriteLine ("Calculate Fee - amount: " + code.amount + " duration: " + duration + " units: " + code.units + " surcharge: " + matter.surchargeFactor);
			} catch (Exception e) {
				Console.WriteLine ("Fucked up ... getting exception " + e.Message);
				return 0;
			}
			
			double fee = 0;
			
			if (duration > 0) {
				int elapsed = DivRoundUp (duration, (int) code.units);
				if (code.units > 0) {
					fee = (code.amount / 60) * code.units * elapsed;
				} else {
					fee = (code.amount / 60) *  elapsed;
				}
				
				
			} else {
				if (code.units > 0) {
					fee = code.amount * code.units;	
				} else {
					fee = code.amount;	
				}
				
			}
			
			if (code.surchargeApplies) {
				double surcharge = fee * matter.surchargeFactor;
				fee = surcharge;
			}
			
			return fee;
		}
		
		public static int DivRoundUp (int dividend, int divisor)
		{
			if (divisor == 0)
				return dividend;
			if (divisor == -1 && dividend == Int32.MinValue)
				return 0;
			int roundedTowardsZeroQuotient = dividend / divisor;
			bool dividedEvenly = (dividend % divisor) == 0;
			if (dividedEvenly) 
				return roundedTowardsZeroQuotient;
		
			bool wasRoundedDown = ((divisor > 0) == (dividend > 0));
			if (wasRoundedDown) 
				return roundedTowardsZeroQuotient + 1;
			else
				return roundedTowardsZeroQuotient;
		}
	}
}

/*
 * The user posts a fee with the following settings:
Duration: 20 minutes
Matter Surcharge Factor: 0.10 (10%)
Posting Code:
Time-based: TRUE
Units: 15 (e.g. billed in 15 minute increments)
Rate: R1000
Surcharge applies: TRUE
To calculate the amount for this fee, the application should first work out the duration increments
charged followed by the fee amount:
Duration charged = ROUNDUP(20 / 15) = ROUNDUP(1.33) = 2
Fee amount (before surcharge) = 1000 / 60 * 15 * Duration charged = R500
Fee amount (after surcharge) = R500 * (1+0.10) = R550
 */ 