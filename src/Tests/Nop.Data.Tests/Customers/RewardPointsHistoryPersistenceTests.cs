using System;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class RewardPointsHistoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_rewardPointsHistory()
        {
            var rewardPointsHistory = TestHelper.GetRewardPointsHistory();

            var fromDb = SaveAndLoadEntity(rewardPointsHistory);
            fromDb.ShouldNotBeNull();
            fromDb.StoreId.ShouldEqual(1);
            fromDb.Points.ShouldEqual(2);
            fromDb.Message.ShouldEqual("Points for registration");
            fromDb.PointsBalance.ShouldEqual(3);
            fromDb.UsedAmount.ShouldEqual(3.1M);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.Customer.ShouldNotBeNull();
        }
        [Test]
        public void Can_save_and_load_rewardPointsHistory_with_order()
        {
            var rewardPointsHistory = TestHelper.GetRewardPointsHistory();
            rewardPointsHistory.UsedWithOrder = TestHelper.GetOrder();

            var fromDb = SaveAndLoadEntity(rewardPointsHistory);
            fromDb.ShouldNotBeNull();

            fromDb.UsedWithOrder.ShouldNotBeNull();
            fromDb.UsedWithOrder.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }
    }
}