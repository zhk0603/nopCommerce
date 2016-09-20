using System;
using System.Linq;
using Nop.Core.Domain.Discounts;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Discounts
{
    [TestFixture]
    public class DiscountPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_discount()
        {
            var discount = TestHelper.GetDiscount();

            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.DiscountType.ShouldEqual(DiscountType.AssignedToCategories);
            fromDb.Name.ShouldEqual("Discount 1");
            fromDb.UsePercentage.ShouldEqual(true);
            fromDb.DiscountPercentage.ShouldEqual(1.1M);
            fromDb.DiscountAmount.ShouldEqual(2.1M);
            fromDb.MaximumDiscountAmount.ShouldEqual(208.1M);
            fromDb.StartDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.EndDateUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.RequiresCouponCode.ShouldEqual(true);
            fromDb.CouponCode.ShouldEqual("SecretCode");
            fromDb.IsCumulative.ShouldEqual(true);
            fromDb.DiscountLimitation.ShouldEqual(DiscountLimitationType.Unlimited);
            fromDb.LimitationTimes.ShouldEqual(3);
            fromDb.MaximumDiscountedQuantity.ShouldEqual(4);
            fromDb.AppliedToSubCategories.ShouldEqual(true);
        }

        [Test]
        public void Can_save_and_load_discount_with_discountRequirements()
        {
            var discount = TestHelper.GetDiscount();
            discount.DiscountRequirements.Add(TestHelper.GetDiscountRequirement());
            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Discount 1");
            
            fromDb.DiscountRequirements.ShouldNotBeNull();
            (fromDb.DiscountRequirements.Count == 1).ShouldBeTrue();
            fromDb.DiscountRequirements.First().DiscountRequirementRuleSystemName.ShouldEqual("BillingCountryIs");
        }

        [Test]
        public void Can_save_and_load_discount_with_appliedProducts()
        {
            var discount = TestHelper.GetDiscount();
            discount.AppliedToProducts.Add(TestHelper.GetProduct());

            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();

            fromDb.AppliedToProducts.ShouldNotBeNull();
            (fromDb.AppliedToProducts.Count == 1).ShouldBeTrue();
            fromDb.AppliedToProducts.First().Name.ShouldEqual("Product name 1");
        }

        [Test]
        public void Can_save_and_load_discount_with_appliedCategories()
        {
            var discount = TestHelper.GetDiscount();
            discount.AppliedToCategories.Add(TestHelper.GetCategory());

            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();

            fromDb.AppliedToCategories.ShouldNotBeNull();
            (fromDb.AppliedToCategories.Count == 1).ShouldBeTrue();
            fromDb.AppliedToCategories.First().Name.ShouldEqual("Books");
        }
    }
}