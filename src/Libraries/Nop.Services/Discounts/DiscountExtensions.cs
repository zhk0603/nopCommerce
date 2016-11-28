using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts
{
    public static class DiscountExtensions
    {
        /// <summary>
        /// Gets the discount amount for the specified value
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="amount">Amount</param>
        /// <returns>The discount amount</returns>
        public static decimal GetDiscountAmount(this DiscountForCaching discount, decimal amount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //calculate discount amount
            var result = discount.UsePercentage ? (decimal)((((float)amount) * ((float)discount.DiscountPercentage)) / 100f) : discount.DiscountAmount;

            //validate maximum discount amount
            if (discount.UsePercentage && discount.MaximumDiscountAmount.HasValue && result > discount.MaximumDiscountAmount.Value)
                result = discount.MaximumDiscountAmount.Value;

            return Math.Max(decimal.Zero, result);
        }

        /// <summary>
        /// Get discount price details
        /// </summary>
        /// <param name="allowedDiscounts">A list of allowed discounts</param>
        /// <param name="requestDetails">Details of the request of discount price</param>
        /// <returns>Result of requesting discount price</returns>
        public static DiscountPrice GetDiscountPrice(this IList<DiscountForCaching> allowedDiscounts, DiscountPriceRequest requestDetails)
        {
            var result = new DiscountPrice { PriceWithDiscount = requestDetails.PriceWithoutDiscounts };

            if (!allowedDiscounts.Any())
                return result;

            //find all possible discounts
            var allPossibleDiscounts = allowedDiscounts.Select(discount =>
            {
                var discountAmount = discount.GetDiscountAmount(requestDetails.PriceWithoutDiscounts);

                //check whether MaximumDiscountedQuantity is set
                var discountQuantity = !discount.MaximumDiscountedQuantity.HasValue || discount.MaximumDiscountedQuantity.Value > requestDetails.Quantity
                    ? requestDetails.Quantity : discount.MaximumDiscountedQuantity.Value;

                return new
                {
                    Discount = discount,
                    DiscountAmount = discountAmount,
                    DiscountQuantity = discountQuantity,
                    TotalDiscountAmount = discountAmount * discountQuantity
                };
            }).ToList();

            //get non-cumulative discounts
            var nonCumulativeDiscounts = allPossibleDiscounts.Where(discount => !discount.Discount.IsCumulative).ToList();
            
            //get cumulative discounts
            var cumulativeDiscounts = allPossibleDiscounts.Where(discount => discount.Discount.IsCumulative).ToList();
            var appliedDiscounts = cumulativeDiscounts;

            if (nonCumulativeDiscounts.Any())
            {
                //there can be only one non-cumulative discount, thus select preferred one 
                var preferredNonCumulativeDiscount = nonCumulativeDiscounts.Aggregate((current, next) => current.TotalDiscountAmount > next.TotalDiscountAmount ? current : next);

                //select preferred discount or all cumulative ones
                if (!cumulativeDiscounts.Any() || preferredNonCumulativeDiscount.TotalDiscountAmount > cumulativeDiscounts.Sum(discount => discount.TotalDiscountAmount))
                    appliedDiscounts = new[] { preferredNonCumulativeDiscount }.ToList();
            }

            //set original price for quantity if there are no discounts for all quantity
            if (!appliedDiscounts.Any(discount => discount.DiscountQuantity == requestDetails.Quantity))
            {
                result.DiscountPricesByQuantity.Add(new DiscountPriceByQuantity
                {
                    Quantity = requestDetails.Quantity,
                    DiscountPrice = requestDetails.PriceWithoutDiscounts
                });
            }

            //summarize discount amount and get the price with discounts for all quantities
            result.DiscountPricesByQuantity.AddRange(appliedDiscounts.Select(discount => discount.DiscountQuantity).Distinct()
                .Select(quantity => new DiscountPriceByQuantity
                {
                    Quantity = quantity,
                    DiscountPrice = requestDetails.PriceWithoutDiscounts - appliedDiscounts.Where(discount => discount.DiscountQuantity >= quantity).Sum(discount => discount.DiscountAmount)
                }));
            result.DiscountPricesByQuantity = result.DiscountPricesByQuantity.OrderBy(priceByQuantity => priceByQuantity.Quantity).ToList();

            result.PriceWithDiscount = result.DiscountPricesByQuantity.FirstOrDefault(priceByQuantity => priceByQuantity.Quantity == requestDetails.Quantity).DiscountPrice;
            result.DiscountAmount = requestDetails.PriceWithoutDiscounts - result.PriceWithDiscount;
            result.AppliedDiscounts = appliedDiscounts.Select(discount => discount.Discount).ToList();

            return result;
        }

        /// <summary>
        /// Get subtotal amount with discounts
        /// </summary>
        /// <param name="discountPrice">Discount ptice details</param>
        /// <returns>Subtotal amount</returns>
        public static decimal GetSubTotalWithDiscounts(this DiscountPrice discountPrice)
        {
            var previousQuantity = 0;

            return discountPrice.DiscountPricesByQuantity.Sum(x =>
            {
                //get the quantity for which price is used 
                var discountQuantity = x.Quantity - previousQuantity;
                previousQuantity = x.Quantity;

                return discountQuantity * x.DiscountPrice;
            });
        }

        /// <summary>
        /// Check whether a list of discounts already contains a certain discount intance
        /// </summary>
        /// <param name="discounts">A list of discounts</param>
        /// <param name="discount">Discount to check</param>
        /// <returns>Result</returns>
        public static bool ContainsDiscount(this IList<DiscountForCaching> discounts,
            DiscountForCaching discount)
        {
            if (discounts == null)
                throw new ArgumentNullException(nameof(discounts));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            return discounts.Any(x => x.Id == discount.Id);
        }

        /// <summary>
        /// Map a discount to the same class for caching
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>Result</returns>
        public static DiscountForCaching MapDiscount(this Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            return new DiscountForCaching
            {
                Id = discount.Id,
                Name = discount.Name,
                DiscountTypeId = discount.DiscountTypeId,
                UsePercentage = discount.UsePercentage,
                DiscountPercentage = discount.DiscountPercentage,
                DiscountAmount = discount.DiscountAmount,
                MaximumDiscountAmount = discount.MaximumDiscountAmount,
                StartDateUtc = discount.StartDateUtc,
                EndDateUtc = discount.EndDateUtc,
                RequiresCouponCode = discount.RequiresCouponCode,
                CouponCode = discount.CouponCode,
                IsCumulative = discount.IsCumulative,
                DiscountLimitationId = discount.DiscountLimitationId,
                LimitationTimes = discount.LimitationTimes,
                MaximumDiscountedQuantity = discount.MaximumDiscountedQuantity,
                AppliedToSubCategories = discount.AppliedToSubCategories
            };
        }
    }
}
