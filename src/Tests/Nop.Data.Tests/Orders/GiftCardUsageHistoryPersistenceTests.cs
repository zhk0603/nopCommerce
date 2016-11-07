using System;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class GiftCardUsageHistoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_giftCardUsageHistory()
        {
            var gcuh = TestHelper.GetGiftCardUsageHistory();
            gcuh.GiftCard = TestHelper.GetGiftCard();
            gcuh.UsedWithOrder = TestHelper.GetOrder();

            var fromDb = SaveAndLoadEntity(gcuh);
            fromDb.ShouldNotBeNull();
            fromDb.UsedValue.ShouldEqual(1.1M);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.GiftCard.ShouldNotBeNull();
            fromDb.UsedWithOrder.ShouldNotBeNull();
        }
    }
}