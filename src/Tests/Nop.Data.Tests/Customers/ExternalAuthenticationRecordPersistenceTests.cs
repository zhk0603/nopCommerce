using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class ExternalAuthenticationRecordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_externalAuthenticationRecord()
        {
            var externalAuthenticationRecord = TestHelper.GetExternalAuthenticationRecord();
            externalAuthenticationRecord.Customer = TestHelper.GetCustomer();

            var fromDb = SaveAndLoadEntity(externalAuthenticationRecord);
            fromDb.ShouldNotBeNull();
            fromDb.Email.ShouldEqual("Email 1");
            fromDb.ExternalIdentifier.ShouldEqual("ExternalIdentifier 1");
            fromDb.ExternalDisplayIdentifier.ShouldEqual("ExternalDisplayIdentifier 1");
            fromDb.OAuthToken.ShouldEqual("OAuthToken 1");
            fromDb.OAuthAccessToken.ShouldEqual("OAuthAccessToken 1");
            fromDb.ProviderSystemName.ShouldEqual("ProviderSystemName 1");

            fromDb.Customer.ShouldNotBeNull();
        }
    }
}