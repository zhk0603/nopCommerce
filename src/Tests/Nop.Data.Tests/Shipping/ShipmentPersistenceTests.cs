using System;
using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Shipping
{
    [TestFixture]
    public class ShipmentPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_shipment()
        {
            var shipment = TestHelper.GetShipment(TestHelper.GetOrder());

            var fromDb = SaveAndLoadEntity(shipment);
            fromDb.ShouldNotBeNull();
            fromDb.TrackingNumber.ShouldEqual("TrackingNumber 1");
            fromDb.TotalWeight.ShouldEqual(9.87M);
            fromDb.ShippedDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.DeliveryDateUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.AdminComment.ShouldEqual("AdminComment 1");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 03));
        }

        [Test]
        public void Can_save_and_load_shipment_with_items()
        {
            var shipment = TestHelper.GetShipment(TestHelper.GetOrder());
            shipment.ShipmentItems.Add(TestHelper.GetShipmentItem());

            var fromDb = SaveAndLoadEntity(shipment);
            fromDb.ShouldNotBeNull();

            fromDb.ShipmentItems.ShouldNotBeNull();
            (fromDb.ShipmentItems.Count == 1).ShouldBeTrue();
            fromDb.ShipmentItems.First().Quantity.ShouldEqual(3);
        }
    }
}