using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Services.Logging;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Logging
{
    [TestFixture]
    public class CustomerActivityServiceTests : ServiceTest
    {
        private ICacheManager _cacheManager;
        private IRepository<ActivityLog> _activityLogRepository;
        private IRepository<ActivityLogType> _activityLogTypeRepository;
        private IWorkContext _workContext;
        private ICustomerActivityService _customerActivityService;
        private ActivityLogType _activityType1, _activityType2;
        private ActivityLog _activity1, _activity2;
        private Customer _customer1, _customer2;
        private IWebHelper _webHelper;

        [SetUp]
        public new void SetUp()
        {
            _activityType1 = TestHelper.GetActivityLogType();
            _activityType1.Id = 1;
            _activityType2 = TestHelper.GetActivityLogType();
            _activityType2.Id = 2;
            _customer1 = TestHelper.GetCustomer();
            _customer1.Id = 1;
           _customer2 = TestHelper.GetCustomer();
            _customer2.Id = 2;
            _activity1 = TestHelper.GetActivityLog();
            _activity1.ActivityLogType = _activityType1;
            _activity1.Customer = _customer1;
            _activity1.CustomerId = _customer1.Id;
            _activity2 = TestHelper.GetActivityLog();
            _activity2.Id = 2;
            _activity2.ActivityLogType = _activityType1;
            _activity2.Customer = _customer2;
            _activity2.CustomerId = _customer2.Id;

            _cacheManager = new NopNullCache();
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _webHelper = MockRepository.GenerateMock<IWebHelper>();
            _activityLogRepository = MockRepository.GenerateMock<IRepository<ActivityLog>>();
            _activityLogTypeRepository = MockRepository.GenerateMock<IRepository<ActivityLogType>>();
            _activityLogTypeRepository.Expect(x => x.Table).Return(new List<ActivityLogType> { _activityType1, _activityType2 }.AsQueryable());
            _activityLogRepository.Expect(x => x.Table).Return(new List<ActivityLog> { _activity1, _activity2 }.AsQueryable());
            _customerActivityService = new CustomerActivityService(_cacheManager, _activityLogRepository, _activityLogTypeRepository, _workContext, null, null, null, _webHelper);
        }

        [Test]
        public void Can_Find_Activities()
        {
            var activities = _customerActivityService.GetAllActivities(null, null, 1, 0, 0, 10);
            activities.Contains(_activity1).ShouldBeTrue();
            activities = _customerActivityService.GetAllActivities(null, null, 2, 0, 0, 10);
            activities.Contains(_activity1).ShouldBeFalse();
            activities = _customerActivityService.GetAllActivities(null, null, 2, 0, 0, 10);
            activities.Contains(_activity2).ShouldBeTrue();
        }
    }
}
