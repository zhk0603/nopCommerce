using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class TierPricePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_tierPrice()
        {
            var tierPrice = TestHelper.GetTierPrice(TestHelper.GetProduct());

            var fromDb = SaveAndLoadEntity(tierPrice);
            fromDb.ShouldNotBeNull();
            fromDb.StoreId.ShouldEqual(1);
            fromDb.Quantity.ShouldEqual(1);
            fromDb.Price.ShouldEqual(2.1M);

            fromDb.Product.ShouldNotBeNull();
        }

        [Test]
        public void Can_save_and_load_tierPriceWithCustomerRole()
        {
            var tierPrice = TestHelper.GetTierPrice(TestHelper.GetProduct(), TestHelper.GetCustomerRole("Administrators"));

            var fromDb = SaveAndLoadEntity(tierPrice);
            fromDb.ShouldNotBeNull();

            fromDb.CustomerRole.ShouldNotBeNull();
            fromDb.CustomerRole.Name.ShouldEqual("Administrators");
        }
    }
}
