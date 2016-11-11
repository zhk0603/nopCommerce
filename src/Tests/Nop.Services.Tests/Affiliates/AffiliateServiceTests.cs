using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Orders;
using Nop.Services.Affiliates;
using Nop.Services.Events;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Affiliates
{
    [TestFixture]
    public class AffiliateServiceTests: ServiceTest
    {
        private IAffiliateService _affiliateService;

        [SetUp]
        public new void SetUp()
        {
            var a1 = TestHelper.GetAffiliate();
            a1.Id = 1;
            var o1 = TestHelper.GetOrder();
            o1.AffiliateId = 1;
            a1.Active = false;

            var a2 = TestHelper.GetAffiliate();
            a2.Id = 2;

            a1.Deleted = a2.Deleted = false;

            var affiliates = TestHelper.ToIQueryable(a1, a2);
            var orders = TestHelper.ToIQueryable(o1);

            var affiliateRepository = MockRepository.GenerateMock<IRepository<Affiliate>>();
            var orderRepository = MockRepository.GenerateMock<IRepository<Order>>();
            var eventPublisher = MockRepository.GenerateMock<IEventPublisher>();

            affiliateRepository.Expect(x => x.GetById(1)).Return(a1);
            affiliateRepository.Expect(x => x.GetById(2)).Return(a2);
            affiliateRepository.Expect(x => x.Table).Return(affiliates);
            orderRepository.Expect(x => x.Table).Return(orders);

            _affiliateService = new AffiliateService(affiliateRepository, orderRepository, eventPublisher);
        }

        [Test]
        public void can_get_affiliate_by_id()
        {
            _affiliateService.GetAffiliateById(0).ShouldBeNull();
            var affiliate = _affiliateService.GetAffiliateById(1);
            affiliate.ShouldNotBeNull();
            affiliate.FriendlyUrlName.ShouldEqual("FriendlyUrlName 1");
        }

        [Test]
        public void can_get_affiliate_by_friendly_url_name()
        {
            _affiliateService.GetAffiliateByFriendlyUrlName("new FriendlyUrlName").ShouldBeNull();
            var affiliate = _affiliateService.GetAffiliateByFriendlyUrlName("FriendlyUrlName 1");
            affiliate.ShouldNotBeNull();
            affiliate.Id.ShouldEqual(1);
        }

        [Test]
        public void can_get_all_affiliates()
        {
            var affiliates = _affiliateService.GetAllAffiliates("FriendlyUrlName 1", "FirstName 1", "LastName 1", showHidden: true);
            affiliates.ShouldNotBeNull();
            affiliates.Count.ShouldEqual(2);
            affiliates = _affiliateService.GetAllAffiliates("FriendlyUrlName 1", "FirstName 1", "LastName 1", true);
            affiliates.ShouldNotBeNull();
            affiliates.Count.ShouldEqual(0);
            affiliates = _affiliateService.GetAllAffiliates("FriendlyUrlName 1", "FirstName 1", "LastName 1");
            affiliates.ShouldNotBeNull();
            affiliates.Count.ShouldEqual(1);
        }
    }
}
