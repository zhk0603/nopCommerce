using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Orders
{
    [TestFixture]
    public class RecurringPaymentTests
    {
        [Test]
        public void Can_calculate_nextPaymentDate_with_days_as_cycle_period()
        {
            var rp = TestHelper.GetRecurringPayment();
            rp.CycleLength = 7;
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 1));

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 8));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 15));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
        }

        [Test]
        public void Can_calculate_nextPaymentDate_with_weeks_as_cycle_period()
        {
            var rp = TestHelper.GetRecurringPayment();
            rp.CyclePeriod = RecurringProductCyclePeriod.Weeks;
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 1));

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 15));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 29));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
        }

        [Test]
        public void Can_calculate_nextPaymentDate_with_months_as_cycle_period()
        {
            var rp = TestHelper.GetRecurringPayment();
            rp.CyclePeriod = RecurringProductCyclePeriod.Months;

            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 1));

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 5, 1));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 7, 1));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
        }

        [Test]
        public void Can_calculate_nextPaymentDate_with_years_as_cycle_period()
        {
            var rp = TestHelper.GetRecurringPayment();
            rp.CyclePeriod = RecurringProductCyclePeriod.Years;
            rp.NextPaymentDate.ShouldEqual(new DateTime(2010, 3, 1));

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2012, 3, 1));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldEqual(new DateTime(2014, 3, 1));
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
        }

        [Test]
        public void Next_payment_date_is_null_when_recurring_payment_is_not_active()
        {
            var rp = TestHelper.GetRecurringPayment();
            rp.IsActive = false;
            rp.NextPaymentDate.ShouldBeNull();
            rp.CycleLength = 7;

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.NextPaymentDate.ShouldBeNull();
        }

        [Test]
        public void Can_calculate_number_of_remaining_cycle()
        {
            var rp = TestHelper.GetRecurringPayment();

            rp.CyclesRemaining.ShouldEqual(3);

            //add one history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.CyclesRemaining.ShouldEqual(2);
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.CyclesRemaining.ShouldEqual(1);
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.CyclesRemaining.ShouldEqual(0);
            //add one more history record
            rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory());
            rp.CyclesRemaining.ShouldEqual(0);
        }
    }
}
