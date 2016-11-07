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
            var gc = TestHelper.GetGiftCard();

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
            var gc = TestHelper.GetGiftCard();
            gc.IsGiftCardActivated = false;
            var giftCardUsageHistory1 = TestHelper.GetGiftCardUsageHistory();
            giftCardUsageHistory1.UsedValue = 30;
            var giftCardUsageHistory2 = TestHelper.GetGiftCardUsageHistory();
            giftCardUsageHistory2.UsedValue = 20;
            var giftCardUsageHistory3 = TestHelper.GetGiftCardUsageHistory();
            giftCardUsageHistory3.UsedValue = 5;

            gc.GiftCardUsageHistory.Add(giftCardUsageHistory1);
            gc.GiftCardUsageHistory.Add(giftCardUsageHistory2);
            gc.GiftCardUsageHistory.Add(giftCardUsageHistory3);

            gc.GetGiftCardRemainingAmount().ShouldEqual(45);
        }
    }
}
