using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributeMappingPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productAttributeMapping()
        {
            var productAttributeMapping = TestHelper.GetProductAttributeMapping();

            var fromDb = SaveAndLoadEntity(productAttributeMapping);
            fromDb.ShouldNotBeNull();
            fromDb.TextPrompt.ShouldEqual("TextPrompt 1");
            fromDb.IsRequired.ShouldEqual(true);
            fromDb.AttributeControlType.ShouldEqual(AttributeControlType.DropdownList);
            fromDb.DisplayOrder.ShouldEqual(1);
            fromDb.ValidationMinLength.ShouldEqual(2);
            fromDb.ValidationMaxLength.ShouldEqual(3);
            fromDb.ValidationFileAllowedExtensions.ShouldEqual("ValidationFileAllowedExtensions 1");
            fromDb.ValidationFileMaximumSize.ShouldEqual(4);
            fromDb.DefaultValue.ShouldEqual("DefaultValue 1");
            fromDb.ConditionAttributeXml.ShouldEqual("ConditionAttributeXml 1");

            fromDb.Product.ShouldNotBeNull();

            fromDb.ProductAttribute.ShouldNotBeNull();
            fromDb.ProductAttribute.Name.ShouldEqual("Name 1");
        }
        
    }
}
