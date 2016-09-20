using System;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class BackInStockSubscriptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_backInStockSubscription()
        {
            var backInStockSubscription = TestHelper.GetBackInStockSubscription();

            var fromDb = SaveAndLoadEntity(backInStockSubscription);
            fromDb.ShouldNotBeNull();

            fromDb.Product.ShouldNotBeNull();

            fromDb.Customer.ShouldNotBeNull();

            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}
