using System.ComponentModel;
using System.Linq;
using Nop.Core.Domain.Shipping;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Shipping
{
    [TestFixture]
    public class ShippingOptionTypeConverterTests
    {
        [SetUp]
        public void SetUp()
        {
            TypeDescriptor.AddAttributes(typeof(ShippingOption),
                new TypeConverterAttribute(typeof(ShippingOptionTypeConverter)));
        }

        [Test]
        public void Can_get_type_converter()
        {
            var converter = TypeDescriptor.GetConverter(typeof(ShippingOption));
            converter.GetType().ShouldEqual(typeof(ShippingOptionTypeConverter));
        }

        [Test]
        public void Can_convert_shippingOption_to_string_and_back()
        {
            var shippingOptionInput = TestHelper.GetShippingOptions().First();

            var converter = TypeDescriptor.GetConverter(shippingOptionInput.GetType());
            var result = converter.ConvertTo(shippingOptionInput, typeof(string)) as string;

            var shippingOptionOutput = converter.ConvertFrom(result) as ShippingOption;
            shippingOptionOutput.ShouldNotBeNull();
            shippingOptionOutput.Name.ShouldEqual("a1");
            shippingOptionOutput.Description.ShouldEqual("a2");
            shippingOptionOutput.Rate.ShouldEqual(3.57M);
            shippingOptionOutput.ShippingRateComputationMethodSystemName.ShouldEqual("a4");
        }
    }
}
