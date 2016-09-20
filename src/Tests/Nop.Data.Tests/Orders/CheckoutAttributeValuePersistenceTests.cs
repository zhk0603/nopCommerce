using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class CheckoutAttributeValuePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_checkoutAttributeValue()
        {
            var cav = TestHelper.GetCheckoutAttributeValue();
            cav.CheckoutAttribute = TestHelper.GetCheckoutAttribute();

            var fromDb = SaveAndLoadEntity(cav);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 2");
            fromDb.ColorSquaresRgb.ShouldEqual("#112233");
            fromDb.PriceAdjustment.ShouldEqual(1);
            fromDb.WeightAdjustment.ShouldEqual(2);
            fromDb.IsPreSelected.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(3);

            fromDb.CheckoutAttribute.ShouldNotBeNull();
            fromDb.CheckoutAttribute.Name.ShouldEqual("Name 1");
        }
    }
}