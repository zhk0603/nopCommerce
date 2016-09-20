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
            var attributes = new List<CheckoutAttribute>
            {
                TestHelper.GetCheckoutAttribute(1, false),
                TestHelper.GetCheckoutAttribute(2),
                TestHelper.GetCheckoutAttribute(3, false),
                TestHelper.GetCheckoutAttribute(4)
            };

            var filtered = attributes.RemoveShippableAttributes();

            filtered.Count.ShouldEqual(2);
            filtered[0].Id.ShouldEqual(1);
            filtered[1].Id.ShouldEqual(3);
        }
    }
}
