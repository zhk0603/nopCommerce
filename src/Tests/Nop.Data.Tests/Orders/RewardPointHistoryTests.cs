using System;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class RewardPointHistoryTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_rewardPointHistory()
        {
            var rph = TestHelper.GetRewardPointsHistory();
            var fromDb = SaveAndLoadEntity(rph);
            fromDb.ShouldNotBeNull();
            fromDb.StoreId.ShouldEqual(1);
            fromDb.Customer.ShouldNotBeNull();
            fromDb.Points.ShouldEqual(2);
            fromDb.PointsBalance.ShouldEqual(3);
            fromDb.UsedAmount.ShouldEqual(3.1M);
            fromDb.Message.ShouldEqual("Points for registration");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }
    }
}
