using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class TierPriceExtensionTests : ServiceTest
    {
        [SetUp]
        public new void SetUp()
        {
        }

        [Test]
        public void Can_remove_duplicatedQuantities()
        {
            var tierPrice1 = TestHelper.GetTierPrice();
            tierPrice1.Price = 150;
            var tierPrice2 = TestHelper.GetTierPrice();
            tierPrice2.Price = 100;
            tierPrice2.Id = 2;
            var tierPrice3 = TestHelper.GetTierPrice();
            tierPrice3.Price = 300;
            tierPrice3.Id = 3;
            tierPrice3.Quantity = 3;
            var tierPrice4= TestHelper.GetTierPrice();
            tierPrice4.Price = 250;
            tierPrice4.Id = 4;
            tierPrice4.Quantity = 4;
            var tierPrice5 = TestHelper.GetTierPrice();
            tierPrice5.Price = 300;
            tierPrice5.Id = 5;
            tierPrice5.Quantity = 4;
            var tierPrice6 = TestHelper.GetTierPrice();
            tierPrice6.Price = 350;
            tierPrice6.Id = 6;
            tierPrice6.Quantity = 6;

            var tierPrices = new List<TierPrice>
            {
                //will be removed
                tierPrice1,
                //will stay
                tierPrice2,
                //will stay
                tierPrice3,
                //will stay
                tierPrice4,
                //will be removed
                tierPrice5,
                 //will stay
                tierPrice6
            };

            tierPrices.RemoveDuplicatedQuantities();

            tierPrices.FirstOrDefault(x => x.Id == 1).ShouldBeNull();
            tierPrices.FirstOrDefault(x => x.Id == 2).ShouldNotBeNull();
            tierPrices.FirstOrDefault(x => x.Id == 3).ShouldNotBeNull();
            tierPrices.FirstOrDefault(x => x.Id == 4).ShouldNotBeNull();
            tierPrices.FirstOrDefault(x => x.Id == 5).ShouldBeNull();
            tierPrices.FirstOrDefault(x => x.Id == 6).ShouldNotBeNull();
        }
    }
}
