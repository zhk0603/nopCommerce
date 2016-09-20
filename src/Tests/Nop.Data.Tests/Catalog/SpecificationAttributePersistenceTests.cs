using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class SpecificationAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_specificationAttribute()
        {
            var specificationAttribute = TestHelper.GetSpecificationAttribute();

            var fromDb = SaveAndLoadEntity(specificationAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("SpecificationAttribute name 1");
            fromDb.DisplayOrder.ShouldEqual(2);
        }

        [Test]
        public void Can_save_and_load_specificationAttribute_with_specificationAttributeOptions()
        {
            var specificationAttribute = TestHelper.GetSpecificationAttribute();
            specificationAttribute.SpecificationAttributeOptions.Add(TestHelper.GetSpecificationAttributeOption(false));
            var fromDb = SaveAndLoadEntity(specificationAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("SpecificationAttribute name 1");

            fromDb.SpecificationAttributeOptions.ShouldNotBeNull();
            (fromDb.SpecificationAttributeOptions.Count == 1).ShouldBeTrue();
            fromDb.SpecificationAttributeOptions.First().Name.ShouldEqual("SpecificationAttributeOption name 1");
        }
    }
}
