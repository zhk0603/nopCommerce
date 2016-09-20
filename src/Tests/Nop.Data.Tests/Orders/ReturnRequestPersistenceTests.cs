using System;
using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class ReturnRequestPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_returnRequest()
        {
            var rr = TestHelper.GetReturnRequest();

            var fromDb = SaveAndLoadEntity(rr);
            fromDb.ShouldNotBeNull();
            fromDb.CustomNumber.ShouldEqual("CustomNumber 1");
            fromDb.StoreId.ShouldEqual(1);
            fromDb.Customer.ShouldNotBeNull();
            fromDb.Quantity.ShouldEqual(2);
            fromDb.ReasonForReturn.ShouldEqual("Wrong product");
            fromDb.RequestedAction.ShouldEqual("Refund");
            fromDb.CustomerComments.ShouldEqual("Some comment");
            fromDb.StaffNotes.ShouldEqual("Some notes");
            fromDb.ReturnRequestStatus.ShouldEqual(ReturnRequestStatus.ItemsRefunded);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}