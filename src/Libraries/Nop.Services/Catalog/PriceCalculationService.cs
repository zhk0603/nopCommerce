using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog.Cache;
using Nop.Services.Customers;
using Nop.Services.Discounts;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Price calculation service
    /// </summary>
    public partial class PriceCalculationService : IPriceCalculationService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IDiscountService _discountService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        public PriceCalculationService(IWorkContext workContext,
            IStoreContext storeContext,
            IDiscountService discountService, 
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductAttributeParser productAttributeParser, 
            IProductService productService,
            IStaticCacheManager cacheManager,
            ShoppingCartSettings shoppingCartSettings, 
            CatalogSettings catalogSettings)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._discountService = discountService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productAttributeParser = productAttributeParser;
            this._productService = productService;
            this._cacheManager = cacheManager;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
        }
        
        #endregion

        #region Nested classes

        [Serializable]
        protected class ProductPriceForCaching
        {
            public ProductPriceForCaching()
            {
                DiscountPrice = new DiscountPrice();
            }

            public decimal Price { get; set; }
            
            public DiscountPrice DiscountPrice { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets allowed discounts applied to product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<DiscountForCaching> GetAllowedDiscountsAppliedToProduct(Product product, Customer customer)
        {
            var allowedDiscounts = new List<DiscountForCaching>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            if (product.HasDiscountsApplied)
            {
                //we use this property ("HasDiscountsApplied") for performance optimization to avoid unnecessary database calls
                foreach (var discount in product.AppliedDiscounts)
                {
                    if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                        discount.DiscountType == DiscountType.AssignedToSkus)
                        allowedDiscounts.Add(discount.MapDiscount());
                }
            }

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts applied to categories
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<DiscountForCaching> GetAllowedDiscountsAppliedToCategories(Product product, Customer customer)
        {
            var allowedDiscounts = new List<DiscountForCaching>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            //load cached discount models (performance optimization)
            foreach (var discount in _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToCategories))
            {
                //load identifier of categories with this discount applied to
                var discountCategoryIds = _discountService.GetAppliedCategoryIds(discount, customer);

                //compare with categories of this product
                var productCategoryIds = new List<int>();
                if (discountCategoryIds.Any())
                {
                    //load identifier of categories of this product
                    var cacheKey = string.Format(PriceCacheEventConsumer.PRODUCT_CATEGORY_IDS_MODEL_KEY,
                        product.Id,
                        string.Join(",", customer.GetCustomerRoleIds()),
                        _storeContext.CurrentStore.Id);
                    productCategoryIds = _cacheManager.Get(cacheKey, () =>
                        _categoryService
                        .GetProductCategoriesByProductId(product.Id)
                        .Select(x => x.CategoryId)
                        .ToList());
                }

                foreach (var categoryId in productCategoryIds)
                {
                    if (discountCategoryIds.Contains(categoryId))
                    {
                        if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                            !allowedDiscounts.ContainsDiscount(discount))
                            allowedDiscounts.Add(discount);
                    }
                }
            }

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts applied to manufacturers
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<DiscountForCaching> GetAllowedDiscountsAppliedToManufacturers(Product product, Customer customer)
        {
            var allowedDiscounts = new List<DiscountForCaching>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            foreach (var discount in _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers))
            {
                //load identifier of manufacturers with this discount applied to
                var discountManufacturerIds = _discountService.GetAppliedManufacturerIds(discount, customer);

                //compare with manufacturers of this product
                var productManufacturerIds = new List<int>();
                if (discountManufacturerIds.Any())
                {
                    //load identifier of manufacturers of this product
                    var cacheKey = string.Format(PriceCacheEventConsumer.PRODUCT_MANUFACTURER_IDS_MODEL_KEY,
                        product.Id,
                        string.Join(",", customer.GetCustomerRoleIds()),
                        _storeContext.CurrentStore.Id);
                    productManufacturerIds = _cacheManager.Get(cacheKey, () =>
                        _manufacturerService
                        .GetProductManufacturersByProductId(product.Id)
                        .Select(x => x.ManufacturerId)
                        .ToList());
                }

                foreach (var manufacturerId in productManufacturerIds)
                {
                    if (discountManufacturerIds.Contains(manufacturerId))
                    {
                        if (_discountService.ValidateDiscount(discount, customer).IsValid &&
                            !allowedDiscounts.ContainsDiscount(discount))
                            allowedDiscounts.Add(discount);
                    }
                }
            }

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<DiscountForCaching> GetAllowedDiscounts(Product product, Customer customer)
        {
            var allowedDiscounts = new List<DiscountForCaching>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            //discounts applied to products
            foreach (var discount in GetAllowedDiscountsAppliedToProduct(product, customer))
                if (!allowedDiscounts.ContainsDiscount(discount))
                    allowedDiscounts.Add(discount);

            //discounts applied to categories
            foreach (var discount in GetAllowedDiscountsAppliedToCategories(product, customer))
                if (!allowedDiscounts.ContainsDiscount(discount))
                    allowedDiscounts.Add(discount);

            //discounts applied to manufacturers
            foreach (var discount in GetAllowedDiscountsAppliedToManufacturers(product, customer))
                if (!allowedDiscounts.ContainsDiscount(discount))
                    allowedDiscounts.Add(discount);

            return allowedDiscounts;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Product product, Customer customer,
            decimal additionalCharge = decimal.Zero, bool includeDiscounts = true, int quantity = 1)
        {
            DiscountPrice discountPrice;
            return GetFinalPrice(product, customer, additionalCharge, includeDiscounts, quantity, out discountPrice);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="discountPrice">Discount price details</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Product product, Customer customer,
            decimal additionalCharge, bool includeDiscounts, int quantity, out DiscountPrice discountPrice)
        {
            return GetFinalPrice(product, customer, additionalCharge, includeDiscounts, quantity, null, null, out discountPrice);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental products)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental products)</param>
        /// <param name="discountPrice">Discount price details</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Product product, Customer customer,
            decimal additionalCharge, bool includeDiscounts, int quantity,
            DateTime? rentalStartDate, DateTime? rentalEndDate, out DiscountPrice discountPrice)
        {
            return GetFinalPrice(product, customer, null, additionalCharge, includeDiscounts, quantity,
                rentalStartDate, rentalEndDate, out discountPrice);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="overriddenProductPrice">Overridden product price. If specified, then it'll be used instead of a product price. For example, used with product attribute combinations</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental products)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental products)</param>
        /// <param name="discountPrice">Discount price details</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Product product, Customer customer,
            decimal? overriddenProductPrice, decimal additionalCharge, bool includeDiscounts, int quantity,
            DateTime? rentalStartDate, DateTime? rentalEndDate, out DiscountPrice discountPrice)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var cacheKey = string.Format(PriceCacheEventConsumer.PRODUCT_PRICE_MODEL_KEY,
                product.Id,
                overriddenProductPrice.HasValue ? overriddenProductPrice.Value.ToString(CultureInfo.InvariantCulture) : null,
                additionalCharge.ToString(CultureInfo.InvariantCulture),
                includeDiscounts, 
                quantity,
                string.Join(",", customer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
             var cacheTime = _catalogSettings.CacheProductPrices ? 60 : 0;

            //we do not cache price for rental products
            //otherwise, it can cause memory leaks (to store all possible date period combinations)
            if (product.IsRental)
                cacheTime = 0;

            var cachedPrice = _cacheManager.Get(cacheKey, cacheTime, () =>
            {
                var result = new ProductPriceForCaching();

                //initial price
                var price = overriddenProductPrice.HasValue ? overriddenProductPrice.Value : product.Price;

                //tier prices
                var tierPrice = product.GetPreferredTierPrice(customer, _storeContext.CurrentStore.Id, quantity);
                if (tierPrice != null)
                    price = tierPrice.Price;

                //additional charge
                price += additionalCharge;

                //rental products
                if (product.IsRental && rentalStartDate.HasValue && rentalEndDate.HasValue)
                    price *= product.GetRentalPeriods(rentalStartDate.Value, rentalEndDate.Value);

                if (includeDiscounts)
                {
                    //we don't apply discounts to products with price entered by a customer and when discounts are disabled
                    if (!product.CustomerEntersPrice && !_catalogSettings.IgnoreDiscounts)
                    {
                        var allowedDiscounts = GetAllowedDiscounts(product, customer);
                        result.DiscountPrice = allowedDiscounts.GetDiscountPrice(new DiscountPriceRequest(price, quantity));
                        price = result.DiscountPrice.AppliedDiscounts.Any() ? result.DiscountPrice.PriceWithDiscount : price;
                    }
                }

                result.Price = Math.Max(decimal.Zero, price);

                return result;
            });

            discountPrice = cachedPrice.DiscountPrice;

            return cachedPrice.Price;
        }

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public virtual decimal GetUnitPrice(ShoppingCartItem shoppingCartItem, bool includeDiscounts = true)
        {
            DiscountPrice discountPrice;
            return GetUnitPrice(shoppingCartItem, includeDiscounts, out discountPrice);
        }

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <param name="discountPrice">Discount price details</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public virtual decimal GetUnitPrice(ShoppingCartItem shoppingCartItem, bool includeDiscounts, out DiscountPrice discountPrice)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            return GetUnitPrice(shoppingCartItem.Product, shoppingCartItem.Customer, shoppingCartItem.ShoppingCartType,
                shoppingCartItem.Quantity, shoppingCartItem.AttributesXml, shoppingCartItem.CustomerEnteredPrice,
                shoppingCartItem.RentalStartDateUtc, shoppingCartItem.RentalEndDateUtc, includeDiscounts, out discountPrice);
        }

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Product atrributes (XML format)</param>
        /// <param name="customerEnteredPrice">Customer entered price (if specified)</param>
        /// <param name="rentalStartDate">Rental start date (null for not rental products)</param>
        /// <param name="rentalEndDate">Rental end date (null for not rental products)</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <param name="discountPrice">Discount price details</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public virtual decimal GetUnitPrice(Product product, Customer customer,  ShoppingCartType shoppingCartType,
            int quantity, string attributesXml, decimal customerEnteredPrice, 
            DateTime? rentalStartDate, DateTime? rentalEndDate, bool includeDiscounts, out DiscountPrice discountPrice)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            discountPrice = new DiscountPrice();
            decimal finalPrice;

            var combination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
            if (combination != null && combination.OverriddenPrice.HasValue)
            {
                finalPrice = GetFinalPrice(product, customer,
                    combination.OverriddenPrice.Value, decimal.Zero, includeDiscounts, quantity,
                    product.IsRental ? rentalStartDate : null, product.IsRental ? rentalEndDate : null, out discountPrice);
            }
            else
            {
                //get price of a product with price of all attributes
                if (!product.CustomerEntersPrice)
                {
                    //summarize price of all attributes
                    var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
                    var attributesTotalPrice = attributeValues.Sum(attributeValue => GetProductAttributeValuePriceAdjustment(attributeValue));

                    if (_shoppingCartSettings.GroupTierPricesForDistinctShoppingCartItems)
                    {
                        //the same products with distinct product attributes could be stored as distinct "ShoppingCartItem" records
                        //so let's find how many of the current products are in the cart
                        var qty = customer.ShoppingCartItems.Where(x => x.ProductId == product.Id)
                            .Where(x => x.ShoppingCartType == shoppingCartType).Sum(x => x.Quantity);
                        if (qty > 0)
                            quantity = qty;
                    }

                    finalPrice = GetFinalPrice(product, customer,
                        attributesTotalPrice, includeDiscounts, quantity,
                        product.IsRental ? rentalStartDate : null, product.IsRental ? rentalEndDate : null, out discountPrice);
                }
                else
                    finalPrice = customerEnteredPrice;
            }
            
            //rounding
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                finalPrice = RoundingHelper.RoundPrice(finalPrice);

            return finalPrice;
        }

        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart item sub total</returns>
        public virtual decimal GetSubTotal(ShoppingCartItem shoppingCartItem, bool includeDiscounts = true)
        {
            DiscountPrice discountPrice;
            return GetSubTotal(shoppingCartItem, includeDiscounts, out discountPrice);
        }

        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <param name="discountPrice">Discount price details</param>
        /// <returns>Shopping cart item sub total</returns>
        public virtual decimal GetSubTotal(ShoppingCartItem shoppingCartItem, bool includeDiscounts, out DiscountPrice discountPrice)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            //unit price
            var unitPrice = GetUnitPrice(shoppingCartItem, includeDiscounts, out discountPrice);

            //discount
            if (discountPrice.AppliedDiscounts.Any())
            {
                var subTotalWithDiscounts = discountPrice.GetSubTotalWithDiscounts();
                discountPrice.DiscountAmount = (discountPrice.PriceWithDiscount + discountPrice.DiscountAmount) * shoppingCartItem.Quantity - subTotalWithDiscounts;

                return subTotalWithDiscounts;
            }

            return unitPrice * shoppingCartItem.Quantity;
        }

        /// <summary>
        /// Gets the product cost (one item)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Shopping cart item attributes in XML</param>
        /// <returns>Product cost (one item)</returns>
        public virtual decimal GetProductCost(Product product, string attributesXml)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            decimal cost = product.ProductCost;
            var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                switch (attributeValue.AttributeValueType)
                {
                    case AttributeValueType.Simple:
                        {
                            //simple attribute
                            cost += attributeValue.Cost;
                        }
                        break;
                    case AttributeValueType.AssociatedToProduct:
                        {
                            //bundled product
                            var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                            if (associatedProduct != null)
                                cost += associatedProduct.ProductCost * attributeValue.Quantity;
                        }
                        break;
                    default:
                        break;
                }
            }

            return cost;
        }

        /// <summary>
        /// Get a price adjustment of a product attribute value
        /// </summary>
        /// <param name="value">Product attribute value</param>
        /// <returns>Price adjustment</returns>
        public virtual decimal GetProductAttributeValuePriceAdjustment(ProductAttributeValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var adjustment = decimal.Zero;
            switch (value.AttributeValueType)
            {
                case AttributeValueType.Simple:
                    {
                        //simple attribute
                        adjustment = value.PriceAdjustment;
                    }
                    break;
                case AttributeValueType.AssociatedToProduct:
                    {
                        //bundled product
                        var associatedProduct = _productService.GetProductById(value.AssociatedProductId);
                        if (associatedProduct != null)
                        {
                            adjustment = GetFinalPrice(associatedProduct, _workContext.CurrentCustomer, includeDiscounts: true) * value.Quantity;
                        }
                    }
                    break;
                default:
                    break;
            }

            return adjustment;
        }

        #endregion
    }
}
