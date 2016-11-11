using Autofac;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class BackInStockSubscriptionServiceTests: ServiceTest
    {
        private BackInStockSubscriptionService _backInStockSubscriptionService;

        [SetUp]
        public new void SetUp()
        {
            var backInStockSubscriptionRepository = MockRepository.GenerateMock<IRepository<BackInStockSubscription>>();
            var workflowMessageService = MockRepository.GenerateMock<IWorkflowMessageService>();
            var eventPublisher = MockRepository.GenerateMock<IEventPublisher>();

            var backInStockSubscription1 = TestHelper.GetBackInStockSubscription();
            backInStockSubscription1.CustomerId = 1;
            backInStockSubscription1.ProductId = 1;
            var backInStockSubscription2 = TestHelper.GetBackInStockSubscription();
            backInStockSubscription2.CustomerId = 1;
            backInStockSubscription2.ProductId = 1;
            var backInStockSubscription3 = TestHelper.GetBackInStockSubscription();
            backInStockSubscription3.CustomerId = 2;
            backInStockSubscription3.ProductId = 2;
            backInStockSubscription3.StoreId = 1;

            var backInStockSubscriptions = TestHelper.ToIQueryable(backInStockSubscription1, backInStockSubscription2, backInStockSubscription3);

            backInStockSubscriptionRepository.Expect(x => x.Table).Return(backInStockSubscriptions);
            backInStockSubscriptionRepository.Expect(x => x.GetById(1)).Return(backInStockSubscription1);

            workflowMessageService.Expect(x => x.SendBackInStockNotification(backInStockSubscription1, 1)).Return(1);

            _backInStockSubscriptionService =new BackInStockSubscriptionService(backInStockSubscriptionRepository, workflowMessageService, eventPublisher);

            var nopEngine = MockRepository.GenerateMock<NopEngine>();
            var containe = MockRepository.GenerateMock<IContainer>();
            var containerManager = MockRepository.GenerateMock<ContainerManager>(containe);
            nopEngine.Expect(x => x.ContainerManager).Return(containerManager);

            var genericAttributeService = MockRepository.GenerateMock<IGenericAttributeService>();
            containerManager.Expect(x => x.Resolve<IGenericAttributeService>()).Return(genericAttributeService);

            EngineContext.Replace(nopEngine);
        }

        [Test]
        public void can_get_all_subscriptions_by_customer_id()
        {
            _backInStockSubscriptionService.GetAllSubscriptionsByCustomerId(1).ShouldNotBeNull().Count.ShouldEqual(2);
            _backInStockSubscriptionService.GetAllSubscriptionsByCustomerId(2).ShouldNotBeNull().Count.ShouldEqual(1);
            _backInStockSubscriptionService.GetAllSubscriptionsByCustomerId(2, 1).ShouldNotBeNull().Count.ShouldEqual(1);
            _backInStockSubscriptionService.GetAllSubscriptionsByCustomerId(2, 2).ShouldNotBeNull().Count.ShouldEqual(0);
            _backInStockSubscriptionService.GetAllSubscriptionsByCustomerId(3).ShouldNotBeNull().Count.ShouldEqual(0);
        }

        [Test]
        public void can_get_all_subscriptions_by_product_id()
        {
            _backInStockSubscriptionService.GetAllSubscriptionsByProductId(1).ShouldNotBeNull().Count.ShouldEqual(2);
            _backInStockSubscriptionService.GetAllSubscriptionsByProductId(2).ShouldNotBeNull().Count.ShouldEqual(1);
            _backInStockSubscriptionService.GetAllSubscriptionsByProductId(2, 1).ShouldNotBeNull().Count.ShouldEqual(1);
            _backInStockSubscriptionService.GetAllSubscriptionsByProductId(2, 2).ShouldNotBeNull().Count.ShouldEqual(0);
            _backInStockSubscriptionService.GetAllSubscriptionsByProductId(3).ShouldNotBeNull().Count.ShouldEqual(0);
        }

        [Test]
        public void can_find_subscription()
        {
            //int customerId, int productId, int storeId
            _backInStockSubscriptionService.FindSubscription(1, 1, 0).ShouldNotBeNull();
            _backInStockSubscriptionService.FindSubscription(1, 1, 1).ShouldBeNull();
            _backInStockSubscriptionService.FindSubscription(2, 2, 1).ShouldNotBeNull();
        }

        [Test]
        public void can_get_subscription_by_id()
        {
            _backInStockSubscriptionService.GetSubscriptionById(0).ShouldBeNull();
            _backInStockSubscriptionService.GetSubscriptionById(1).ShouldNotBeNull().CustomerId.ShouldEqual(1);
        }

        [Test]
        public void can_send_notifications_to_subscribers()
        {
            var product = TestHelper.GetProduct();
            _backInStockSubscriptionService.SendNotificationsToSubscribers(product).ShouldEqual(2);
            product.Id = 2;
            _backInStockSubscriptionService.SendNotificationsToSubscribers(product).ShouldEqual(1);
        }
    }
}
