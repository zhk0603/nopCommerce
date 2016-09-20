using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductWarehouseInventoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productWarehouseInventory()
        {
            var pwi = TestHelper.GetProductWarehouseInventory();

            var fromDb = SaveAndLoadEntity(pwi);
            fromDb.ShouldNotBeNull();
            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Product name 1");
            fromDb.Warehouse.ShouldNotBeNull();
            fromDb.Warehouse.Name.ShouldEqual("Name 2");
            fromDb.StockQuantity.ShouldEqual(3);
            fromDb.ReservedQuantity.ShouldEqual(4);
        }
    }
}
