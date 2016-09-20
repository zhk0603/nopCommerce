using System;
using Nop.Core.Domain.Common;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Common
{
    [TestFixture]
    public class AddressTests
    {
        [Test]
        public void Can_clone_address()
        {
            var address = TestHelper.GetAddress(3, 4);

            var newAddress = address.Clone() as Address;
            newAddress.ShouldNotBeNull();
            newAddress.Id.ShouldEqual(0);
            newAddress.FirstName.ShouldEqual("FirstName 1");
            newAddress.LastName.ShouldEqual("LastName 1");
            newAddress.Email.ShouldEqual("Email 1");
            newAddress.Company.ShouldEqual("Company 1");
            newAddress.City.ShouldEqual("City 1");
            newAddress.Address1.ShouldEqual("Address1");
            newAddress.Address2.ShouldEqual("Address2");
            newAddress.ZipPostalCode.ShouldEqual("ZipPostalCode 1");
            newAddress.PhoneNumber.ShouldEqual("PhoneNumber 1");
            newAddress.FaxNumber.ShouldEqual("FaxNumber 1");
            newAddress.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            newAddress.Country.ShouldNotBeNull();
            newAddress.CountryId.ShouldEqual(4);
            newAddress.Country.Name.ShouldEqual("United States");
            newAddress.StateProvince.ShouldNotBeNull();
            newAddress.StateProvinceId.ShouldEqual(3);
            newAddress.StateProvince.Name.ShouldEqual("Louisiana");
        }
    }
}
