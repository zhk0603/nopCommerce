using Nop.Core;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductPicturePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productPicture()
        {
            var productPicture = TestHelper.GetProductPicture();

            var fromDb = SaveAndLoadEntity(productPicture);
            fromDb.ShouldNotBeNull();
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Product name 1");

            fromDb.Picture.ShouldNotBeNull();
            fromDb.Picture.MimeType.ShouldEqual(MimeTypes.ImagePJpeg);
        }
    }
}
