using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductSpecificationAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productSpecificationAttribute()
        {
            var productSpecificationAttribute = TestHelper.GetProductSpecificationAttribute();

            var fromDb = SaveAndLoadEntity(productSpecificationAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.AttributeType.ShouldEqual(SpecificationAttributeType.Hyperlink);
            fromDb.AllowFiltering.ShouldEqual(true);
            fromDb.ShowOnProductPage.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Product name 1");

            fromDb.SpecificationAttributeOption.ShouldNotBeNull();
            fromDb.SpecificationAttributeOption.Name.ShouldEqual("SpecificationAttributeOption name 1");
        }
    }
}
