using System.Collections.Generic;
using System.IO;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.ExportImport
{
    [TestFixture]
    public class ExportManagerTests : ServiceTest
    {
        private ICategoryService _categoryService;
        private IManufacturerService _manufacturerService;
        private IProductAttributeService _productAttributeService;
        private IPictureService _pictureService;
        private INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private IExportManager _exportManager;
        private IStoreService _storeService;
        private ProductEditorSettings _productEditorSettings;
        private IWorkContext _workContext;
        private IVendorService _vendorService;
        private IProductTemplateService _productTemplateService;
        private IShippingService _shippingService;
        private IDateRangeService _dateRangeService;
        private ITaxCategoryService _taxCategoryService;
        private IMeasureService _measureService;
        private CatalogSettings _catalogSettings;

        [SetUp]
        public new void SetUp()
        {
            _storeService = MockRepository.GenerateMock<IStoreService>();
            _categoryService = MockRepository.GenerateMock<ICategoryService>();
            _manufacturerService = MockRepository.GenerateMock<IManufacturerService>();
            _productAttributeService = MockRepository.GenerateMock<IProductAttributeService>();
            _pictureService = MockRepository.GenerateMock<IPictureService>();
            _newsLetterSubscriptionService = MockRepository.GenerateMock<INewsLetterSubscriptionService>();
            _productEditorSettings = new ProductEditorSettings();
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _vendorService = MockRepository.GenerateMock<IVendorService>();
            _productTemplateService = MockRepository.GenerateMock<IProductTemplateService>();
            _shippingService = MockRepository.GenerateMock<IShippingService>();
            _dateRangeService = MockRepository.GenerateMock<IDateRangeService>();
            _taxCategoryService = MockRepository.GenerateMock<ITaxCategoryService>();
            _measureService = MockRepository.GenerateMock<IMeasureService>();
            _catalogSettings = new CatalogSettings();

            _exportManager = new ExportManager(_categoryService,
                _manufacturerService, _productAttributeService, 
                _pictureService, _newsLetterSubscriptionService,
                _storeService, _workContext, _productEditorSettings, 
                _vendorService, _productTemplateService, _shippingService,
                _dateRangeService, _taxCategoryService, _measureService, _catalogSettings);
        }

        [Test]
        public void Can_export_manufacturers_to_xml()
        {
            var manufacturers = new List<Manufacturer>()
            {
                TestHelper.GetManufacturer()
            };

            //TODO uncomment
            //string result = _exportManager.ExportManufacturersToXml(manufacturers);
            //TODO test it
            //String.IsNullOrEmpty(result).ShouldBeFalse();
        }

        [Test]
        public void Can_export_orders_xlsx()
        {
            var orders = new List<Order>
            {
                TestHelper.GetOrder()
            };

            var fileName = Path.GetTempFileName();
            //TODO uncomment
            //_exportManager.ExportOrdersToXlsx(fileName, orders);
        }
       
    }
}
