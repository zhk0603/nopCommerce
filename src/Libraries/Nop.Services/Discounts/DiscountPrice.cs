using System.Collections.Generic;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Represents the details of result of requesting discount price
    /// </summary>
    public class DiscountPrice
    {
        public DiscountPrice()
        {
            AppliedDiscounts = new List<DiscountForCaching>();
            DiscountPricesByQuantity = new List<DiscountPriceByQuantity>();
        }

        /// <summary>
        /// Gets or sets a price with discounts (the same for all items)
        /// </summary>
        public decimal PriceWithDiscount { get; set; }

        /// <summary>
        /// Gets or sets a discount amount (the same for all items)
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets an applied discounts
        /// </summary>
        public List<DiscountForCaching> AppliedDiscounts { get; set; }

        /// <summary>
        /// Gets or sets a distribution of prices by quantity (in case if there are applied discounts with MaximumDiscountedQuantity)
        /// </summary>
        public List<DiscountPriceByQuantity> DiscountPricesByQuantity { get; set; }
    }

    /// <summary>
    /// Represents a request of discount price
    /// </summary>
    public class DiscountPriceRequest
    {
        public DiscountPriceRequest(decimal priceWithoutDiscounts, int quantity = 1)
        {
            PriceWithoutDiscounts = priceWithoutDiscounts;
            Quantity = quantity;
        }

        /// <summary>
        /// Gets or sets an original price without any discounts
        /// </summary>
        public decimal PriceWithoutDiscounts { get; set; }

        /// <summary>
        /// Gets or sets a quantity
        /// </summary>
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Represents a price with discount for particular quantity
    /// </summary>
    public class DiscountPriceByQuantity
    {
        /// <summary>
        /// Gets or sets a price
        /// </summary>
        public decimal DiscountPrice { get; set; }

        /// <summary>
        /// Gets or sets a quantity for which is used a certain price with discount
        /// </summary>
        public int Quantity { get; set; }
    }
}
