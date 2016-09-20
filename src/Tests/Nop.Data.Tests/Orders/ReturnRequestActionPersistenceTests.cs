using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class ReturnRequestActionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_returnRequestAction()
        {
            var returnRequestAction = TestHelper.GetReturnRequestAction();

            var fromDb = SaveAndLoadEntity(returnRequestAction);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}
