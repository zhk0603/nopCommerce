using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class CustomerAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customerAttribute()
        {
            var ca = TestHelper.GetCustomerAttribute(false);

            var fromDb = SaveAndLoadEntity(ca);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.IsRequired.ShouldEqual(true);
            fromDb.AttributeControlType.ShouldEqual(AttributeControlType.Datepicker);
            fromDb.DisplayOrder.ShouldEqual(2);
        }

        [Test]
        public void Can_save_and_load_customerAttribute_with_values()
        {
            var ca = TestHelper.GetCustomerAttribute();
            var fromDb = SaveAndLoadEntity(ca);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.CustomerAttributeValues.ShouldNotBeNull();
            (fromDb.CustomerAttributeValues.Count == 1).ShouldBeTrue();
            fromDb.CustomerAttributeValues.First().Name.ShouldEqual("Name 2");
        }
    }
}