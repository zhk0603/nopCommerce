using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributeCombinationPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productAttributeCombination()
        {
            var combination = TestHelper.GetProductAttributeCombination();

            var fromDb = SaveAndLoadEntity(combination);
            fromDb.ShouldNotBeNull();
            fromDb.AttributesXml.ShouldEqual("Some XML");
            fromDb.StockQuantity.ShouldEqual(2);
            fromDb.AllowOutOfStockOrders.ShouldEqual(true);
            fromDb.Sku.ShouldEqual("Sku1");
            fromDb.ManufacturerPartNumber.ShouldEqual("ManufacturerPartNumber1");
            fromDb.Gtin.ShouldEqual("Gtin1");
            fromDb.OverriddenPrice.ShouldEqual(0.01M);
            fromDb.NotifyAdminForQuantityBelow.ShouldEqual(3);
        }
    }
}