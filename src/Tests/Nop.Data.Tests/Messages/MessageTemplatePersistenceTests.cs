using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Messages
{
    [TestFixture]
    public class MessageTemplatePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_messageTemplate()
        {
            var mt = TestHelper.GetMessageTemplate();

            var fromDb = SaveAndLoadEntity(mt);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Template1");
            fromDb.BccEmailAddresses.ShouldEqual("Bcc");
            fromDb.Subject.ShouldEqual("Subj");
            fromDb.Body.ShouldEqual("Some text");
            fromDb.IsActive.ShouldBeTrue();
            fromDb.AttachedDownloadId.ShouldEqual(3);
            fromDb.LimitedToStores.ShouldBeTrue();
            fromDb.DelayBeforeSend.ShouldEqual(2);
            fromDb.DelayPeriodId.ShouldEqual(0);
            fromDb.EmailAccountId.ShouldEqual(1);
        }
    }
}