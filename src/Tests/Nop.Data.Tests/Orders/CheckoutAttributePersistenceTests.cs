using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class CheckoutAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_checkoutAttribute()
        {
            var ca = TestHelper.GetCheckoutAttribute();

            var fromDb = SaveAndLoadEntity(ca);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.TextPrompt.ShouldEqual("TextPrompt 1");
            fromDb.IsRequired.ShouldEqual(true);
            fromDb.ShippableProductRequired.ShouldEqual(true);
            fromDb.IsTaxExempt.ShouldEqual(true);
            fromDb.TaxCategoryId.ShouldEqual(1);
            fromDb.AttributeControlType.ShouldEqual(AttributeControlType.Datepicker);
            fromDb.DisplayOrder.ShouldEqual(2);
            fromDb.ValidationMinLength.ShouldEqual(3);
            fromDb.ValidationMaxLength.ShouldEqual(4);
            fromDb.ValidationFileAllowedExtensions.ShouldEqual("ValidationFileAllowedExtensions 1");
            fromDb.ValidationFileMaximumSize.ShouldEqual(5);
            fromDb.DefaultValue.ShouldEqual("DefaultValue 1");
            fromDb.LimitedToStores.ShouldEqual(true);
            fromDb.ConditionAttributeXml.ShouldEqual("ConditionAttributeXml 1");
        }

        [Test]
        public void Can_save_and_load_checkoutAttribute_with_values()
        {
            var ca = TestHelper.GetCheckoutAttribute();
            ca.CheckoutAttributeValues.Add(TestHelper.GetCheckoutAttributeValue());
            var fromDb = SaveAndLoadEntity(ca);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.CheckoutAttributeValues.ShouldNotBeNull();
            (fromDb.CheckoutAttributeValues.Count == 1).ShouldBeTrue();
            fromDb.CheckoutAttributeValues.First().Name.ShouldEqual("Name 2");
        }
    }
}