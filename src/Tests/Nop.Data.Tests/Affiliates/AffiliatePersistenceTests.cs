using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Affiliates
{
    [TestFixture]
    public class AffiliatePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_affiliate()
        {
            var affiliate = TestHelper.GetAffiliate();

            var fromDb = SaveAndLoadEntity(affiliate);
            fromDb.ShouldNotBeNull();
            fromDb.Deleted.ShouldEqual(true);
            fromDb.Active.ShouldEqual(true);
            fromDb.Address.ShouldNotBeNull();
            fromDb.Address.FirstName.ShouldEqual("FirstName 1");
            fromDb.AdminComment.ShouldEqual("AdminComment 1");
            fromDb.FriendlyUrlName.ShouldEqual("FriendlyUrlName 1");
        }

    }
}
