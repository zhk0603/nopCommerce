using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Stores
{
    [TestFixture]
    public class StorePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_store()
        {
            var store = TestHelper.GetStore();

            var fromDb = SaveAndLoadEntity(store);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Store 1");
            fromDb.Url.ShouldEqual("http://www.test.com");
            fromDb.Hosts.ShouldEqual("yourstore.com, www.yourstore.com, ");
            fromDb.DefaultLanguageId.ShouldEqual(1);
            fromDb.DisplayOrder.ShouldEqual(1);
            fromDb.CompanyName.ShouldEqual("company name");
            fromDb.CompanyAddress.ShouldEqual("some address");
            fromDb.CompanyPhoneNumber.ShouldEqual("123456789");
            fromDb.CompanyVat.ShouldEqual("some vat");
        }
    }
}
