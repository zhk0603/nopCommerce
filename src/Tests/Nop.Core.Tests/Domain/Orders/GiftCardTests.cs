using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Orders
{
    [TestFixture]
    public class GiftCardTests
    {
        [Test]
        public void Can_validate_giftCard()
        {
            var gc = TestHelper.GetGiftCard(true);

            //valid
            gc.IsGiftCardValid().ShouldEqual(true);

            //mark as not active
            gc.IsGiftCardActivated = false;
            gc.IsGiftCardValid().ShouldEqual(false);

            //again active
            gc.IsGiftCardActivated = true;
            gc.IsGiftCardValid().ShouldEqual(true);

            //add usage history record
            gc.GiftCardUsageHistory.Add(new GiftCardUsageHistory
            {
                UsedValue = 1000
            });
            gc.IsGiftCardValid().ShouldEqual(false);
        }

        [Test]
        public void Can_calculate_giftCard_remainingAmount()
        {
            var gc = TestHelper.GetGiftCard(false);

            gc.GetGiftCardRemainingAmount().ShouldEqual(45);
        }
    }
}
