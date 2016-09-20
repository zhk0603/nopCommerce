using System;
using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class ShoppingCartItemPeristenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_shoppingCartItem()
        {
            var sci = TestHelper.GetShoppingCartItem();
            sci.Customer = TestHelper.GetCustomer();
            var fromDb = SaveAndLoadEntity(sci);
            fromDb.ShouldNotBeNull();
            
            fromDb.StoreId.ShouldEqual(1);
            fromDb.ShoppingCartType.ShouldEqual(ShoppingCartType.ShoppingCart);
            fromDb.AttributesXml.ShouldEqual("AttributesXml 1");
            fromDb.CustomerEnteredPrice.ShouldEqual(1);
            fromDb.Quantity.ShouldEqual(2);
            fromDb.RentalStartDateUtc.ShouldEqual(new DateTime(2010, 01, 03));
            fromDb.RentalEndDateUtc.ShouldEqual(new DateTime(2010, 01, 04));
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.Customer.ShouldNotBeNull();
            fromDb.Product.ShouldNotBeNull();
        }
    }
}
