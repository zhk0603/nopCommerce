using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributeValuePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productAttributeValue()
        {
            var pav = TestHelper.GetProductAttributeValue();

            var fromDb = SaveAndLoadEntity(pav);
            fromDb.ShouldNotBeNull();
            fromDb.AttributeValueType.ShouldEqual(AttributeValueType.AssociatedToProduct);
            fromDb.AssociatedProductId.ShouldEqual(10);
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.ColorSquaresRgb.ShouldEqual("12FF33");
            fromDb.ImageSquaresPictureId.ShouldEqual(1);
            fromDb.PriceAdjustment.ShouldEqual(1.1M);
            fromDb.WeightAdjustment.ShouldEqual(2.1M);
            fromDb.Cost.ShouldEqual(3.1M);
            fromDb.Quantity.ShouldEqual(2);
            fromDb.IsPreSelected.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(3);

            fromDb.ProductAttributeMapping.ShouldNotBeNull();
            fromDb.ProductAttributeMapping.TextPrompt.ShouldEqual("TextPrompt 1");
        }
    }
}
