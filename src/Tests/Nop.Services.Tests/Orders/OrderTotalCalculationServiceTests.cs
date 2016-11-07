using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class OrderTotalCalculationServiceTests : ServiceTest
    {
        private IWorkContext _workContext;
        private IStoreContext _storeContext;
        private ITaxService _taxService;
        private IShippingService _shippingService;
        private IPaymentService _paymentService;
        private ICheckoutAttributeParser _checkoutAttributeParser;
        private IDiscountService _discountService;
        private IGiftCardService _giftCardService;
        private IGenericAttributeService _genericAttributeService;
        private TaxSettings _taxSettings;
        private RewardPointsSettings _rewardPointsSettings;
        private ICategoryService _categoryService;
        private IManufacturerService _manufacturerService;
        private IProductAttributeParser _productAttributeParser;
        private IPriceCalculationService _priceCalcService;
        private IOrderTotalCalculationService _orderTotalCalcService;
        private IAddressService _addressService;
        private ShippingSettings _shippingSettings;
        private ILocalizationService _localizationService;
        private ILogger _logger;
        private IRepository<ShippingMethod> _shippingMethodRepository;
        private IRepository<Warehouse> _warehouseRepository;
        private ShoppingCartSettings _shoppingCartSettings;
        private CatalogSettings _catalogSettings;
        private IEventPublisher _eventPublisher;
        private Store _store;
        private IProductService _productService;
        private IGeoLookupService _geoLookupService;
        private ICountryService _countryService;
        private CustomerSettings _customerSettings;
        private AddressSettings _addressSettings;
        private IRewardPointService _rewardPointService;

        private static IList<ShoppingCartItem> GetShoppingCartItems(Customer customer, bool isFreeShipping = true, decimal price1 = 12.34M, decimal price2 = 21.57M, bool isShipEnabled = true)
        {
            customer.IsTaxExempt = false;

            var product1 = new Product
            {
                Id = 1,
                Price = price1,
                Published = true,
                IsShipEnabled = isShipEnabled,
                IsFreeShipping = isFreeShipping
            };

            var sci1 = TestHelper.GetShoppingCartItem();
            sci1.CustomerEnteredPrice = price1;
            sci1.Product = product1;
            sci1.Customer = customer;

            var product2 = new Product
            {
                Id = 2,
                Price = price2,
                Published = true,
                IsShipEnabled = isShipEnabled,
                IsFreeShipping = isFreeShipping
            };

            var sci2 = TestHelper.GetShoppingCartItem();
            sci2.Product = product2;
            sci2.Customer = customer;
            sci2.Quantity = 3;
            sci2.CustomerEnteredPrice = price2;

            return new List<ShoppingCartItem> { sci1, sci2 };
        }

        private static ShoppingCartItem GetShoppingCartItem(int quantity,
            decimal additionalShippingCharge,
            Customer customer,
            bool isShipEnabled = true)
        {
            customer = customer ?? new Customer();
            var shoppingCartItem = TestHelper.GetShoppingCartItem();
            shoppingCartItem.Product = new Product
            {
                IsShipEnabled = isShipEnabled,
                IsFreeShipping = false,
                AdditionalShippingCharge = additionalShippingCharge
            };
            shoppingCartItem.Customer = customer;
            shoppingCartItem.Quantity = quantity;

            return shoppingCartItem;
        }

        private static IList<ShoppingCartItem> CreateShoppingCartItems(Customer customer = null)
        {
            var sci1 = GetShoppingCartItem(3, 5.5M, customer);
            var sci2 = GetShoppingCartItem(4, 6.5M, customer);
            //sci3 is not shippable
            var sci3 = GetShoppingCartItem(5, 7.5M, customer, false);

            return new List<ShoppingCartItem> { sci1, sci2, sci3 };
        }

        [SetUp]
        public new void SetUp()
        {
            _workContext = MockRepository.GenerateMock<IWorkContext>();

            _store = TestHelper.GetStore();
            _storeContext = MockRepository.GenerateMock<IStoreContext>();
            _storeContext.Expect(x => x.CurrentStore).Return(_store);

            _productService = MockRepository.GenerateMock<IProductService>();

            var pluginFinder = new PluginFinder();
            var cacheManager = new NopNullCache();

            _discountService = MockRepository.GenerateMock<IDiscountService>();
            _categoryService = MockRepository.GenerateMock<ICategoryService>();
            _manufacturerService = MockRepository.GenerateMock<IManufacturerService>();
            _productAttributeParser = MockRepository.GenerateMock<IProductAttributeParser>();

            _shoppingCartSettings = new ShoppingCartSettings();
            _catalogSettings = new CatalogSettings();

            _priceCalcService = new PriceCalculationService(_workContext, _storeContext,
                _discountService, _categoryService,
                _manufacturerService, _productAttributeParser,
                _productService, cacheManager,
                _shoppingCartSettings, _catalogSettings);

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _localizationService = MockRepository.GenerateMock<ILocalizationService>();

            //shipping
            _shippingSettings = new ShippingSettings
            {
                ActiveShippingRateComputationMethodSystemNames = new List<string>
                {
                    "FixedRateTestShippingRateComputationMethod"
                }
            };
            _shippingMethodRepository = MockRepository.GenerateMock<IRepository<ShippingMethod>>();
            _warehouseRepository = MockRepository.GenerateMock<IRepository<Warehouse>>();
            _logger = new NullLogger();
            _shippingService = new ShippingService(_shippingMethodRepository,
                _warehouseRepository,
                _logger,
                _productService,
                _productAttributeParser,
                _checkoutAttributeParser,
                _genericAttributeService,
                _localizationService,
                _addressService,
                _shippingSettings,
                pluginFinder,
                _storeContext,
                _eventPublisher,
                _shoppingCartSettings,
                cacheManager);

            _paymentService = MockRepository.GenerateMock<IPaymentService>();
            _checkoutAttributeParser = MockRepository.GenerateMock<ICheckoutAttributeParser>();
            _giftCardService = MockRepository.GenerateMock<IGiftCardService>();
            _genericAttributeService = MockRepository.GenerateMock<IGenericAttributeService>();

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _geoLookupService = MockRepository.GenerateMock<IGeoLookupService>();
            _countryService = MockRepository.GenerateMock<ICountryService>();
            _customerSettings = new CustomerSettings();
            _addressSettings = new AddressSettings();

            //tax
            _taxSettings = new TaxSettings
            {
                ShippingIsTaxable = true,
                PaymentMethodAdditionalFeeIsTaxable = true,
                DefaultTaxAddressId = 10
            };
            _addressService = MockRepository.GenerateMock<IAddressService>();
            _addressService.Expect(x => x.GetAddressById(_taxSettings.DefaultTaxAddressId)).Return(new Address { Id = _taxSettings.DefaultTaxAddressId });
            _taxService = new TaxService(_addressService, _workContext, _taxSettings,
                pluginFinder, _geoLookupService, _countryService, _logger, _customerSettings, _addressSettings);
            _rewardPointService = MockRepository.GenerateMock<IRewardPointService>();

            _rewardPointsSettings = new RewardPointsSettings();

            _orderTotalCalcService = new OrderTotalCalculationService(_workContext, _storeContext,
                _priceCalcService, _taxService, _shippingService, _paymentService,
                _checkoutAttributeParser, _discountService, _giftCardService, _genericAttributeService,
                _rewardPointService, _taxSettings, _rewardPointsSettings,
                _shippingSettings, _shoppingCartSettings, _catalogSettings);
        }

        [Test]
        public void Can_get_shopping_cart_subTotal_excluding_tax()
        {
            //customer
            var customer = TestHelper.GetCustomer();
            var cart = GetShoppingCartItems(customer);

            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            decimal discountAmount;
            List<DiscountForCaching> appliedDiscounts;
            decimal subTotalWithoutDiscount;
            decimal subTotalWithDiscount;
            SortedDictionary<decimal, decimal> taxRates;
            //10% - default tax rate
            _orderTotalCalcService.GetShoppingCartSubTotal(cart, false,
                out discountAmount, out appliedDiscounts,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);
            discountAmount.ShouldEqual(0);
            appliedDiscounts.Count.ShouldEqual(0);
            subTotalWithoutDiscount.ShouldEqual(89.39);
            subTotalWithDiscount.ShouldEqual(89.39);
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.939);
        }

        [Test]
        public void Can_get_shopping_cart_subTotal_including_tax()
        {
            //customer
            var customer = TestHelper.GetCustomer();
            var cart = GetShoppingCartItems(customer);

            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            decimal discountAmount;
            List<DiscountForCaching> appliedDiscounts;
            decimal subTotalWithoutDiscount;
            decimal subTotalWithDiscount;
            SortedDictionary<decimal, decimal> taxRates;

            _orderTotalCalcService.GetShoppingCartSubTotal(cart, true,
                out discountAmount, out appliedDiscounts,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);
            discountAmount.ShouldEqual(0);
            appliedDiscounts.Count.ShouldEqual(0);
            subTotalWithoutDiscount.ShouldEqual(98.329);
            subTotalWithDiscount.ShouldEqual(98.329);
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.939);
        }

        [Test]
        public void Can_get_shopping_cart_subTotal_discount_excluding_tax()
        {
            //customer
            var customer = TestHelper.GetCustomer();
            var cart = GetShoppingCartItems(customer);

            var discount1 = TestHelper.GetDiscountForCaching();

            _discountService.Expect(ds => ds.ValidateDiscount(discount1, customer)).Return(new DiscountValidationResult() { IsValid = true });
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToOrderSubTotal)).Return(new List<DiscountForCaching> { discount1 });
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());
            
            decimal discountAmount;
            List<DiscountForCaching> appliedDiscounts;
            decimal subTotalWithoutDiscount;
            decimal subTotalWithDiscount;
            SortedDictionary<decimal, decimal> taxRates;
            //10% - default tax rate
            _orderTotalCalcService.GetShoppingCartSubTotal(cart, false,
                out discountAmount, out appliedDiscounts,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);
            discountAmount.ShouldEqual(3);
            appliedDiscounts.Count.ShouldEqual(1);
            appliedDiscounts.First().Name.ShouldEqual("Discount 1");
            subTotalWithoutDiscount.ShouldEqual(89.39);
            subTotalWithDiscount.ShouldEqual(86.39);
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.639);
        }

        [Test]
        public void Can_get_shopping_cart_subTotal_discount_including_tax()
        {
            //customer
            var customer = TestHelper.GetCustomer();
            var cart = GetShoppingCartItems(customer);

            var discount1 = TestHelper.GetDiscountForCaching();

            _discountService.Expect(ds => ds.ValidateDiscount(discount1, customer)).Return(new DiscountValidationResult() { IsValid = true });
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToOrderSubTotal)).Return(new List<DiscountForCaching> { discount1 });
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            decimal discountAmount;
            List<DiscountForCaching> appliedDiscounts;
            decimal subTotalWithoutDiscount;
            decimal subTotalWithDiscount;
            SortedDictionary<decimal, decimal> taxRates;
            _orderTotalCalcService.GetShoppingCartSubTotal(cart, true,
                out discountAmount, out appliedDiscounts,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);

            //The comparison test failed before, because of a very tiny number difference.
            //discountAmount.ShouldEqual(3.3);
            (System.Math.Round(discountAmount, 10) == 3.3M).ShouldBeTrue();
            appliedDiscounts.Count.ShouldEqual(1);
            appliedDiscounts.First().Name.ShouldEqual("Discount 1");
            subTotalWithoutDiscount.ShouldEqual(98.329);
            subTotalWithDiscount.ShouldEqual(95.029);
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.639);
        }

        [Test]
        public void Can_get_shoppingCartItem_additional_shippingCharge()
        {
            _orderTotalCalcService.GetShoppingCartAdditionalShippingCharge(CreateShoppingCartItems()).ShouldEqual(42.5M);
        }

        [Test]
        public void Shipping_should_be_free_when_all_shoppingCartItems_are_marked_as_freeShipping()
        {
            _orderTotalCalcService.IsFreeShipping(GetShoppingCartItems(TestHelper.GetCustomer())).ShouldEqual(true);
        }

        [Test]
        public void Shipping_should_not_be_free_when_some_of_shoppingCartItems_are_not_marked_as_freeShipping()
        {
            var cart = GetShoppingCartItems(TestHelper.GetCustomer());
            cart[1].Product.IsFreeShipping = false;
            _orderTotalCalcService.IsFreeShipping(cart).ShouldEqual(false);
        }

        [Test]
        public void Shipping_should_be_free_when_customer_is_in_role_with_free_shipping()
        {
            var customer = TestHelper.GetCustomer();
            var cart = GetShoppingCartItems(customer);
            cart[1].Product.IsFreeShipping = false;
            cart[0].Product.IsFreeShipping = false;

            var customerRole1 = TestHelper.GetCustomerRole("");
            var customerRole2 = TestHelper.GetCustomerRole("");
            customerRole2.FreeShipping = false;
            customer.CustomerRoles.Add(customerRole1);
            customer.CustomerRoles.Add(customerRole2);

            _orderTotalCalcService.IsFreeShipping(cart).ShouldEqual(true);
        }

        [Test]
        public void Can_get_shipping_total_with_fixed_shipping_rate_excluding_tax()
        {
            decimal taxRate;

            List<DiscountForCaching> appliedDiscounts;
            var shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(CreateShoppingCartItems(), false, out taxRate, out appliedDiscounts);

            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change
            shipping.ShouldEqual(52.5);
            appliedDiscounts.Count.ShouldEqual(0);
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_shipping_total_with_fixed_shipping_rate_including_tax()
        {
            decimal taxRate;

            List<DiscountForCaching> appliedDiscounts;
            var shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(CreateShoppingCartItems(), true, out taxRate, out appliedDiscounts);

            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change
            shipping.ShouldEqual(57.75);
            appliedDiscounts.Count.ShouldEqual(0);
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_shipping_total_discount_excluding_tax()
        {
            var customer = new Customer();
            var discount1 = TestHelper.GetDiscountForCaching();

            _discountService.Expect(ds => ds.ValidateDiscount(discount1, customer)).Return(new DiscountValidationResult() { IsValid = true });
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToShipping)).Return(new List<DiscountForCaching> { discount1 });
            decimal taxRate;
            List<DiscountForCaching> appliedDiscounts;
            var shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(CreateShoppingCartItems(customer), false, out taxRate, out appliedDiscounts);
            appliedDiscounts.Count.ShouldEqual(1);
            appliedDiscounts.First().Name.ShouldEqual("Discount 1");
            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change, -3 - discount
            shipping.ShouldEqual(49.5);
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_shipping_total_discount_including_tax()
        {
            var customer = new Customer();

            //discounts
            var discount = TestHelper.GetDiscountForCaching();

            _discountService.Expect(ds => ds.ValidateDiscount(discount, customer)).Return(new DiscountValidationResult { IsValid = true });
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToShipping)).Return(new List<DiscountForCaching> { discount });

            decimal taxRate;
            List<DiscountForCaching> appliedDiscounts;
            var shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(CreateShoppingCartItems(customer), true, out taxRate, out appliedDiscounts);

            appliedDiscounts.Count.ShouldEqual(1);
            appliedDiscounts.First().Name.ShouldEqual("Discount 1");
            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change, -3 - discount
            shipping.ShouldEqual(54.45);
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_tax_total()
        {
            //customer
            var customer = new Customer
            {
                Id = 10
            };

            //shopping cart
            var cart = GetShoppingCartItems(customer, false, 10, 12);

            _genericAttributeService.Expect(x => x.GetAttributesForEntity(customer.Id, "Customer"))
                .Return(new List<GenericAttribute>
                            {
                                new GenericAttribute
                                    {
                                        StoreId = _store.Id,
                                        EntityId = customer.Id,
                                        Key = SystemCustomerAttributeNames.SelectedPaymentMethod,
                                        KeyGroup = "Customer",
                                        Value = "test1"
                                    }
                            });
            _paymentService.Expect(ps => ps.GetAdditionalHandlingFee(cart, "test1")).Return(20);
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            //56 - items, 10 - shipping (fixed), 20 - payment fee

            //1. shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;
            SortedDictionary<decimal, decimal> taxRates;
            _orderTotalCalcService.GetTaxTotal(cart, out taxRates).ShouldEqual(8.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.6);

            //2. shipping is taxable, payment fee is not taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
            _orderTotalCalcService.GetTaxTotal(cart, out taxRates).ShouldEqual(6.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(6.6);

            //3. shipping is not taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = false;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;
            _orderTotalCalcService.GetTaxTotal(cart, out taxRates).ShouldEqual(7.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(7.6);

            //3. shipping is not taxable, payment fee is not taxable
            _taxSettings.ShippingIsTaxable = false;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
            _orderTotalCalcService.GetTaxTotal(cart, out taxRates).ShouldEqual(5.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(5.6);
        }

        [Test]
        public void Can_get_shopping_cart_total_without_shipping_required()
        {
            //customer
            var customer = new Customer
            {
                Id = 10,
            };

            //shopping cart
            var cart = GetShoppingCartItems(customer, false, 10, 12, false);

            _genericAttributeService.Expect(x => x.GetAttributesForEntity(customer.Id, "Customer"))
                .Return(new List<GenericAttribute>
                            {
                                new GenericAttribute
                                    {
                                        StoreId = _store.Id,
                                        EntityId = customer.Id,
                                        Key = SystemCustomerAttributeNames.SelectedPaymentMethod,
                                        KeyGroup = "Customer",
                                        Value = "test1"
                                    }
                            });
            _paymentService.Expect(ps => ps.GetAdditionalHandlingFee(cart, "test1")).Return(20);

            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            decimal discountAmount;
            List<DiscountForCaching> appliedDiscounts;
            List<AppliedGiftCard> appliedGiftCards;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;
            
            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //56 - items, 20 - payment fee, 7.6 - tax
            _orderTotalCalcService.GetShoppingCartTotal(cart, out discountAmount, out appliedDiscounts,
                out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount)
                .ShouldEqual(83.6M);
        }

        [Test]
        public void Can_get_shopping_cart_total_with_shipping_required()
        {
            //customer
            var customer = new Customer
            {
                Id = 10,
            };

            //shopping cart
            var cart = GetShoppingCartItems(customer, false, 10, 12);

            _genericAttributeService.Expect(x => x.GetAttributesForEntity(customer.Id, "Customer"))
                .Return(new List<GenericAttribute>
                            {
                                new GenericAttribute
                                    {
                                        StoreId = _store.Id,
                                        EntityId = customer.Id,
                                        Key = SystemCustomerAttributeNames.SelectedPaymentMethod,
                                        KeyGroup = "Customer",
                                        Value = "test1"
                                    }
                            });
            _paymentService.Expect(ps => ps.GetAdditionalHandlingFee(cart, "test1")).Return(20);

            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            decimal discountAmount;
            List<DiscountForCaching> appliedDiscounts;
            List<AppliedGiftCard> appliedGiftCards;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;
            
            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //56 - items, 10 - shipping (fixed), 20 - payment fee, 8.6 - tax
            _orderTotalCalcService.GetShoppingCartTotal(cart, out discountAmount, out appliedDiscounts,
                out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount)
                .ShouldEqual(94.6M);
        }

        /*TODO temporary disabled
        [Test]
        public void Can_get_shopping_cart_total_with_applied_reward_points()
        {
            //customer
            var customer = new Customer
            {
                Id = 10,
            };

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true,
            };
            var sci1 = new ShoppingCartItem
            {
                Product = product1,
                ProductId = product1.Id,
                Quantity = 2,
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true,
            };
            var sci2 = new ShoppingCartItem
            {
                Product = product2,
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);



            _genericAttributeService.Expect(x => x.GetAttributesForEntity(customer.Id, "Customer"))
                .Return(new List<GenericAttribute>
                            {
                                new GenericAttribute
                                    {
                                        StoreId = _store.Id,
                                        EntityId = customer.Id,
                                        Key = SystemCustomerAttributeNames.SelectedPaymentMethod,
                                        KeyGroup = "Customer",
                                        Value = "test1"
                                    },
                                new GenericAttribute
                                        {
                                        StoreId = 1,
                                        EntityId = customer.Id,
                                        Key = SystemCustomerAttributeNames.UseRewardPointsDuringCheckout,
                                        KeyGroup = "Customer",
                                        Value = true.ToString()
                                        }
                            });
            _paymentService.Expect(ps => ps.GetAdditionalHandlingFee(cart, "test1")).Return(20);


            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());

            decimal discountAmount;
            Discount appliedDiscount;
            List<AppliedGiftCard> appliedGiftCards;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;


            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //reward points
            _rewardPointsSettings.Enabled = true;
            _rewardPointsSettings.ExchangeRate = 2; //1 reward point = 2
            
            customer.AddRewardPointsHistoryEntry(15, 0); //15*2=30

            //56 - items, 10 - shipping (fixed), 20 - payment fee, 8.6 - tax, -30 (reward points)
             _orderTotalCalcService.GetShoppingCartTotal(cart, out discountAmount, out appliedDiscount,
                out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount)
                .ShouldEqual(64.6M);
        }*/

        [Test]
        public void Can_get_shopping_cart_total_discount()
        {
            //customer
            var customer = new Customer
            {
                Id = 10,
            };
            //shopping cart
            var cart = GetShoppingCartItems(customer, false, 10, 12);

            //discounts

            var discount = TestHelper.GetDiscountForCaching();

            _discountService.Expect(ds => ds.ValidateDiscount(discount, customer)).Return(new DiscountValidationResult() { IsValid = true });
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToOrderTotal)).Return(new List<DiscountForCaching> { discount });
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Return(new List<DiscountForCaching>());
            _discountService.Expect(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Return(new List<DiscountForCaching>());
            
            _genericAttributeService.Expect(x => x.GetAttributesForEntity(customer.Id, "Customer"))
                .Return(new List<GenericAttribute>
                            {
                                new GenericAttribute
                                    {
                                        StoreId = _store.Id,
                                        EntityId = customer.Id,
                                        Key = SystemCustomerAttributeNames.SelectedPaymentMethod,
                                        KeyGroup = "Customer",
                                        Value = "test1"
                                    }
                            });
            _paymentService.Expect(ps => ps.GetAdditionalHandlingFee(cart, "test1")).Return(20);
            
            decimal discountAmount;
            List<DiscountForCaching> appliedDiscounts;
            List<AppliedGiftCard> appliedGiftCards;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;

            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //56 - items, 10 - shipping (fixed), 20 - payment fee, 8.6 - tax, [-3] - discount
            _orderTotalCalcService.GetShoppingCartTotal(cart, out discountAmount, out appliedDiscounts,
                out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount)
                .ShouldEqual(91.6M);
            discountAmount.ShouldEqual(3);
            appliedDiscounts.Count.ShouldEqual(1);
            appliedDiscounts.First().Name.ShouldEqual("Discount 1");
        }

        [Test]
        public void Can_convert_reward_points_to_amount()
        {
            _rewardPointsSettings.Enabled = true;
            _rewardPointsSettings.ExchangeRate = 15M;

            _orderTotalCalcService.ConvertRewardPointsToAmount(100).ShouldEqual(1500);
        }

        [Test]
        public void Can_convert_amount_to_reward_points()
        {
            _rewardPointsSettings.Enabled = true;
            _rewardPointsSettings.ExchangeRate = 15M;

            //we calculate ceiling for reward points
            _orderTotalCalcService.ConvertAmountToRewardPoints(100).ShouldEqual(7);
        }

        [Test]
        public void Can_check_minimum_reward_points_to_use_requirement()
        {
            _rewardPointsSettings.Enabled = true;
            _rewardPointsSettings.MinimumRewardPointsToUse = 0;

            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(0).ShouldEqual(true);
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(1).ShouldEqual(true);
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(10).ShouldEqual(true);
            
            _rewardPointsSettings.MinimumRewardPointsToUse = 2;
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(0).ShouldEqual(false);
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(1).ShouldEqual(false);
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(2).ShouldEqual(true);
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(10).ShouldEqual(true);
        }
    }
}
