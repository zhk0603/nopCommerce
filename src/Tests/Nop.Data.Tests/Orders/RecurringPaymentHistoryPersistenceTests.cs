using System;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class RecurringPaymentHistoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_recurringPaymentHistory()
        {
            var rph = TestHelper.GetRecurringPaymentHistory(TestHelper.GetRecurringPayment(RecurringProductCyclePeriod.Days, initialOrder: TestHelper.GetOrder()));

            var fromDb = SaveAndLoadEntity(rph);
            fromDb.ShouldNotBeNull();
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 03));

            fromDb.RecurringPayment.ShouldNotBeNull();
            fromDb.RecurringPayment.StartDateUtc.ShouldEqual(new DateTime(2010, 03, 01));
        }
    }
}