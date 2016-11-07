using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class CheckoutAttributeExtensionsTests : ServiceTest
    {
        [SetUp]
        public new void SetUp()
        {
        }

        [Test]
        public void Can_remove_shippable_attributes()
        {
            var checkoutAttribute1 = TestHelper.GetCheckoutAttribute();
            checkoutAttribute1.ShippableProductRequired = false;
            var checkoutAttribute2 = TestHelper.GetCheckoutAttribute();
            checkoutAttribute2.Id = 2;
            var checkoutAttribute3 = TestHelper.GetCheckoutAttribute();
            checkoutAttribute3.Id = 3;
            checkoutAttribute3.ShippableProductRequired = false;
            var checkoutAttribute4 = TestHelper.GetCheckoutAttribute();
            checkoutAttribute4.Id = 4;

            var attributes = new List<CheckoutAttribute>
            {
                checkoutAttribute1,
                checkoutAttribute2,
                checkoutAttribute3,
                checkoutAttribute4
            };

            var filtered = attributes.RemoveShippableAttributes();

            filtered.Count.ShouldEqual(2);
            filtered[0].Id.ShouldEqual(1);
            filtered[1].Id.ShouldEqual(3);
        }
    }
}
