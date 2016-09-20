using System;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class ProductExtensionTests : ServiceTest
    {
        [SetUp]
        public new void SetUp()
        {
        }

        [Test]
        public void Can_parse_allowed_quantities()
        {
            var product = TestHelper.GetProduct();

            var result = product.ParseAllowedQuantities();
            result.Length.ShouldEqual(4);
            result[0].ShouldEqual(1);
            result[1].ShouldEqual(5);
            result[2].ShouldEqual(4);
            result[3].ShouldEqual(10);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_not_use_multiple_warehouses()
        {
            var product = TestHelper.GetProduct(useMultipleWarehouses: false);
            
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 1,
                stockQuantity: 2));
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 2,
                stockQuantity: 8));
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 3,
                stockQuantity: -2));

            var result = product.GetTotalStockQuantity(true);
            result.ShouldEqual(13);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_use_multiple_warehouses_with_reserved()
        {
            var product = TestHelper.GetProduct();

            //ReservedQuantity = 4
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 1,
                stockQuantity: 8));
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 2,
                stockQuantity: 14));
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 3,
                stockQuantity: -2));

            var result = product.GetTotalStockQuantity(true);
            result.ShouldEqual(8);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_use_multiple_warehouses_without_reserved()
        {
            var product = TestHelper.GetProduct();

            //ReservedQuantity = 4
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 1,
                stockQuantity: 8));
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 2,
                stockQuantity: 14));
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 3,
                stockQuantity: -2));

            var result = product.GetTotalStockQuantity(false);
            result.ShouldEqual(20);
        }

        [Test]
        public void Can_calculate_total_quantity_when_we_do_use_multiple_warehouses_with_warehouse_specified()
        {
            var product = TestHelper.GetProduct();

            //ReservedQuantity = 4
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 1,
                stockQuantity: 8));
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 2,
                stockQuantity: 14));
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product, warehouseId: 3,
                stockQuantity: -2));

            var result = product.GetTotalStockQuantity(true, 1);
            result.ShouldEqual(4);
        }

        [Test]
        public void Can_calculate_rental_periods_for_days()
        {
            var product = TestHelper.GetProduct();
            var startDate = new DateTime(2014, 3, 5);

            DateTime[] dt =
            {
                //the same date
                startDate,
                //1 day
                new DateTime(2014, 3, 6),
                //2 days
                new DateTime(2014, 3, 7),
                //3 days
                new DateTime(2014, 3, 8)
            };

            //rental period length = 1 day
            product.RentalPriceLength = 1;
            RentalPeriodsTest(product, startDate, dt, new[] { 1, 1, 2, 3 });

            //rental period length = 2 days
            product.RentalPriceLength = 2;
            RentalPeriodsTest(product, startDate, dt, new[] { 1, 1, 1, 2 });
        }

        [Test]
        public void Can_calculate_rental_periods_for_weeks()
        {
            var product = TestHelper.GetProduct(rentalPricePeriod: RentalPricePeriod.Weeks);
            var startDate = new DateTime(2014, 3, 5);

            DateTime[] dt =
            {
                //the same date
                startDate,
                //several days but less than a week
                new DateTime(2014, 3, 3),
                //1 week
                new DateTime(2014, 3, 12),
                //several days but less than two weeks
                new DateTime(2014, 3, 13),
                //2 weeks
                new DateTime(2014, 3, 19),
                //3 weeks
                new DateTime(2014, 3, 26)
            };

            //rental period length = 1 week
            product.RentalPriceLength = 1;
            RentalPeriodsTest(product, startDate, dt, new[] { 1, 1, 1, 2, 2, 3 });

            //rental period length = 2 weeks
            product.RentalPriceLength = 2;
            RentalPeriodsTest(product, startDate, dt, new[] { 1, 1, 1, 1, 1, 2 });
        }

        [Test]
        public void Can_calculate_rental_periods_for_months()
        {
            var product = TestHelper.GetProduct(rentalPricePeriod: RentalPricePeriod.Months);
            var startDate = new DateTime(2014, 3, 5);

            DateTime[] dt =
            {
                //the same date
                startDate,
                //several days but less than a month
                new DateTime(2014, 3, 4),
                //1 month
                new DateTime(2014, 4, 5),
                //1 month and 1 day
                new DateTime(2014, 4, 6),
                //several days but less than two months
                new DateTime(2014, 4, 13),
                //2 months
                new DateTime(2014, 5, 5),
                //3 months
                new DateTime(2014, 5, 8)
            };

            //rental period length = 1 month
            product.RentalPriceLength = 1;
            RentalPeriodsTest(product, startDate, dt, new[] { 1, 1, 1, 2, 2, 2, 3 });

            //several more unit tests
            RentalPeriodsTest(product, new DateTime(1900, 1, 1),
                new[]
                {
                    new DateTime(1900, 1, 1),
                    new DateTime(1900, 1, 2),
                    new DateTime(1900, 2, 1),
                    new DateTime(1901, 1, 1),
                    new DateTime(1911, 1, 1)
                }, new[] { 1, 1, 1, 12, 132 });

            RentalPeriodsTest(product, new DateTime(1900, 8, 31),
                new[]
                {
                    new DateTime(1901, 8, 30),
                    new DateTime(1900, 9, 30),
                    new DateTime(1900, 10, 1)
                }, new[] { 12, 1, 2 });

            product.GetRentalPeriods(new DateTime(1900, 2, 1), new DateTime(1900, 1, 1)).ShouldEqual(1);
            product.GetRentalPeriods(new DateTime(1900, 1, 31), new DateTime(1900, 2, 1)).ShouldEqual(1);

            //rental period length = 2 months
            product.RentalPriceLength = 2;
            RentalPeriodsTest(product, startDate, dt, new[] { 1, 1, 1, 1, 1, 1, 2 });
        }

        [Test]
        public void Can_calculate_rental_periods_for_years()
        {
            var product = TestHelper.GetProduct(rentalPricePeriod: RentalPricePeriod.Years);
            var startDate = new DateTime(2014, 3, 5);

            DateTime[] dt =
            {
                //the same date
                startDate,
                //several days but less than a month
                new DateTime(2014, 3, 4),
                //more than one year
                new DateTime(2015, 3, 7),
                //more than two year
                new DateTime(2016, 3, 7)
            };

            //rental period length = 1 years
            product.RentalPriceLength = 1;
            RentalPeriodsTest(product, startDate, dt, new[] { 1, 1, 2, 3 });

            //rental period length = 2 years
            product.RentalPriceLength = 2;
            RentalPeriodsTest(product, startDate, dt, new[] { 1, 1, 1, 2 });
        }

        void RentalPeriodsTest(Product product, DateTime startDateTime, DateTime[] endsPeriod, int[] rentalPeriods)
        {
            endsPeriod.Length.ShouldEqual(rentalPeriods.Length);

            for (var i = 0; i < endsPeriod.Length; i++)
            {
                product.GetRentalPeriods(startDateTime, endsPeriod[i]).ShouldEqual(rentalPeriods[i]);
            }
        }
    }
}
