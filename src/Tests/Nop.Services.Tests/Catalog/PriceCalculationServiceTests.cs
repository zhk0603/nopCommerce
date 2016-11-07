using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class PriceCalculationServiceTests : ServiceTest
    {
        private IWorkContext _workContext;
        private IStoreContext _storeContext;
        private IDiscountService _discountService;
        private ICategoryService _categoryService;
        private IManufacturerService _manufacturerService;
        private IProductAttributeParser _productAttributeParser;
        private IProductService _productService;
        private IPriceCalculationService _priceCalcService;
        private ShoppingCartSettings _shoppingCartSettings;
        private CatalogSettings _catalogSettings;
        private ICacheManager _cacheManager;

        private Store _store;

        [SetUp]
        public new void SetUp()
        {
            _workContext = null;

            _store = new Store { Id = 1 };
            _storeContext = MockRepository.GenerateMock<IStoreContext>();
            _storeContext.Expect(x => x.CurrentStore).Return(_store);

            _discountService = MockRepository.GenerateMock<IDiscountService>();
            _categoryService = MockRepository.GenerateMock<ICategoryService>();
            _manufacturerService = MockRepository.GenerateMock<IManufacturerService>();
            _productService = MockRepository.GenerateMock<IProductService>();
            
            _productAttributeParser = MockRepository.GenerateMock<IProductAttributeParser>();

            _shoppingCartSettings = new ShoppingCartSettings();
            _catalogSettings = new CatalogSettings();

            _cacheManager = new NopNullCache();

            _priceCalcService = new PriceCalculationService(_workContext,
                _storeContext, 
                _discountService,
                _categoryService,
                _manufacturerService,
                _productAttributeParser,
                _productService,
                _cacheManager,
                _shoppingCartSettings, 
                _catalogSettings);
        }

        [Test]
        public void Can_get_final_product_price()
        {
            var product = TestHelper.GetProduct();

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 0, false, 1).ShouldEqual(21.1M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).ShouldEqual(21.1M);
        }

        [Test]
        public void Can_get_final_product_price_with_tier_prices()
        {
            var product = TestHelper.GetProduct();

            //add tier prices
            product.AddTierPrices(10, 2);
            product.AddTierPrices(8, 5);

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 0, false, 1).ShouldEqual(21.1M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).ShouldEqual(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 3).ShouldEqual(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 5).ShouldEqual(8);
        }

        [Test]
        public void Can_get_final_product_price_with_tier_prices_by_customerRole()
        {
            var product = TestHelper.GetProduct();

            //customer roles
            var customerRole1 = TestHelper.GetCustomerRole("Some role 1");
            var customerRole2 = TestHelper.GetCustomerRole("Some role 2");

            //add tier prices
            product.AddTierPrices(10, 2, customerRole1);
            product.AddTierPrices(9, 2, customerRole2);
            product.AddTierPrices(8, 5, customerRole1);
            product.AddTierPrices(5, 10, customerRole2);
            
            //customer
            var customer = new Customer();
            customer.CustomerRoles.Add(customerRole1);

            _priceCalcService.GetFinalPrice(product, customer, 0, false, 1).ShouldEqual(21.1M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).ShouldEqual(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 3).ShouldEqual(10);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 5).ShouldEqual(8);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 10).ShouldEqual(8);
        }

        [Test]
        public void Can_get_final_product_price_with_additionalFee()
        {
            var product = TestHelper.GetProduct();

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 5, false, 1).ShouldEqual(26.1M);
        }

        [Test]
        public void Can_get_final_product_price_with_discount()
        {
            var product = TestHelper.GetProduct();
            product.CustomerEntersPrice = false;

            //customer
            var customer = new Customer();
            
            //discounts
            var discount = TestHelper.GetDiscount();
            discount.UsePercentage = false;
            discount.DiscountType = DiscountType.AssignedToSkus;

            discount.AppliedToProducts.Add(product);
            product.AppliedDiscounts.Add(discount);

            _discountService.Expect(ds => ds.ValidateDiscount(discount, customer)).Return(new DiscountValidationResult {IsValid = true});
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            _priceCalcService.GetFinalPrice(product, customer).ShouldEqual(19M);
        }

        [Test]
        public void Can_get_final_product_price_with_special_price()
        {
            var product = TestHelper.GetProduct();
            product.SpecialPriceStartDateTimeUtc = DateTime.UtcNow.AddDays(-1);
            product.SpecialPriceEndDateTimeUtc = DateTime.UtcNow.AddDays(1);

            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            //customer
            var customer = new Customer();
            //valid dates
            _priceCalcService.GetFinalPrice(product, customer, 0, true, 1).ShouldEqual(32.1M);
            
            //invalid date
            product.SpecialPriceStartDateTimeUtc = DateTime.UtcNow.AddDays(1);
            _priceCalcService.GetFinalPrice(product, customer, 0, true, 1).ShouldEqual(21.1M);

            //no dates
            product.SpecialPriceStartDateTimeUtc = null;
            product.SpecialPriceEndDateTimeUtc = null;
            _priceCalcService.GetFinalPrice(product, customer, 0, true, 1).ShouldEqual(32.1M);
        }

        [Test]
        public void Can_get_shopping_cart_item_unitPrice()
        {
            //customer
            var customer = new Customer();

            //shopping cart
            var product = TestHelper.GetProduct();
            product.CustomerEntersPrice = false;
            var sci = TestHelper.GetShoppingCartItem();
            sci.Product = product;
            sci.Customer = customer;

            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            _priceCalcService.GetUnitPrice(sci).ShouldEqual(21.1M);

        }

        [Test]
        public void Can_get_shopping_cart_item_subTotal()
        {
            //customer
            var customer = new Customer();

            //shopping cart
            var product = TestHelper.GetProduct();
            product.CustomerEntersPrice = false;

            var sci = TestHelper.GetShoppingCartItem();
            sci.Product = product;
            sci.Customer = customer;

            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            _priceCalcService.GetSubTotal(sci).ShouldEqual(42.2);
        }

        [Test]
        [TestCase(12.00009, 12.00)]
        [TestCase(12.119, 12.12)]
        [TestCase(12.115, 12.12)]
        [TestCase(12.114, 12.11)]        
        public void Test_GetUnitPrice_WhenRoundPricesDuringCalculationIsTrue_PriceMustBeRounded(decimal inputPrice, decimal expectedPrice)
        {
            // arrange
            var shoppingCartItem = CreateTestShopCartItem(inputPrice);

            // act
            _shoppingCartSettings.RoundPricesDuringCalculation = true;
            decimal resultPrice = _priceCalcService.GetUnitPrice(shoppingCartItem);

            // assert
            resultPrice.ShouldEqual(expectedPrice);
        }

        [Test]
        [TestCase(12.00009, 12.00009)]
        [TestCase(12.119, 12.119)]
        [TestCase(12.115, 12.115)]
        [TestCase(12.114, 12.114)]
        public void Test_GetUnitPrice_WhenNotRoundPricesDuringCalculationIsFalse_PriceMustNotBeRounded(decimal inputPrice, decimal expectedPrice)
        {
            // arrange            
            var shoppingCartItem = CreateTestShopCartItem(inputPrice);

            // act
            _shoppingCartSettings.RoundPricesDuringCalculation = false;
            decimal resultPrice = _priceCalcService.GetUnitPrice(shoppingCartItem);

            // assert
            resultPrice.ShouldEqual(expectedPrice);
        }

        private ShoppingCartItem CreateTestShopCartItem(decimal productPrice, int quantity = 1)
        {
            //customer
            var customer = new Customer();

            var product = TestHelper.GetProduct();
            product.CustomerEntersPrice = false;
            product.Price = productPrice;

            var shoppingCartItem = TestHelper.GetShoppingCartItem();
            shoppingCartItem.Quantity = quantity;
            shoppingCartItem.Product = product;
            shoppingCartItem.Customer = customer;

            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            return shoppingCartItem;
        }
    }
}
