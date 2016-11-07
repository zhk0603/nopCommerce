using System;
using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class OrderItemPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_orderItem()
        {
            var orderItem = TestHelper.GetOrderItem();
            orderItem.Order = TestHelper.GetOrder();
            var fromDb = SaveAndLoadEntity(orderItem);
            fromDb.ShouldNotBeNull();
            fromDb.Order.ShouldNotBeNull();
            fromDb.Product.ShouldNotBeNull();
            fromDb.UnitPriceInclTax.ShouldEqual(1.1M);
            fromDb.UnitPriceExclTax.ShouldEqual(2.1M);
            fromDb.PriceInclTax.ShouldEqual(3.1M);
            fromDb.PriceExclTax.ShouldEqual(4.1M);
            fromDb.DiscountAmountInclTax.ShouldEqual(5.1M);
            fromDb.DiscountAmountExclTax.ShouldEqual(6.1M);
            fromDb.OriginalProductCost.ShouldEqual(7.1M);
            fromDb.AttributeDescription.ShouldEqual("AttributeDescription1");
            fromDb.AttributesXml.ShouldEqual("AttributesXml1");
            fromDb.DownloadCount.ShouldEqual(7);
            fromDb.IsDownloadActivated.ShouldEqual(true);
            fromDb.LicenseDownloadId.ShouldEqual(8);
            fromDb.ItemWeight.ShouldEqual(9.87M);
            fromDb.RentalStartDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.RentalEndDateUtc.ShouldEqual(new DateTime(2010, 01, 02));

            fromDb.Order.ShouldNotBeNull();
        }

        [Test]
        public void Can_save_and_load_orderItem_with_giftCard()
        {
            var orderItem = TestHelper.GetOrderItem();
            orderItem.Order = TestHelper.GetOrder();
            orderItem.AssociatedGiftCards.Add(TestHelper.GetGiftCard());

            var fromDb = SaveAndLoadEntity(orderItem);
            fromDb.ShouldNotBeNull();

            fromDb.AssociatedGiftCards.ShouldNotBeNull();
            (fromDb.AssociatedGiftCards.Count == 1).ShouldBeTrue();
            fromDb.AssociatedGiftCards.First().Amount.ShouldEqual(100);
        }
    }
}