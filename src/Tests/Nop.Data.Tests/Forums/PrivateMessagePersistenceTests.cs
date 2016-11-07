using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Forums
{
    [TestFixture]
    public class PrivateMessagePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_privatemessage()
        {
            var storeFromDb = SaveAndLoadEntity(TestHelper.GetStore());
            storeFromDb.ShouldNotBeNull();

            var customer1 = TestHelper.GetCustomer();
            var customer1FromDb = SaveAndLoadEntity(customer1);
            customer1FromDb.ShouldNotBeNull();

            var customer2 = TestHelper.GetCustomer();
            var customer2FromDb = SaveAndLoadEntity(customer2);
            customer2FromDb.ShouldNotBeNull();

            var privateMessage = TestHelper.GetPrivateMessage();
            privateMessage.FromCustomer = customer1FromDb;
            privateMessage.ToCustomer = customer2FromDb;
            privateMessage.StoreId = storeFromDb.Id;

            var fromDb = SaveAndLoadEntity(privateMessage);
            fromDb.ShouldNotBeNull();
            fromDb.Subject.ShouldEqual("Private Message 1 Subject");
            fromDb.Text.ShouldEqual("Private Message 1 Text");
            fromDb.IsDeletedByAuthor.ShouldBeFalse();
            fromDb.IsDeletedByRecipient.ShouldBeFalse();
            fromDb.IsRead.ShouldBeFalse();
        }
    }
}
