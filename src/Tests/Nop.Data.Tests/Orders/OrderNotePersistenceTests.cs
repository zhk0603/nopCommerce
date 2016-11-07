using System;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class OrderNotePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_orderNote()
        {
            var on = TestHelper.GetOrderNote();
            on.Order = TestHelper.GetOrder();

            var fromDb = SaveAndLoadEntity(on);
            fromDb.ShouldNotBeNull();
            fromDb.Note.ShouldEqual("Note1");
            fromDb.DownloadId.ShouldEqual(1);
            fromDb.DisplayToCustomer.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.Order.ShouldNotBeNull();
        }
    }
}