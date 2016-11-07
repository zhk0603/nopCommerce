using Nop.Services.Discounts;
﻿using System;
using Nop.Core.Domain.Discounts;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Discounts
{
    public static class DiscountExtensions
    {
        public static decimal GetDiscountAmount(this Discount discount, decimal amount)
        {
            if (discount == null)
                throw new ArgumentNullException("discount");

            return discount.MapDiscount().GetDiscountAmount(amount);
        }
    }

    [TestFixture]
    public class DiscountExtensionsTests : ServiceTest
    {
        [Test]
        public void Can_calculate_discount_amount_percentage()
        {
            var discount = TestHelper.GetDiscount(discountPercentage: 30);

            discount.GetDiscountAmount(100).ShouldEqual(30);
            discount.DiscountPercentage = 60;
            discount.GetDiscountAmount(200).ShouldEqual(120);
        }

        [Test]
        public void Can_calculate_discount_amount_fixed()
        {
            var discount = TestHelper.GetDiscount(discountAmount: 10, usePercentage: false);

            discount.GetDiscountAmount(100).ShouldEqual(10);
            discount.DiscountAmount = 20;
            discount.GetDiscountAmount(200).ShouldEqual(20);
        }

        [Test]
        public void Maximum_discount_amount_is_used()
        {
            var discount = TestHelper.GetDiscount(discountPercentage: 30, maximumDiscountAmount: 3.4M);

            discount.GetDiscountAmount(100).ShouldEqual(3.4M);
            discount.DiscountPercentage = 60;
            discount.GetDiscountAmount(200).ShouldEqual(3.4M);
            discount.GetDiscountAmount(100).ShouldEqual(3.4M);
            discount.DiscountPercentage = 1;
            discount.GetDiscountAmount(200).ShouldEqual(2);
        }
    }
}
