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
            var tierPrices = new List<TierPrice>
            {
                //will be removed
                TestHelper.GetTierPrice(price: 150),
                //will stay
                TestHelper.GetTierPrice(price: 100, id: 2),
                //will stay
                TestHelper.GetTierPrice(price: 300, quantity: 3, id: 3),
                //will stay
                TestHelper.GetTierPrice(price: 250, quantity: 4, id: 4),
                //will be removed
                TestHelper.GetTierPrice(price: 300, quantity: 4, id: 5),
                 //will stay
                TestHelper.GetTierPrice(price: 350, quantity: 5, id: 6)
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
