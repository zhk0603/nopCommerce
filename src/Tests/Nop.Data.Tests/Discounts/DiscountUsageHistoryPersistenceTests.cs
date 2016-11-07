using System;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Discounts
{
    [TestFixture]
    public class DiscountUsageHistoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_discountUsageHistory()
        {
            var discountHistory = TestHelper.GetDiscountUsageHistory();
            discountHistory.Order = TestHelper.GetOrder();

            var fromDb = SaveAndLoadEntity(discountHistory);
            fromDb.ShouldNotBeNull();
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.Discount.ShouldNotBeNull();
            fromDb.Order.ShouldNotBeNull();
        }
    }
}