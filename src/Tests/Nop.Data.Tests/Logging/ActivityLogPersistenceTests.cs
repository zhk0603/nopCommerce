using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Logging
{
    [TestFixture]
    public class ActivityLogPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_activityLogType()
        {
            var activityLogType = TestHelper.GetActivityLogType();

            var fromDb = SaveAndLoadEntity(activityLogType);
            fromDb.ShouldNotBeNull();
            fromDb.SystemKeyword.ShouldEqual("SystemKeyword 1");
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.Enabled.ShouldEqual(true);
        }
    }
}