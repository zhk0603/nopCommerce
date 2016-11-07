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
            var rp = TestHelper.GetRecurringPayment();
            rp.InitialOrder = TestHelper.GetOrder();

            var rph = TestHelper.GetRecurringPaymentHistory();
            rph.RecurringPayment = rp;
            
            var fromDb = SaveAndLoadEntity(rph);
            fromDb.ShouldNotBeNull();
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 03));

            fromDb.RecurringPayment.ShouldNotBeNull();
            fromDb.RecurringPayment.StartDateUtc.ShouldEqual(new DateTime(2010, 03, 01));
        }
    }
}