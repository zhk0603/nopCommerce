using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductManufacturerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productManufacturer()
        {
            var productManufacturer = TestHelper.GetProductManufacturer();

            var fromDb = SaveAndLoadEntity(productManufacturer);
            fromDb.ShouldNotBeNull();
            fromDb.IsFeaturedProduct.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Product name 1");

            fromDb.Manufacturer.ShouldNotBeNull();
            fromDb.Manufacturer.Name.ShouldEqual("Name");
        }
    }
}
