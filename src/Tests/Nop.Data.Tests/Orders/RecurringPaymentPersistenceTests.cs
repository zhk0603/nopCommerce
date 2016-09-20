using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class RecurringPaymentPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_recurringPayment()
        {
            var rp = TestHelper.GetRecurringPayment(RecurringProductCyclePeriod.Days, 1);
            rp.Deleted = true;
            rp.InitialOrder = TestHelper.GetOrder();

            var fromDb = SaveAndLoadEntity(rp);
            fromDb.ShouldNotBeNull();
            fromDb.CycleLength.ShouldEqual(1);
            fromDb.CyclePeriod.ShouldEqual(RecurringProductCyclePeriod.Days);
            fromDb.TotalCycles.ShouldEqual(3);
            fromDb.StartDateUtc.ShouldEqual(new DateTime(2010, 03, 01));
            fromDb.IsActive.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.InitialOrder.ShouldNotBeNull();
        }

        [Test]
        public void Can_save_and_load_recurringPayment_with_history()
        {
            var rp = TestHelper.GetRecurringPayment(RecurringProductCyclePeriod.Days, 1, initialOrder: TestHelper.GetOrder(), deleted: true);
            rp.RecurringPaymentHistory.Add(TestHelper.GetRecurringPaymentHistory());

            var fromDb = SaveAndLoadEntity(rp);
            fromDb.ShouldNotBeNull();

            fromDb.RecurringPaymentHistory.ShouldNotBeNull();
            (fromDb.RecurringPaymentHistory.Count == 1).ShouldBeTrue();
            fromDb.RecurringPaymentHistory.First().CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 03));
        }
    }
}