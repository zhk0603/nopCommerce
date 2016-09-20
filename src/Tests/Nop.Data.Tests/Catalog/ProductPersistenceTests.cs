using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductPersistenceTests : PersistenceTest
    {
        [Test]
        public void CanSaveAndLoadProduct()
        {
            var product = TestHelper.GetProduct();
            
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.ProductType.ShouldEqual(ProductType.GroupedProduct);
            fromDb.ParentGroupedProductId.ShouldEqual(2);
            fromDb.VisibleIndividually.ShouldEqual(true);
            fromDb.Name.ShouldEqual("Product name 1");
            fromDb.ShortDescription.ShouldEqual("ShortDescription 1");
            fromDb.FullDescription.ShouldEqual("FullDescription 1");
            fromDb.AdminComment.ShouldEqual("AdminComment 1");
            fromDb.VendorId.ShouldEqual(1);
            fromDb.ProductTemplateId.ShouldEqual(2);
            fromDb.ShowOnHomePage.ShouldEqual(false);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.AllowCustomerReviews.ShouldEqual(true);
            fromDb.ApprovedRatingSum.ShouldEqual(2);
            fromDb.NotApprovedRatingSum.ShouldEqual(3);
            fromDb.ApprovedTotalReviews.ShouldEqual(4);
            fromDb.NotApprovedTotalReviews.ShouldEqual(5);
            fromDb.SubjectToAcl.ShouldEqual(true);
            fromDb.LimitedToStores.ShouldEqual(true);
            fromDb.ShouldNotBeNull();
            fromDb.Sku.ShouldEqual("sku 1");
            fromDb.ManufacturerPartNumber.ShouldEqual("manufacturerPartNumber");
            fromDb.Gtin.ShouldEqual("gtin 1");
            fromDb.IsGiftCard.ShouldEqual(true);
            fromDb.GiftCardTypeId.ShouldEqual(1);
            fromDb.OverriddenGiftCardAmount.ShouldEqual(1);
            fromDb.IsDownload.ShouldEqual(true);
            fromDb.DownloadId.ShouldEqual(2);
            fromDb.UnlimitedDownloads.ShouldEqual(true);
            fromDb.MaxNumberOfDownloads.ShouldEqual(3);
            fromDb.DownloadExpirationDays.ShouldEqual(4);
            fromDb.DownloadActivationTypeId.ShouldEqual(5);
            fromDb.HasSampleDownload.ShouldEqual(true);
            fromDb.SampleDownloadId.ShouldEqual(6);
            fromDb.HasUserAgreement.ShouldEqual(true);
            fromDb.UserAgreementText.ShouldEqual("userAgreementText");
            fromDb.IsRecurring.ShouldEqual(true);
            fromDb.RecurringCycleLength.ShouldEqual(7);
            fromDb.RecurringCyclePeriodId.ShouldEqual(8);
            fromDb.RecurringTotalCycles.ShouldEqual(9);
            fromDb.IsRental.ShouldEqual(true);
            fromDb.RentalPriceLength.ShouldEqual(9);
            fromDb.RentalPricePeriodId.ShouldEqual(0);
            fromDb.IsShipEnabled.ShouldEqual(true);
            fromDb.IsFreeShipping.ShouldEqual(true);
            fromDb.ShipSeparately.ShouldEqual(true);
            fromDb.AdditionalShippingCharge.ShouldEqual(10.1M);
            fromDb.DeliveryDateId.ShouldEqual(5);
            fromDb.IsTaxExempt.ShouldEqual(true);
            fromDb.TaxCategoryId.ShouldEqual(11);
            fromDb.IsTelecommunicationsOrBroadcastingOrElectronicServices.ShouldEqual(true);
            fromDb.ManageInventoryMethodId.ShouldEqual(1);
            fromDb.UseMultipleWarehouses.ShouldEqual(true);
            fromDb.WarehouseId.ShouldEqual(6);
            fromDb.StockQuantity.ShouldEqual(13);
            fromDb.DisplayStockAvailability.ShouldEqual(true);
            fromDb.DisplayStockQuantity.ShouldEqual(true);
            fromDb.MinStockQuantity.ShouldEqual(14);
            fromDb.LowStockActivityId.ShouldEqual(15);
            fromDb.NotifyAdminForQuantityBelow.ShouldEqual(16);
            fromDb.BackorderModeId.ShouldEqual(17);
            fromDb.AllowBackInStockSubscriptions.ShouldEqual(true);
            fromDb.OrderMinimumQuantity.ShouldEqual(18);
            fromDb.OrderMaximumQuantity.ShouldEqual(19);
            fromDb.AllowedQuantities.ShouldEqual("1, 5,4,10,sdf");
            fromDb.AllowAddingOnlyExistingAttributeCombinations.ShouldEqual(true);
            fromDb.NotReturnable.ShouldEqual(true);
            fromDb.DisableBuyButton.ShouldEqual(true);
            fromDb.DisableWishlistButton.ShouldEqual(true);
            fromDb.AvailableForPreOrder.ShouldEqual(true);
            fromDb.PreOrderAvailabilityStartDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.CallForPrice.ShouldEqual(true);
            fromDb.Price.ShouldEqual(21.1M);
            fromDb.OldPrice.ShouldEqual(22.1M);
            fromDb.ProductCost.ShouldEqual(23.1M);
            fromDb.SpecialPrice.ShouldEqual(32.1M);
            fromDb.SpecialPriceStartDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 05));
            fromDb.SpecialPriceEndDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 06));
            fromDb.CustomerEntersPrice.ShouldEqual(true);
            fromDb.MinimumCustomerEnteredPrice.ShouldEqual(24.1M);
            fromDb.MaximumCustomerEnteredPrice.ShouldEqual(25.1M);
            fromDb.BasepriceEnabled.ShouldEqual(true);
            fromDb.BasepriceAmount.ShouldEqual(33.1M);
            fromDb.BasepriceUnitId.ShouldEqual(4);
            fromDb.BasepriceBaseAmount.ShouldEqual(34.1M);
            fromDb.BasepriceBaseUnitId.ShouldEqual(5);
            fromDb.MarkAsNew.ShouldEqual(true);
            fromDb.MarkAsNewStartDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 07));
            fromDb.MarkAsNewEndDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 08));
            fromDb.HasTierPrices.ShouldEqual(true);
            fromDb.HasDiscountsApplied.ShouldEqual(true);
            fromDb.Weight.ShouldEqual(26.1M);
            fromDb.Length.ShouldEqual(27.1M);
            fromDb.Width.ShouldEqual(28.1M);
            fromDb.Height.ShouldEqual(29.1M);
            fromDb.AvailableStartDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.AvailableEndDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 03));
            fromDb.RequireOtherProducts.ShouldEqual(true);
            fromDb.RequiredProductIds.ShouldEqual("1, 4,7 ,a,");
            fromDb.AutomaticallyAddRequiredProducts.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(30);
            fromDb.Published.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 03));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 04));
        }

        [Test]
        public void CanSaveAndLoadProductWithProductCategories()
        {
            var product = TestHelper.GetProduct();
            product.ProductCategories.Add(TestHelper.GetProductCategory(product));
           
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Product name 1");

            fromDb.ProductCategories.ShouldNotBeNull();
            (fromDb.ProductCategories.Count == 1).ShouldBeTrue();
            fromDb.ProductCategories.First().IsFeaturedProduct.ShouldEqual(true);

            fromDb.ProductCategories.First().Category.ShouldNotBeNull();
            fromDb.ProductCategories.First().Category.Name.ShouldEqual("Books");
        }

        [Test]
        public void CanSaveAndLoadProductWithProductManufacturers()
        {
            var product = TestHelper.GetProduct();
            product.ProductManufacturers.Add(TestHelper.GetProductManufacturer(product));

            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Product name 1");

            fromDb.ProductManufacturers.ShouldNotBeNull();
            (fromDb.ProductManufacturers.Count == 1).ShouldBeTrue();
            fromDb.ProductManufacturers.First().IsFeaturedProduct.ShouldEqual(true);

            fromDb.ProductManufacturers.First().Manufacturer.ShouldNotBeNull();
            fromDb.ProductManufacturers.First().Manufacturer.Name.ShouldEqual("Name");
        }

        [Test]
        public void CanSaveAndLoadProductWithProductPictures()
        {
            var product = TestHelper.GetProduct();
            product.ProductPictures.Add(TestHelper.GetProductPicture(product));

            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Product name 1");

            fromDb.ProductPictures.ShouldNotBeNull();
            (fromDb.ProductPictures.Count == 1).ShouldBeTrue();
            fromDb.ProductPictures.First().DisplayOrder.ShouldEqual(1);

            fromDb.ProductPictures.First().Picture.ShouldNotBeNull();
            fromDb.ProductPictures.First().Picture.MimeType.ShouldEqual(MimeTypes.ImagePJpeg);
        }

        [Test]
        public void CanSaveAndLoadProductWithProductTags()
        {
            var product = TestHelper.GetProduct();
            product.ProductTags.Add(TestHelper.GetProductTag());

            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Product name 1");

            fromDb.ProductTags.ShouldNotBeNull();
            (fromDb.ProductTags.Count == 1).ShouldBeTrue();
            fromDb.ProductTags.First().Name.ShouldEqual("Name 1");
        }

        [Test]
        public void CanSaveAndLoadProductWithTierPrices()
        {
            var product = TestHelper.GetProduct();
            product.TierPrices.Add(TestHelper.GetTierPrice(quantity: 1, price: 2));

            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Product name 1");

            fromDb.TierPrices.ShouldNotBeNull();
            (fromDb.TierPrices.Count == 1).ShouldBeTrue();
            fromDb.TierPrices.First().Quantity.ShouldEqual(1);
        }

        [Test]
        public void CanSaveAndLoadProductWithProductWarehouseInventory()
        {
            var product = TestHelper.GetProduct();
            product.ProductWarehouseInventory.Add(TestHelper.GetProductWarehouseInventory(product));
           
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Product name 1");

            fromDb.ProductWarehouseInventory.ShouldNotBeNull();
            (fromDb.ProductWarehouseInventory.Count == 1).ShouldBeTrue();
            fromDb.ProductWarehouseInventory.First().StockQuantity.ShouldEqual(3);
        }
    }
}
