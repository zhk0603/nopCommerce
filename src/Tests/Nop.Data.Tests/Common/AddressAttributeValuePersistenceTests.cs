using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Common
{
    [TestFixture]
    public class CheckoutAttributeValuePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_addressAttributeValue()
        {
            var cav = TestHelper.GetAddressAttributeValue();

            var fromDb = SaveAndLoadEntity(cav);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 2");
            fromDb.IsPreSelected.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.AddressAttribute.ShouldNotBeNull();
            fromDb.AddressAttribute.Name.ShouldEqual("Name 1");
        }
    }
}