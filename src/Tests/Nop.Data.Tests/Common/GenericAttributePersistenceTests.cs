using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Common
{
    [TestFixture]
    public class GenericAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_genericAttribute()
        {
            var genericAttribute = TestHelper.GetGenericAttribute();

            var fromDb = SaveAndLoadEntity(genericAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.EntityId.ShouldEqual(1);
            fromDb.KeyGroup.ShouldEqual("Customer");
            fromDb.Key.ShouldEqual("Key 1");
            fromDb.Value.ShouldEqual("Value 1");
            fromDb.StoreId.ShouldEqual(2);
        }
    }
}