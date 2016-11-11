using System.Web;
using Autofac;
using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Affiliates;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Affiliates
{
    [TestFixture]
    public class AffiliateExtensionsTests
    {
        private IWebHelper _webHelper;

        [SetUp]
        public void SetUp()
        {
            var a1 = TestHelper.GetAffiliate();
            a1.Id = 1;

            var httpContext = MockRepository.GenerateMock<HttpContextBase>();
            var request = MockRepository.GenerateMock<HttpRequestBase>();
            var containe = MockRepository.GenerateMock<IContainer>();
            var containerManager = MockRepository.GenerateMock<ContainerManager>(containe);
            var affiliateService = MockRepository.GenerateMock<IAffiliateService>();
            var nopEngine = MockRepository.GenerateMock<NopEngine>();

            request.Expect(x => x.ServerVariables["HTTP_HOST"]).Return("test.com");
            affiliateService.Expect(x => x.GetAffiliateByFriendlyUrlName("test-1-name")).Return(a1);
            httpContext.Expect(x => x.Request).Return(request);
            containerManager.Expect(x => x.Resolve<SeoSettings>()).Return(new SeoSettings());
            containerManager.Expect(x => x.Resolve<IAffiliateService>()).Return(affiliateService);
            nopEngine.Expect(x => x.ContainerManager).Return(containerManager);
            _webHelper = new WebHelper(httpContext);

            EngineContext.Replace(nopEngine);
        }

        [Test]
        public void can_get_full_name()
        {
            var affiliate = TestHelper.GetAffiliate();
            affiliate.GetFullName().ShouldEqual("FirstName 1 LastName 1");
            affiliate.Address.FirstName = string.Empty;
            affiliate.GetFullName().ShouldEqual("LastName 1");
            affiliate.Address.LastName = string.Empty;
            affiliate.GetFullName().ShouldEqual(string.Empty);
            affiliate.Address.FirstName = "FirstName 1";
            affiliate.GetFullName().ShouldEqual("FirstName 1");
        }

        [Test]
        public void can_generate_url()
        {
            var affiliate = TestHelper.GetAffiliate();
            affiliate.GenerateUrl(_webHelper).ShouldEqual("http://test.com/?affiliate=friendlyurlname 1");
            affiliate.FriendlyUrlName = string.Empty;
            affiliate.GenerateUrl(_webHelper).ShouldEqual("http://test.com/?affiliateid=0");
        }

        [Test]
        public void can_validate_friendly_url_name()
        {
            var affiliate = TestHelper.GetAffiliate();
            affiliate.ValidateFriendlyUrlName("test 1 name").ShouldEqual("test-1-name-2");
            affiliate.ValidateFriendlyUrlName("<test 1> &nbsp;").ShouldEqual("test-1-nbsp");
        }
    }
}
