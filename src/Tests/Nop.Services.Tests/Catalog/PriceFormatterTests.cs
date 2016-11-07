using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class PriceFormatterTests : ServiceTest
    {
        private IRepository<Currency> _currencyRepo;
        private IStoreMappingService _storeMappingService;
        private ICurrencyService _currencyService;
        private CurrencySettings _currencySettings;
        private IWorkContext _workContext;
        private ILocalizationService _localizationService;
        private TaxSettings _taxSettings;
        private IPriceFormatter _priceFormatter;
        
        [SetUp]
        public new void SetUp()
        {
            var cacheManager = new NopNullCache();

            _workContext = null;

            _currencySettings = new CurrencySettings();

            _currencyRepo = MockRepository.GenerateMock<IRepository<Currency>>();
            _currencyRepo.Expect(x => x.Table).Return(new List<Currency> { TestHelper.GetCurrency() }.AsQueryable());

            _storeMappingService = MockRepository.GenerateMock<IStoreMappingService>();

            var pluginFinder = new PluginFinder();
            _currencyService = new CurrencyService(cacheManager, _currencyRepo, _storeMappingService,
                _currencySettings, pluginFinder, null);

            _taxSettings = new TaxSettings();

            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _localizationService.Expect(x => x.GetResource("Products.InclTaxSuffix", 1, false)).Return("{0} incl tax");
            _localizationService.Expect(x => x.GetResource("Products.ExclTaxSuffix", 1, false)).Return("{0} excl tax");
            
            _priceFormatter = new PriceFormatter(_workContext, _currencyService, _localizationService, 
                _taxSettings, _currencySettings);
        }

        [Test]
        public void Can_formatPrice_with_custom_currencyFormatting()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var currency = TestHelper.GetCurrency();
            currency.CustomFormatting = "€0.00";

            _priceFormatter.FormatPrice(1234.5M, false, currency, TestHelper.GetLanguage(), false, false).ShouldEqual("€1234.50");
        }

        [Test]
        public void Can_formatPrice_with_distinct_currencyDisplayLocale()
        {
            var currency = TestHelper.GetCurrency();
            var language = TestHelper.GetLanguage();

            _priceFormatter.FormatPrice(1234.5M, false, currency, language, false, false).ShouldEqual("$1,234.50");
            currency.DisplayLocale = "en-GB";
            _priceFormatter.FormatPrice(1234.5M, false, currency, language, false, false).ShouldEqual("£1,234.50");
        }

        [Test]
        public void Can_formatPrice_with_showTax()
        {
            var currency = TestHelper.GetCurrency();
            var language = TestHelper.GetLanguage();

            _priceFormatter.FormatPrice(1234.5M, false, currency, language, true, true).ShouldEqual("$1,234.50 incl tax");
            _priceFormatter.FormatPrice(1234.5M, false, currency, language, false, true).ShouldEqual("$1,234.50 excl tax");
        }

        [Test]
        public void Can_formatPrice_with_showCurrencyCode()
        {
            var currency = TestHelper.GetCurrency();
            var language = TestHelper.GetLanguage();

            _currencySettings.DisplayCurrencyLabel = true;
            _priceFormatter.FormatPrice(1234.5M, true, currency, language, false, false).ShouldEqual("$1,234.50 (USD)");
            
            _currencySettings.DisplayCurrencyLabel = false;
            _priceFormatter.FormatPrice(1234.5M, true, currency, language, false, false).ShouldEqual("$1,234.50");
        }
    }
}
