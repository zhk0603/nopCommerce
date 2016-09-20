using System;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Common
{
    [TestFixture]
    public class AddressPeristenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_address()
        {
            var address = TestHelper.GetAddress();

            var fromDb = SaveAndLoadEntity(address);
            fromDb.ShouldNotBeNull();

            fromDb.FirstName.ShouldEqual("FirstName 1");
            fromDb.LastName.ShouldEqual("LastName 1");
            fromDb.Email.ShouldEqual("Email 1");
            fromDb.Company.ShouldEqual("Company 1");
            fromDb.City.ShouldEqual("City 1");
            fromDb.Address1.ShouldEqual("Address1");
            fromDb.Address2.ShouldEqual("Address2");
            fromDb.ZipPostalCode.ShouldEqual("ZipPostalCode 1");
            fromDb.PhoneNumber.ShouldEqual("PhoneNumber 1");
            fromDb.FaxNumber.ShouldEqual("FaxNumber 1");
            fromDb.CustomAttributes.ShouldEqual("CustomAttributes 1");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.Country.ShouldNotBeNull();
            fromDb.Country.Name.ShouldEqual("United States");

        }

        [Test]
        public void Can_save_and_load_address_with_stateProvince()
        {
            var address = TestHelper.GetAddress();

            var fromDb = SaveAndLoadEntity(address);
            fromDb.ShouldNotBeNull();

            fromDb.StateProvince.ShouldNotBeNull();
            fromDb.StateProvince.Name.ShouldEqual("Louisiana");
        }
    }
}
