using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class GiftCardPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_giftCard()
        {
            var giftCard = TestHelper.GetGiftCard();

            var fromDb = SaveAndLoadEntity(giftCard);
            fromDb.ShouldNotBeNull();
            fromDb.GiftCardType.ShouldEqual(GiftCardType.Physical);
            fromDb.Amount.ShouldEqual(100);
            fromDb.IsGiftCardActivated.ShouldEqual(true);
            fromDb.GiftCardCouponCode.ShouldEqual("Secret");
            fromDb.RecipientName.ShouldEqual("RecipientName 1");
            fromDb.RecipientEmail.ShouldEqual("a@b.c");
            fromDb.SenderName.ShouldEqual("SenderName 1");
            fromDb.SenderEmail.ShouldEqual("d@e.f");
            fromDb.Message.ShouldEqual("Message 1");
            fromDb.IsRecipientNotified.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_giftCard_with_usageHistory()
        {
            var giftCard = TestHelper.GetGiftCard();
            var gcuh = TestHelper.GetGiftCardUsageHistory();
            gcuh.UsedWithOrder = TestHelper.GetOrder();
            giftCard.GiftCardUsageHistory.Add(gcuh);
            var fromDb = SaveAndLoadEntity(giftCard);
            fromDb.ShouldNotBeNull();

            fromDb.GiftCardUsageHistory.ShouldNotBeNull();
            (fromDb.GiftCardUsageHistory.Count == 1).ShouldBeTrue();
            fromDb.GiftCardUsageHistory.First().UsedValue.ShouldEqual(1.1M);
        }
        
        [Test]
        public void Can_save_and_load_giftCard_with_associatedItem()
        {
            var giftCard = TestHelper.GetGiftCard();
            var oi = TestHelper.GetOrderItem();
            oi.Order = TestHelper.GetOrder();
            giftCard.PurchasedWithOrderItem = oi;

            var fromDb = SaveAndLoadEntity(giftCard);
            fromDb.ShouldNotBeNull();
            
            fromDb.PurchasedWithOrderItem.ShouldNotBeNull();
            fromDb.PurchasedWithOrderItem.Product.ShouldNotBeNull();
            fromDb.PurchasedWithOrderItem.Product.Name.ShouldEqual("Product name 1");
        }
    }
}