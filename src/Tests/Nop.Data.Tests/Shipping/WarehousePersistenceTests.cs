using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Shipping
{
    [TestFixture]
    public class WarehousePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_warehouse()
        {
            var warehouse = TestHelper.GetWarehouse();

            var fromDb = SaveAndLoadEntity(warehouse);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 2");
            fromDb.AdminComment.ShouldEqual("AdminComment 1");
            fromDb.AddressId.ShouldEqual(1);
        }
    }
}