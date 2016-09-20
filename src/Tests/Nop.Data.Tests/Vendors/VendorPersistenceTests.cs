using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Vendors
{
    [TestFixture]
    public class VendorPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_vendor()
        {
            var vendor = TestHelper.GetVendor();

            var fromDb = SaveAndLoadEntity(vendor);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.Email.ShouldEqual("Email 1");
            fromDb.Description.ShouldEqual("Description 1");
            fromDb.AdminComment.ShouldEqual("AdminComment 1");
            fromDb.PictureId.ShouldEqual(1);
            fromDb.Active.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(2);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.MetaTitle.ShouldEqual("Meta title");
            fromDb.PageSize.ShouldEqual(4);
            fromDb.AllowCustomersToSelectPageSize.ShouldEqual(true);
            fromDb.PageSizeOptions.ShouldEqual("4, 2, 8, 12");
        }

        [Test]
        public void Can_save_and_load_vendor_with_vendorNotes()
        {
            var vendor = TestHelper.GetVendor();
            vendor.VendorNotes.Add(TestHelper.GetVendorNote());

            var fromDb = SaveAndLoadEntity(vendor);
            fromDb.ShouldNotBeNull();

            fromDb.VendorNotes.ShouldNotBeNull();
            fromDb.VendorNotes.Count.ShouldEqual(1);
            fromDb.VendorNotes.First().Note.ShouldEqual("Note1");
        }

    }
}
