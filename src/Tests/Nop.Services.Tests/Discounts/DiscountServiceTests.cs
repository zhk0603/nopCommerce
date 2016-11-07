using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Discounts
{
    [TestFixture]
    public class DiscountServiceTests : ServiceTest
    {
        private IRepository<Discount> _discountRepo;
        private IRepository<DiscountRequirement> _discountRequirementRepo;
        private IRepository<DiscountUsageHistory> _discountUsageHistoryRepo;
        private IEventPublisher _eventPublisher;
        private IGenericAttributeService _genericAttributeService;
        private ILocalizationService _localizationService;
        private ICategoryService _categoryService;
        private IDiscountService _discountService;
        private IStoreContext _storeContext;
        private IWorkContext _workContext;

        [SetUp]
        public new void SetUp()
        {
            _discountRepo = MockRepository.GenerateMock<IRepository<Discount>>();
            var discount1 = TestHelper.GetDiscount();
            discount1.EndDateUtc = null;
            var discount2 = TestHelper.GetDiscount();
            discount2.EndDateUtc = null;

            _discountRepo.Expect(x => x.Table).Return(new List<Discount> { discount1, discount2 }.AsQueryable());

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _storeContext = MockRepository.GenerateMock<IStoreContext>();
            _workContext = null;

            var cacheManager = new NopNullCache();
            _discountRequirementRepo = MockRepository.GenerateMock<IRepository<DiscountRequirement>>();
            _discountRequirementRepo.Expect(x => x.Table).Return(new List<DiscountRequirement>().AsQueryable());

            _discountUsageHistoryRepo = MockRepository.GenerateMock<IRepository<DiscountUsageHistory>>();
            var pluginFinder = new PluginFinder();
            _genericAttributeService = MockRepository.GenerateMock<IGenericAttributeService>();
            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _categoryService = MockRepository.GenerateMock<ICategoryService>();
            _discountService = new DiscountService(cacheManager, _discountRepo, _discountRequirementRepo,
                _discountUsageHistoryRepo, _storeContext,
                _localizationService, _categoryService, pluginFinder, _eventPublisher, _workContext);
        }

        [Test]
        public void Can_get_all_discount()
        {
            var discounts = _discountService.GetAllDiscounts(null);
            discounts.ShouldNotBeNull();
            discounts.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_load_discountRequirementRules()
        {
            var rules = _discountService.LoadAllDiscountRequirementRules();
            rules.ShouldNotBeNull();
            rules.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_load_discountRequirementRuleBySystemKeyword()
        {
            var rule = _discountService.LoadDiscountRequirementRuleBySystemName("TestDiscountRequirementRule");
            rule.ShouldNotBeNull();
        }

        [Test]
        public void Should_accept_valid_discount_code()
        {
            var discount = TestHelper.GetDiscount();
            discount.EndDateUtc = null;
            discount.CouponCode = "CouponCode 1";
            var customer = TestHelper.GetCustomer();

            var genericAttribute = TestHelper.GetGenericAttribute();
            genericAttribute.EntityId = customer.Id;
            genericAttribute.Value = "CouponCode 1";
            genericAttribute.Key = SystemCustomerAttributeNames.DiscountCouponCode;
            genericAttribute.StoreId = 0;

            _genericAttributeService.Expect(x => x.GetAttributesForEntity(customer.Id, "Customer"))
                .Return(new List<GenericAttribute> { genericAttribute });

            //UNDONE: little workaround here
            //we have to register "nop_cache_static" cache manager (null manager) from DependencyRegistrar.cs
            //because DiscountService right now dynamically Resolve<ICacheManager>("nop_cache_static")
            //we cannot inject it because DiscountService already has "per-request" cache manager injected 
            //EngineContext.Initialize(false);
            
            _discountService.ValidateDiscount(discount, customer, new [] { "CouponCode 1" }).IsValid.ShouldEqual(true);
        }

        [Test]
        public void Should_not_accept_wrong_discount_code()
        {
            var discount = TestHelper.GetDiscount();
            discount.CouponCode = "CouponCode 1";
            discount.EndDateUtc = null;
            var customer = TestHelper.GetCustomer();

            var genericAttribute = TestHelper.GetGenericAttribute();
            genericAttribute.EntityId = customer.Id;
            genericAttribute.Value = "CouponCode 2";

            _genericAttributeService.Expect(x => x.GetAttributesForEntity(customer.Id, "Customer"))
                .Return(new List<GenericAttribute> { genericAttribute });
                
            _discountService.ValidateDiscount(discount, customer, new[] { "CouponCode 2" }).IsValid.ShouldEqual(false);
        }

        [Test]
        public void Can_validate_discount_dateRange()
        {

            var discount = TestHelper.GetDiscount();
            discount.RequiresCouponCode = false;
            discount.StartDateUtc = DateTime.UtcNow.AddDays(-1);
            discount.EndDateUtc = DateTime.UtcNow.AddDays(1);

            var customer = TestHelper.GetCustomer();
            _discountService.ValidateDiscount(discount, customer, null).IsValid.ShouldEqual(true);

            discount.StartDateUtc = DateTime.UtcNow.AddDays(1);
            _discountService.ValidateDiscount(discount, customer, null).IsValid.ShouldEqual(false);
        }
    }
}
