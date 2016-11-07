using System;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Vendors
{
    [TestFixture]
    public class VendorNotePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_vendorNote()
        {
            var vendorNote = TestHelper.GetVendorNote();
            vendorNote.Vendor = TestHelper.GetVendor();

            var fromDb = SaveAndLoadEntity(vendorNote);
            fromDb.ShouldNotBeNull();
            fromDb.Note.ShouldEqual("Note1");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.Vendor.ShouldNotBeNull();
        }
    }
}