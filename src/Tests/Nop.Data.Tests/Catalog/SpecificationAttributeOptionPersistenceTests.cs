using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class SpecificationAttributeOptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_specificationAttributeOption()
        {
            var specificationAttributeOption = TestHelper.GetSpecificationAttributeOption();

            var fromDb = SaveAndLoadEntity(specificationAttributeOption);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("SpecificationAttributeOption name 1");
            fromDb.ColorSquaresRgb.ShouldEqual("ColorSquaresRgb 2");
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.SpecificationAttribute.ShouldNotBeNull();
            fromDb.SpecificationAttribute.Name.ShouldEqual("SpecificationAttribute name 1");
        }
    }
}
