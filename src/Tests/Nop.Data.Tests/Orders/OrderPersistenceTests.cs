using System;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class OrderPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_order()
        {
            var order = TestHelper.GetOrder();
            var fromDb = SaveAndLoadEntity(order);

            fromDb.ShouldNotBeNull();
            fromDb.StoreId.ShouldEqual(1);
            fromDb.Customer.ShouldNotBeNull();
            fromDb.OrderStatus.ShouldEqual(OrderStatus.Complete);
            fromDb.ShippingStatus.ShouldEqual(ShippingStatus.Shipped);
            fromDb.PaymentStatus.ShouldEqual(PaymentStatus.Paid);
            fromDb.PaymentMethodSystemName.ShouldEqual("PaymentMethodSystemName1");
            fromDb.CustomerCurrencyCode.ShouldEqual("RUR");
            fromDb.CurrencyRate.ShouldEqual(1.1M);
            fromDb.CustomerTaxDisplayType.ShouldEqual(TaxDisplayType.ExcludingTax);
            fromDb.VatNumber.ShouldEqual("123456789");
            fromDb.OrderSubtotalInclTax.ShouldEqual(2.1M);
            fromDb.OrderSubtotalExclTax.ShouldEqual(3.1M);
            fromDb.OrderSubTotalDiscountInclTax.ShouldEqual(4.1M);
            fromDb.OrderSubTotalDiscountExclTax.ShouldEqual(5.1M);
            fromDb.OrderShippingInclTax.ShouldEqual(6.1M);
            fromDb.OrderShippingExclTax.ShouldEqual(7.1M);
            fromDb.PaymentMethodAdditionalFeeInclTax.ShouldEqual(8.1M);
            fromDb.PaymentMethodAdditionalFeeExclTax.ShouldEqual(9.1M);
            fromDb.TaxRates.ShouldEqual("1,3,5,7");
            fromDb.OrderTax.ShouldEqual(10.1M);
            fromDb.OrderDiscount.ShouldEqual(11.1M);
            fromDb.OrderTotal.ShouldEqual(12.1M);
            fromDb.RefundedAmount.ShouldEqual(13.1M);
            fromDb.RewardPointsWereAdded.ShouldEqual(true);
            fromDb.CheckoutAttributeDescription.ShouldEqual("CheckoutAttributeDescription1");
            fromDb.CheckoutAttributesXml.ShouldEqual("CheckoutAttributesXml1");
            fromDb.CustomerLanguageId.ShouldEqual(14);
            fromDb.CustomerIp.ShouldEqual("CustomerIp1");
            fromDb.AllowStoringCreditCardNumber.ShouldEqual(true);
            fromDb.CardType.ShouldEqual("Visa");
            fromDb.CardName.ShouldEqual("John Smith");
            fromDb.CardNumber.ShouldEqual("4111111111111111");
            fromDb.MaskedCreditCardNumber.ShouldEqual("************1111");
            fromDb.CardCvv2.ShouldEqual("123");
            fromDb.CardExpirationMonth.ShouldEqual("12");
            fromDb.CardExpirationYear.ShouldEqual("2010");
            fromDb.AuthorizationTransactionId.ShouldEqual("AuthorizationTransactionId1");
            fromDb.AuthorizationTransactionCode.ShouldEqual("AuthorizationTransactionCode1");
            fromDb.AuthorizationTransactionResult.ShouldEqual("AuthorizationTransactionResult1");
            fromDb.CaptureTransactionId.ShouldEqual("CaptureTransactionId1");
            fromDb.CaptureTransactionResult.ShouldEqual("CaptureTransactionResult1");
            fromDb.SubscriptionTransactionId.ShouldEqual("SubscriptionTransactionId1");
            fromDb.PaidDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.BillingAddress.ShouldNotBeNull();
            fromDb.BillingAddress.FirstName.ShouldEqual("FirstName 1");
            fromDb.ShippingAddress.ShouldBeNull();
            fromDb.PickupAddress.ShouldNotBeNull();
            fromDb.PickupAddress.LastName.ShouldEqual("LastName 1");
            fromDb.ShippingMethod.ShouldEqual("ShippingMethod1");
            fromDb.ShippingRateComputationMethodSystemName.ShouldEqual("ShippingRateComputationMethodSystemName1");
            fromDb.PickUpInStore.ShouldEqual(true);
            fromDb.CustomValuesXml.ShouldEqual("CustomValuesXml1");
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_order_with_shipping_address()
        {
            var order = TestHelper.GetOrder();
            order.ShippingAddress = TestHelper.GetAddress();

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.ShippingAddress.ShouldNotBeNull();
            fromDb.ShippingAddress.FirstName.ShouldEqual("FirstName 1");
        }

        [Test]
        public void Can_save_and_load_order_with_usedRewardPoints()
        {
            var order = TestHelper.GetOrder();
            order.RedeemedRewardPointsEntry = TestHelper.GetRewardPointsHistory();

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.RedeemedRewardPointsEntry.ShouldNotBeNull();
            fromDb.RedeemedRewardPointsEntry.Points.ShouldEqual(2);
        }

        [Test]
        public void Can_save_and_load_order_with_discountUsageHistory()
        {
            var order = TestHelper.GetOrder();
            order.DiscountUsageHistory.Add(TestHelper.GetDiscountUsageHistory());
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.DiscountUsageHistory.ShouldNotBeNull();
            fromDb.DiscountUsageHistory.ShouldNotBeNull();
            fromDb.DiscountUsageHistory.Count.ShouldEqual(1);
            fromDb.DiscountUsageHistory.First().CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_order_with_giftCardUsageHistory()
        {
            var order = TestHelper.GetOrder();
            var gcuh = TestHelper.GetGiftCardUsageHistory();
            gcuh.GiftCard = TestHelper.GetGiftCard();
            order.GiftCardUsageHistory.Add(gcuh);
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.GiftCardUsageHistory.ShouldNotBeNull();
            fromDb.GiftCardUsageHistory.Count.ShouldEqual(1);
            fromDb.GiftCardUsageHistory.First().CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_order_with_orderNotes()
        {
            var order = TestHelper.GetOrder();
            order.OrderNotes.Add(TestHelper.GetOrderNote());
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.OrderNotes.ShouldNotBeNull();
            fromDb.OrderNotes.Count.ShouldEqual(1);
            fromDb.OrderNotes.First().Note.ShouldEqual("Note1");
        }

        [Test]
        public void Can_save_and_load_order_with_orderItems()
        {
            var order = TestHelper.GetOrder();
            order.OrderItems.Add(TestHelper.GetOrderItem());
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.OrderItems.ShouldNotBeNull();
            fromDb.OrderItems.Count.ShouldEqual(1);
            fromDb.OrderItems.First().Quantity.ShouldEqual(1);
        }
        
        [Test]
        public void Can_save_and_load_order_with_shipments()
        {
            var order = TestHelper.GetOrder();
            order.Shipments.Add(TestHelper.GetShipment());
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.Shipments.ShouldNotBeNull();
            fromDb.Shipments.Count.ShouldEqual(1);
            fromDb.Shipments.First().TrackingNumber.ShouldEqual("TrackingNumber 1");
        }
    }
}
