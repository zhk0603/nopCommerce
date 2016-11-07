using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class CheckoutAttributeValuePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerAttributeValue()
        {
            var cav = TestHelper.GetCustomerAttributeValue();

            var fromDb = SaveAndLoadEntity(cav);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 2");
            fromDb.IsPreSelected.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.CustomerAttribute.ShouldNotBeNull();
            fromDb.CustomerAttribute.Name.ShouldEqual("Name 1");
        }
    }
}