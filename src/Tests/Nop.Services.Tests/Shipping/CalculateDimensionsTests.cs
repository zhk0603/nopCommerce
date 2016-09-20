using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Shipping
{
    [TestFixture]
    public class CalculateDimensionsTests : ServiceTest
    {
        private IRepository<ShippingMethod> _shippingMethodRepository;
        private IRepository<DeliveryDate> _deliveryDateRepository;
        private IRepository<Warehouse> _warehouseRepository;
        private ILogger _logger;
        private IProductAttributeParser _productAttributeParser;
        private ICheckoutAttributeParser _checkoutAttributeParser;
        private ShippingSettings _shippingSettings;
        private IEventPublisher _eventPublisher;
        private ILocalizationService _localizationService;
        private IAddressService _addressService;
        private IGenericAttributeService _genericAttributeService;
        private IShippingService _shippingService;
        private ShoppingCartSettings _shoppingCartSettings;
        private IProductService _productService;
        private Store _store;
        private IStoreContext _storeContext;

        [SetUp]
        public new void SetUp()
        {
            _shippingSettings = new ShippingSettings { UseCubeRootMethod = true };

            _shippingMethodRepository = MockRepository.GenerateMock<IRepository<ShippingMethod>>();
            _deliveryDateRepository = MockRepository.GenerateMock<IRepository<DeliveryDate>>();
            _warehouseRepository = MockRepository.GenerateMock<IRepository<Warehouse>>();
            _logger = new NullLogger();
            _productAttributeParser = MockRepository.GenerateMock<IProductAttributeParser>();
            _checkoutAttributeParser = MockRepository.GenerateMock<ICheckoutAttributeParser>();

            var cacheManager = new NopNullCache();

            var pluginFinder = new PluginFinder();
            _productService = MockRepository.GenerateMock<IProductService>();

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _addressService = MockRepository.GenerateMock<IAddressService>();
            _genericAttributeService = MockRepository.GenerateMock<IGenericAttributeService>();

            _store = new Store { Id = 1 };
            _storeContext = MockRepository.GenerateMock<IStoreContext>();
            _storeContext.Expect(x => x.CurrentStore).Return(_store);

            _shoppingCartSettings = new ShoppingCartSettings();
            _shippingService = new ShippingService(_shippingMethodRepository,
                _deliveryDateRepository,
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
        }

        private static IList<GetShippingOptionRequest.PackageItem> GetPackageItem(params PackageItemData[] packageItemDatas)
        {
            return packageItemDatas.Select(packageItemData => new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = packageItemData.Quantity,
                    Product = new Product
                    {
                        Length = packageItemData.Length,
                        Width = packageItemData.Width,
                        Height = packageItemData.Height
                    }
                })).ToList();
        }

        private static IList<GetShippingOptionRequest.PackageItem> GetPackageItem(PackageItemData packageItemData, int copyCount)
        {
            var items = new List<GetShippingOptionRequest.PackageItem>();

            for (var i = 0; i < copyCount; i++)
            {
                items.Add(new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = packageItemData.Quantity,
                    Product = new Product
                    {
                        Length = packageItemData.Length,
                        Width = packageItemData.Width,
                        Height = packageItemData.Height
                    }
                }));
            }

            return items;
        }

        private void CheckDimensions(IList<GetShippingOptionRequest.PackageItem> packageItems, decimal length, decimal width, decimal height)
        {
            decimal l, w, h;
            _shippingService.GetDimensions(packageItems, out w, out l, out h);
            Math.Round(l, 2).ShouldEqual(length);
            Math.Round(w, 2).ShouldEqual(width);
            Math.Round(h, 2).ShouldEqual(height);
        }

        private void CheckDimensions(IList<GetShippingOptionRequest.PackageItem> packageItems, decimal size)
        {
            CheckDimensions(packageItems, size, size, size);
        }

        [Test]
        public void ShouldReturnZeroWithAllZeroDimensions()
        {
            CheckDimensions(GetPackageItem(new PackageItemData()), 0);
            CheckDimensions(GetPackageItem(new PackageItemData(2)), 0);
        }
        
        [Test]
        public void CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod()
        {
            CheckDimensions(GetPackageItem(new PackageItemData(2, 3, 4)), 2, 3, 4);
        }

        [Test]
        public void CanCalculateWithSingleItemAndQty2()
        {
            CheckDimensions(GetPackageItem(new PackageItemData(2, 4, 4, 2)), 4);
        }

        [Test]
        public void CanCalculateWithCubicItemAndMultipleQty()
        {
            CheckDimensions(GetPackageItem(new PackageItemData(3, 2)), 2.88M);
        }

        [Test]
        public void CanCalculateWithMultipleItems1()
        {
            //preserve max width
            CheckDimensions(GetPackageItem(new PackageItemData(3, 2), new PackageItemData(3, 5, 2)), 3.78M, 5, 3.78M);
        }

        [Test]
        public void CanCalculateWithMultipleItems2()
        {
            //take 8 cubes of 1x1x1 which is "packed" as 2x2x2 
           CheckDimensions(GetPackageItem(new PackageItemData(1M), 8), 2);
        }
    }

    internal class PackageItemData
    {
        public int Quantity { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }

        public PackageItemData(decimal size = 0)
        {
            Quantity = 1;
            Length = Width = Height = size;
        }

        public PackageItemData(int quantity, decimal size = 0)
        {
            Quantity = quantity;
            Length = Width = Height = size;
        }

        public PackageItemData(decimal length, decimal width, decimal height, int quantity = 1)
        {
            Quantity = quantity;
            Length = length;
            Width = width;
            Height = height;
        }
    }
}
