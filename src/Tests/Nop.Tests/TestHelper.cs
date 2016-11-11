using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tasks;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Services.Discounts;
using Nop.Services.Messages;

namespace Nop.Tests
{
    public static class TestHelper
    {
        private static int _id = 1;

        #region Nop.Core.Caching

        /// <summary>
        /// Get memory cache manager
        /// </summary>
        /// <returns></returns>
        public static MemoryCacheManager GetMemoryCacheManager()
        {
            var cacheManager = new MemoryCacheManager();
            cacheManager.Set("some_key_1", 3, int.MaxValue);
            cacheManager.Set("some_key_2", 4, int.MaxValue);

            return cacheManager;
        }

        #endregion

        #region Nop.Core.Domain

        #region Nop.Core.Domain.Affiliates

        /// <summary>
        /// Get affiliate
        /// </summary>
        /// <returns></returns>
        public static Affiliate GetAffiliate()
        {
            return new Affiliate
            {
                Deleted = true,
                Active = true,
                Address = GetAddress(),
                AdminComment = "AdminComment 1",
                FriendlyUrlName = "FriendlyUrlName 1"
            };
        }

        #endregion

        #region Nop.Core.Domain.Blogs

        /// <summary>
        /// Get blog comment
        /// </summary>
        /// <returns></returns>
        public static BlogComment GetBlogComment()
        {
            return new BlogComment
            {
                CreatedOnUtc = new DateTime(2010, 01, 03),
                Customer = GetCustomer()
            };
        }

        /// <summary>
        /// Get blog post
        /// </summary>
        /// <returns></returns>
        public static BlogPost GetBlogPost()
        {
            var blogPost = new BlogPost
            {
                Title = "Title 1",
                Body = "Body 1",
                BodyOverview = "BodyOverview 1",
                AllowComments = true,
                CommentCount = 1,
                Tags = "Tags 1",
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                CreatedOnUtc = new DateTime(2010, 01, 03),
                MetaTitle = "MetaTitle 1",
                MetaDescription = "MetaDescription 1",
                MetaKeywords = "MetaKeywords 1",
                LimitedToStores = true,
                Language = GetLanguage()
            };

            blogPost.BlogComments.Add(GetBlogComment());

            return blogPost;
        }

        #endregion

        #region Nop.Core.Domain.Catalog

        /// <summary>
        /// Add tier prices to this product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="customerRole">Customer role; can by null</param>
        /// <returns></returns>
        public static Product AddTierPrices(this Product product, decimal price, int quantity,
            CustomerRole customerRole = null)
        {
            var tierPrice = GetTierPrice();
            tierPrice.Product = product;
            tierPrice.CustomerRole = customerRole;
            tierPrice.Price = price;
            tierPrice.Quantity = quantity;
            product.TierPrices.Add(tierPrice);
            return product;
        }

        /// <summary>
        /// Get back in stock subscription
        /// </summary>
        /// <returns></returns>
        public static BackInStockSubscription GetBackInStockSubscription()
        {
            return new BackInStockSubscription
            {
                Product = GetProduct(),
                Customer = GetCustomer(),
                CreatedOnUtc = new DateTime(2010, 01, 02)
            };
        }

        /// <summary>
        /// Get category
        /// </summary>
        /// <returns></returns>
        public static Category GetCategory()
        {
            return new Category
            {
                Name = "Books",
                Description = "Description 1",
                CategoryTemplateId = 1,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                ParentCategoryId = 2,
                PictureId = 3,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PriceRanges = "1-3;",
                ShowOnHomePage = false,
                IncludeInTopMenu = true,
                Published = true,
                SubjectToAcl = true,
                LimitedToStores = true,
                Deleted = false,
                DisplayOrder = 5,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        /// <summary>
        /// Get category template
        /// </summary>
        /// <returns></returns>
        public static CategoryTemplate GetCategoryTemplate()
        {
            return new CategoryTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

        /// <summary>
        /// Get manufacturer
        /// </summary>
        /// <returns></returns>
        public static Manufacturer GetManufacturer()
        {
            return new Manufacturer
            {
                Name = "Name",
                Description = "Description 1",
                ManufacturerTemplateId = 1,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                PictureId = 3,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PriceRanges = "1-3;",
                Published = true,
                SubjectToAcl = true,
                LimitedToStores = true,
                Deleted = false,
                DisplayOrder = 5,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        /// <summary>
        /// Get manufacturer template
        /// </summary>
        /// <returns></returns>
        public static ManufacturerTemplate GetManufacturerTemplate()
        {
            return new ManufacturerTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

        /// <summary>
        /// Get predefined product attribute value
        /// </summary>
        /// <returns></returns>
        public static PredefinedProductAttributeValue GetPredefinedProductAttributeValue()
        {
            return new PredefinedProductAttributeValue
            {
                Name = "Name 1",
                PriceAdjustment = 1.1M,
                WeightAdjustment = 2.1M,
                Cost = 3.1M,
                IsPreSelected = true,
                DisplayOrder = 3,
                ProductAttribute = GetProductAttribute()
            };
        }

        /// <summary>
        /// Get product
        /// </summary>
        /// <returns></returns>
        public static Product GetProduct()
        {
            return new Product
            {
                Id = 1,
                Name = "Product name 1",
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 01),
                RequiredProductIds = "1, 4,7 ,a,",
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 03),
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
                ProductType = ProductType.GroupedProduct,
                ParentGroupedProductId = 2,
                VisibleIndividually = true,
                ProductAvailabilityRangeId = 1,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                ShortDescription = "ShortDescription 1",
                FullDescription = "FullDescription 1",
                AdminComment = "AdminComment 1",
                VendorId = 1,
                ProductTemplateId = 2,
                ShowOnHomePage = false,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                AllowCustomerReviews = true,
                ApprovedRatingSum = 2,
                NotApprovedRatingSum = 3,
                ApprovedTotalReviews = 4,
                NotApprovedTotalReviews = 5,
                SubjectToAcl = true,
                LimitedToStores = true,
                Sku = "sku 1",
                ManufacturerPartNumber = "manufacturerPartNumber",
                Gtin = "gtin 1",
                IsGiftCard = true,
                GiftCardTypeId = 1,
                OverriddenGiftCardAmount = 1,
                IsDownload = true,
                DownloadId = 2,
                UnlimitedDownloads = true,
                MaxNumberOfDownloads = 3,
                DownloadExpirationDays = 4,
                DownloadActivationTypeId = 5,
                HasSampleDownload = true,
                SampleDownloadId = 6,
                HasUserAgreement = true,
                UserAgreementText = "userAgreementText",
                IsRecurring = true,
                RecurringCycleLength = 7,
                RecurringCyclePeriodId = 8,
                RecurringTotalCycles = 9,
                IsRental = true,
                RentalPriceLength = 9,
                RentalPricePeriodId = 0,
                RentalPricePeriod = 0,
                IsShipEnabled = true,
                IsFreeShipping = true,
                ShipSeparately = true,
                AdditionalShippingCharge = 10.1M,
                DeliveryDateId = 5,
                IsTaxExempt = true,
                TaxCategoryId = 11,
                IsTelecommunicationsOrBroadcastingOrElectronicServices = true,
                ManageInventoryMethodId = 1,
                UseMultipleWarehouses = true,
                WarehouseId = 6,
                StockQuantity = 13,
                DisplayStockAvailability = true,
                DisplayStockQuantity = true,
                MinStockQuantity = 14,
                LowStockActivityId = 15,
                NotifyAdminForQuantityBelow = 16,
                BackorderModeId = 17,
                AllowBackInStockSubscriptions = true,
                OrderMinimumQuantity = 18,
                OrderMaximumQuantity = 19,
                AllowedQuantities = "1, 5,4,10,sdf",
                AllowAddingOnlyExistingAttributeCombinations = true,
                NotReturnable = true,
                DisableBuyButton = true,
                DisableWishlistButton = true,
                AvailableForPreOrder = true,
                PreOrderAvailabilityStartDateTimeUtc = new DateTime(2010, 01, 01),
                CallForPrice = true,
                Price = 21.1M,
                OldPrice = 22.1M,
                ProductCost = 23.1M,
                SpecialPrice = 32.1M,
                SpecialPriceStartDateTimeUtc = new DateTime(2010, 01, 05),
                SpecialPriceEndDateTimeUtc = new DateTime(2010, 01, 06),
                CustomerEntersPrice = true,
                MinimumCustomerEnteredPrice = 24.1M,
                MaximumCustomerEnteredPrice = 25.1M,
                BasepriceEnabled = true,
                BasepriceAmount = 33.1M,
                BasepriceUnitId = 4,
                BasepriceBaseAmount = 34.1M,
                BasepriceBaseUnitId = 5,
                MarkAsNew = true,
                MarkAsNewStartDateTimeUtc = new DateTime(2010, 01, 07),
                MarkAsNewEndDateTimeUtc = new DateTime(2010, 01, 08),
                HasTierPrices = true,
                HasDiscountsApplied = true,
                Weight = 26.1M,
                Length = 27.1M,
                Width = 28.1M,
                Height = 29.1M,
                RequireOtherProducts = true,
                AutomaticallyAddRequiredProducts = true,
                DisplayOrder = 30,
                Published = true,
                Deleted = false
            };
        }

        /// <summary>
        /// Get product attribute
        /// </summary>
        /// <returns></returns>
        public static ProductAttribute GetProductAttribute()
        {
            return new ProductAttribute
            {
                Id = 1,
                Name = "Name 1",
                Description = "Description 1"
            };
        }

        /// <summary>
        /// Get product attribute combination
        /// </summary>
        /// <returns></returns>
        public static ProductAttributeCombination GetProductAttributeCombination()
        {
            return new ProductAttributeCombination
            {
                AttributesXml = "Some XML",
                StockQuantity = 2,
                AllowOutOfStockOrders = true,
                Sku = "Sku1",
                ManufacturerPartNumber = "ManufacturerPartNumber1",
                Gtin = "Gtin1",
                OverriddenPrice = 0.01M,
                NotifyAdminForQuantityBelow = 3,
                Product = GetProduct()
            };
        }

        /// <summary>
        /// Get product attribute mapping
        /// </summary>
        /// <returns></returns>
        public static ProductAttributeMapping GetProductAttributeMapping()
        {
            var product = GetProduct();
            var productAttribute = GetProductAttribute();

            return new ProductAttributeMapping
            {
                Id = 1,
                TextPrompt = "TextPrompt 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.DropdownList,
                DisplayOrder = 1,
                ValidationMinLength = 2,
                ValidationMaxLength = 3,
                ValidationFileAllowedExtensions = "ValidationFileAllowedExtensions 1",
                ValidationFileMaximumSize = 4,
                DefaultValue = "DefaultValue 1",
                ConditionAttributeXml = "ConditionAttributeXml 1",
                Product = product,
                ProductId = product.Id,
                ProductAttribute = productAttribute,
                ProductAttributeId = productAttribute.Id
            };
        }

        /// <summary>
        /// Get product attribute value
        /// </summary>
        /// <returns></returns>
        public static ProductAttributeValue GetProductAttributeValue()
        {
            var productAttributeMapping = GetProductAttributeMapping();
            return new ProductAttributeValue
            {
                Id = 1,
                AttributeValueType = AttributeValueType.AssociatedToProduct,
                AssociatedProductId = 10,
                Name = "Name 1",
                ColorSquaresRgb = "12FF33",
                ImageSquaresPictureId = 1,
                PriceAdjustment = 1.1M,
                WeightAdjustment = 2.1M,
                Cost = 3.1M,
                Quantity = 2,
                IsPreSelected = true,
                DisplayOrder = 3,
                ProductAttributeMapping = productAttributeMapping,
                ProductAttributeMappingId = productAttributeMapping.Id
            };
        }

        /// <summary>
        /// Get product category
        /// </summary>
        /// <returns></returns>
        public static ProductCategory GetProductCategory()
        {
            return new ProductCategory
            {
                IsFeaturedProduct = true,
                DisplayOrder = 1,
                Product = GetProduct(),
                Category = GetCategory()
            };
        }

        /// <summary>
        /// Get product manufacturer
        /// </summary>
        /// <returns></returns>
        public static ProductManufacturer GetProductManufacturer()
        {
            return new ProductManufacturer
            {
                IsFeaturedProduct = true,
                DisplayOrder = 1,
                Product = GetProduct(),
                Manufacturer = GetManufacturer()
            };
        }

        /// <summary>
        /// Get product picture
        /// </summary>
        /// <returns></returns>
        public static ProductPicture GetProductPicture()
        {
            return new ProductPicture
            {
                DisplayOrder = 1,
                Product = GetProduct(),
                Picture = GetPicture()
            };
        }

        /// <summary>
        /// Get product specification attribute
        /// </summary>
        /// <returns></returns>
        public static ProductSpecificationAttribute GetProductSpecificationAttribute()
        {
            return new ProductSpecificationAttribute
            {
                AttributeType = SpecificationAttributeType.Hyperlink,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                Product = GetProduct(),
                SpecificationAttributeOption = GetSpecificationAttributeOption()
            };
        }

        /// <summary>
        /// Get product tag
        /// </summary>
        /// <returns></returns>
        public static ProductTag GetProductTag()
        {
            return new ProductTag
            {
                Name = "Name 1"
            };
        }

        /// <summary>
        /// Get product template
        /// </summary>
        /// <returns></returns>
        public static ProductTemplate GetProductTemplate()
        {
            return new ProductTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

        /// <summary>
        /// Get product warehouse inventory
        /// </summary>
        /// <returns></returns>
        public static ProductWarehouseInventory GetProductWarehouseInventory()
        {
            var warehouse = GetWarehouse();

            var productWarehouseInventory = new ProductWarehouseInventory
            {
                Product = GetProduct(),

                StockQuantity = 3,
                ReservedQuantity = 4,
                Warehouse = warehouse,
                WarehouseId = warehouse.Id
            };

            return productWarehouseInventory;
        }

        /// <summary>
        /// Get specification attribute
        /// </summary>
        /// <returns></returns>
        public static SpecificationAttribute GetSpecificationAttribute()
        {
            return new SpecificationAttribute
            {
                Name = "SpecificationAttribute name 1",
                DisplayOrder = 2
            };
        }

        /// <summary>
        /// Get specification attribute option
        /// </summary>
        /// <returns></returns>
        public static SpecificationAttributeOption GetSpecificationAttributeOption()
        {
            var specificationAttributeOption = new SpecificationAttributeOption
            {
                Name = "SpecificationAttributeOption name 1",
                DisplayOrder = 1,
                ColorSquaresRgb = "ColorSquaresRgb 2",
                SpecificationAttribute = GetSpecificationAttribute()

            };

            return specificationAttributeOption;
        }

        /// <summary>
        /// Get tier price
        /// </summary>
        /// <returns></returns>
        public static TierPrice GetTierPrice()
        {
            var tierPrice = new TierPrice
            {
                Id = 1,
                StoreId = 1,
                Quantity = 1,
                Price = 2.1M
            };
            
            return tierPrice;
        }

        #endregion

        #region Nop.Core.Domain.Common

        /// <summary>
        /// Get address
        /// </summary>
        /// <returns></returns>
        public static Address GetAddress()
        {
            var country = GetCountry();
            var state = GetStateProvince();
            state.Country = country;

            return new Address
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "Email 1",
                Company = "Company 1",
                Country = country,
                CountryId = country.Id,
                StateProvince = state,
                StateProvinceId = state.Id,
                City = "City 1",
                Address1 = "Address1",
                Address2 = "Address2",
                ZipPostalCode = "ZipPostalCode 1",
                PhoneNumber = "PhoneNumber 1",
                FaxNumber = "FaxNumber 1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                CustomAttributes = "CustomAttributes 1"
            };
        }

        /// <summary>
        /// Get address attribute
        /// </summary>
        /// <returns></returns>
        public static AddressAttribute GetAddressAttribute()
        {
            var addressAttribute = new AddressAttribute
            {
                Name = "Name 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };

            return addressAttribute;
        }

        /// <summary>
        /// Get address attribute value
        /// </summary>
        /// <returns></returns>
        public static AddressAttributeValue GetAddressAttributeValue()
        {
            var addess = new AddressAttributeValue
            {
                Name = "Name 2",
                IsPreSelected = true,
                DisplayOrder = 1,
                AddressAttribute = GetAddressAttribute()
            };
            
            return addess;
        }

        /// <summary>
        /// Get generic attribute
        /// </summary>
        /// <returns></returns>
        public static GenericAttribute GetGenericAttribute()
        {
            return new GenericAttribute
            {
                EntityId = 1,
                Key = "Key 1",
                KeyGroup = "Customer",
                Value = "Value 1",
                StoreId = 2
            };
        }

        /// <summary>
        /// Get aearch term
        /// </summary>
        /// <returns></returns>
        public static SearchTerm GetSearchTerm()
        {
            return new SearchTerm
            {
                Keyword = "Keyword 1",
                StoreId = 1,
                Count = 2
            };
        }

        #endregion

        #region Nop.Core.Domain.Configuration

        /// <summary>
        /// Get setting
        /// </summary>
        /// <returns></returns>
        public static Setting GetSetting()
        {
            return new Setting
            {
                Name = "Setting1",
                Value = "Value1",
                StoreId = 1
            };
        }

        #endregion

        #region Nop.Core.Domain.Customers

        /// <summary>
        /// Get customer
        /// </summary>
        /// <param name="systemNames">Customer role's system names</param>
        /// <returns></returns>
        public static Customer GetCustomer(params string[] systemNames)
        {
            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02),
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                Username = "a@b.com",
                Password = "password",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Email = "a@b.com",
                IsTaxExempt = true,
                AffiliateId = 1,
                VendorId = 2,
                HasShoppingCartItems = true,
                IsSystemAccount = true,
                SystemName = "SystemName 1",
                LastIpAddress = "192.168.1.1",

                LastLoginDateUtc = new DateTime(2010, 01, 02),
            };

            foreach (var systemName in systemNames)
            {
                customer.CustomerRoles.Add(GetCustomerRole(systemName));
            }

            customer.ShoppingCartItems.Add(GetShoppingCartItem());
            return customer;
        }

        /// <summary>
        /// Get customer attribute
        /// </summary>
        /// <returns></returns>
        public static CustomerAttribute GetCustomerAttribute()
        {
            var customerAttribute = new CustomerAttribute
            {
                Name = "Name 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };

            return customerAttribute;
        }

        /// <summary>
        /// Get customer attribute value
        /// </summary>
        /// <returns></returns>
        public static CustomerAttributeValue GetCustomerAttributeValue()
        {
            var customerAttributeValue = new CustomerAttributeValue
            {
                Name = "Name 2",
                IsPreSelected = true,
                DisplayOrder = 1,
                CustomerAttribute = GetCustomerAttribute()
            };

            customerAttributeValue.CustomerAttribute.CustomerAttributeValues.Clear();

            return customerAttributeValue;
        }
        
        /// <summary>
        /// Get customer role
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns></returns>
        public static CustomerRole GetCustomerRole(string systemName)
        {
            return new CustomerRole
            {
                Active = true,
                Name = systemName.Replace(" system", string.Empty),
                SystemName = systemName,
                FreeShipping = true,
                TaxExempt = true,
                IsSystemRole = true,
                PurchasedWithProductId = 1
            };
        }

        /// <summary>
        /// Get external authentication record
        /// </summary>
        /// <returns></returns>
        public static ExternalAuthenticationRecord GetExternalAuthenticationRecord()
        {
            return new ExternalAuthenticationRecord
            {
                ExternalIdentifier = "ExternalIdentifier 1",
                ExternalDisplayIdentifier = "ExternalDisplayIdentifier 1",
                OAuthToken = "OAuthToken 1",
                OAuthAccessToken = "OAuthAccessToken 1",
                ProviderSystemName = "ProviderSystemName 1",
                Email = "Email 1"
            };
        }

        /// <summary>
        /// Get reward points history
        /// </summary>
        /// <returns></returns>
        public static RewardPointsHistory GetRewardPointsHistory()
        {
            return new RewardPointsHistory
            {
                Customer = GetCustomer(),
                StoreId = 1,
                Points = 2,
                Message = "Points for registration",
                PointsBalance = 3,
                UsedAmount = 3.1M,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        #endregion

        #region Nop.Core.Domain.Directory

        /// <summary>
        /// Get country
        /// </summary>
        /// <returns></returns>
        public static Country GetCountry()
        {
            return new Country
            {
                Id = _id++,
                Name = "United States",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                SubjectToVat = true,
                Published = true,
                DisplayOrder = 1,
                LimitedToStores = true
            };
        }

        /// <summary>
        /// Get currency
        /// </summary>
        /// <returns></returns>
        public static Currency GetCurrency()
        {
            return new Currency
            {
                Id = 1,
                Name = "US Dollar",
                CurrencyCode = "USD",
                Rate = 1.1M,
                DisplayLocale = "en-US",
                CustomFormatting = "",
                LimitedToStores = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };
        }

        /// <summary>
        /// Get measure dimension
        /// </summary>
        /// <returns></returns>
        public static MeasureDimension GetMeasureDimension()
        {
            return new MeasureDimension
            {
                Name = "inch(es)",
                SystemKeyword = "inches",
                Ratio = 1.12345678M,
                DisplayOrder = 2
            };
        }

        /// <summary>
        /// Get measure dimensions
        /// </summary>
        /// <param name="ratios">Ratios</param>
        /// <returns></returns>
        public static IList<MeasureDimension> GetMeasureDimensions(params decimal[] ratios)
        {
            var measureDimensions = new List<MeasureDimension>();

            var id = 1;

            measureDimensions.AddRange(ratios.Select(r => new MeasureDimension
            {
                Id = id,
                DisplayOrder = id++,
                Ratio = r
            }));

            return measureDimensions;
        }

        /// <summary>
        /// Get measure weight
        /// </summary>
        /// <returns></returns>
        public static MeasureWeight GetMeasureWeight()
        {
            return new MeasureWeight
            {
                Name = "ounce(s)",
                SystemKeyword = "ounce",
                Ratio = 1.12345678M,
                DisplayOrder = 2,
            };
        }

        /// <summary>
        /// Get measure weights
        /// </summary>
        /// <param name="ratios">Ratios</param>
        /// <returns></returns>
        public static IList<MeasureWeight> GetMeasureWeights(params decimal[] ratios)
        {
            var measureWeights = new List<MeasureWeight>();

            var id = 1;

            measureWeights.AddRange(ratios.Select(r => new MeasureWeight
            {
                Id = id,
                DisplayOrder = id++,
                Ratio = r
            }));

            return measureWeights;
        }

        /// <summary>
        /// Get state province
        /// </summary>
        /// <returns></returns>
        public static StateProvince GetStateProvince()
        {
            var stateProvince = new StateProvince
            {
                Id = 0,
                Name = "Louisiana",
                Abbreviation = "LA",
                DisplayOrder = 1,
                Published = true
            };

            return stateProvince;
        }

        #endregion

        #region Nop.Core.Domain.Discounts

        /// <summary>
        /// Get discount
        /// </summary>
        /// <returns></returns>
        public static Discount GetDiscount()
        {
            return new Discount
            {
                DiscountType = DiscountType.AssignedToCategories,
                Name = "Discount 1",
                UsePercentage = true,
                DiscountPercentage = 1.1M,
                DiscountAmount = 2.1M,
                MaximumDiscountAmount = 208.1M,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                RequiresCouponCode = true,
                CouponCode = "SecretCode",
                IsCumulative = true,
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 3,
                MaximumDiscountedQuantity = 4,
                AppliedToSubCategories = true
            };
        }

        /// <summary>
        /// Get discount requirement
        /// </summary>
        /// <returns></returns>
        public static DiscountRequirement GetDiscountRequirement()
        {
            return new DiscountRequirement
            {
                DiscountRequirementRuleSystemName = "BillingCountryIs"
            };
        }

        /// <summary>
        /// Get discount usage history
        /// </summary>
        /// <returns></returns>
        public static DiscountUsageHistory GetDiscountUsageHistory()
        {
            return new DiscountUsageHistory
            {
                Discount = GetDiscount(),
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        #endregion

        #region Nop.Core.Domain.Forums

        /// <summary>
        /// Get forum
        /// </summary>
        /// <returns></returns>
        public static Forum GetForum()
        {
            var forum = new Forum
            {
                Name = "Forum 1",
                Description = "Forum 1 Description",
                DisplayOrder = 10,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                NumPosts = 25,
                NumTopics = 15
            };

            return forum;
        }

        /// <summary>
        /// Get forum group
        /// </summary>
        /// <returns></returns>
        public static ForumGroup GetForumGroup()
        {
            return new ForumGroup
            {
                Name = "Forum Group 1",
                DisplayOrder = 1,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        /// <summary>
        /// Get forum post
        /// </summary>
        /// <returns></returns>
        public static ForumPost GetForumPost()
        {
            var forumPost = new ForumPost
            {
                Text = "Forum Post 1 Text",
                IPAddress = "127.0.0.1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
            
            return forumPost;
        }

        /// <summary>
        /// Get forum subscription
        /// </summary>
        /// <returns></returns>
        public static ForumSubscription GetForumSubscription()
        {
            var forumSubscription = new ForumSubscription
            {
                CreatedOnUtc = DateTime.UtcNow,
                SubscriptionGuid = new Guid("11111111-2222-3333-4444-555555555555")
            };
            
            return forumSubscription;
        }

        /// <summary>
        /// Get forum topic
        /// </summary>
        /// <returns></returns>
        public static ForumTopic GetForumTopic()
        {
            var forumTopic = new ForumTopic
            {
                Subject = "Forum Topic 1",
                TopicTypeId = (int) ForumTopicType.Sticky,
                Views = 123,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                NumPosts = 100
            };
            
            return forumTopic;
        }

        /// <summary>
        /// Get private message
        /// </summary>
        /// <returns></returns>
        public static PrivateMessage GetPrivateMessage()
        {
            var privateMessage = new PrivateMessage
            {
                Subject = "Private Message 1 Subject",
                Text = "Private Message 1 Text",
                IsDeletedByAuthor = false,
                IsDeletedByRecipient = false,
                IsRead = false,
                CreatedOnUtc = DateTime.UtcNow
            };
            
            return privateMessage;
        }

        #endregion

        #region Nop.Core.Domain.Localization

        /// <summary>
        /// Get language
        /// </summary>
        /// <returns></returns>
        public static Language GetLanguage()
        {
            return new Language
            {
                Name = "English",
                LanguageCulture = "en-Us",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Rtl = true,
                DefaultCurrencyId = 1,
                Published = true,
                LimitedToStores = true,
                DisplayOrder = 1
            };
        }

        /// <summary>
        /// Get locale string resource
        /// </summary>
        /// <returns></returns>
        public static LocaleStringResource GetLocaleStringResource()
        {
            return new LocaleStringResource
            {
                ResourceName = "ResourceName1",
                ResourceValue = "ResourceValue2"
            };
        }

        /// <summary>
        /// Get localized property
        /// </summary>
        /// <returns></returns>
        public static LocalizedProperty GetLocalizedProperty()
        {
            return new LocalizedProperty
            {
                EntityId = 1,
                LocaleKeyGroup = "LocaleKeyGroup 1",
                LocaleKey = "LocaleKey 1",
                LocaleValue = "LocaleValue 1"
            };
        }

        #endregion

        #region Nop.Core.Domain.Logging

        /// <summary>
        /// Get activity log
        /// </summary>
        /// <returns></returns>
        public static ActivityLog GetActivityLog()
        {
            return new ActivityLog
            {
                Id = 1,
                ActivityLogType = GetActivityLogType(),
                Customer = GetCustomer()
            };
        }

        /// <summary>
        /// Get activity log type
        /// </summary>
        /// <returns></returns>
        public static ActivityLogType GetActivityLogType()
        {
            return new ActivityLogType
            {
                SystemKeyword = "SystemKeyword 1",
                Name = "Name 1",
                Enabled = true
            };
        }

        /// <summary>
        /// Get log
        /// </summary>
        /// <returns></returns>
        public static Log GetLog()
        {
            return new Log
            {
                LogLevel = LogLevel.Error,
                ShortMessage = "ShortMessage1",
                FullMessage = "FullMessage1",
                IpAddress = "127.0.0.1",
                PageUrl = "http://www.someUrl1.com",
                ReferrerUrl = "http://www.someUrl2.com",
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        #endregion

        #region Nop.Core.Domain.Media

        /// <summary>
        /// Get download
        /// </summary>
        /// <returns></returns>
        public static Download GetDownload()
        {
            return new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = true,
                DownloadUrl = "http://www.someUrl.com/file.zip",
                DownloadBinary = new byte[] { 1, 2, 3 },
                ContentType = MimeTypes.ApplicationXZipCo,
                Filename = "file",
                Extension = ".zip",
                IsNew = true
            };
        }

        /// <summary>
        /// Get picture
        /// </summary>
        /// <returns></returns>
        public static Picture GetPicture()
        {
            return new Picture
            {
                PictureBinary = new byte[] { 1, 2, 3 },
                MimeType = MimeTypes.ImagePJpeg,
                IsNew = true,
                SeoFilename = "seo filename 1",
                AltAttribute = "AltAttribute 1",
                TitleAttribute = "TitleAttribute 1"
            };
        }

        #endregion

        #region Nop.Core.Domain.Messages

        /// <summary>
        /// Get campaign
        /// </summary>
        /// <returns></returns>
        public static Campaign GetCampaign()
        {
            return new Campaign
            {
                Name = "Name 1",
                Subject = "Subject 1",
                Body = "Body 1",
                CreatedOnUtc = new DateTime(2010, 01, 02),
                DontSendBeforeDateUtc = new DateTime(2016, 2, 23),
                CustomerRoleId = 1,
                StoreId = 1
            };
        }

        /// <summary>
        /// Get email account
        /// </summary>
        /// <returns></returns>
        public static EmailAccount GetEmailAccount()
        {
            return new EmailAccount
            {
                Email = "admin@yourstore.com",
                DisplayName = "Administrator",
                Host = "127.0.0.1",
                Port = 125,
                Username = "John",
                Password = "111",
                EnableSsl = true,
                UseDefaultCredentials = true
            };
        }

        /// <summary>
        /// Get message template
        /// </summary>
        /// <returns></returns>
        public static MessageTemplate GetMessageTemplate()
        {
            return new MessageTemplate
            {
                Name = "Template1",
                BccEmailAddresses = "Bcc",
                Subject = "Subj",
                Body = "Some text",
                IsActive = true,
                AttachedDownloadId = 3,
                EmailAccountId = 1,
                LimitedToStores = true,
                DelayBeforeSend = 2,
                DelayPeriodId = 0
            };
        }

        /// <summary>
        /// Get news letter subscription
        /// </summary>
        /// <returns></returns>
        public static NewsLetterSubscription GetNewsLetterSubscription()
        {
            return new NewsLetterSubscription
            {
                Email = "me@yourstore.com",
                NewsLetterSubscriptionGuid = Guid.NewGuid(),
                CreatedOnUtc = new DateTime(2010, 01, 01),
                StoreId = 1,
                Active = true
            };
        }

        /// <summary>
        /// Get queued email
        /// </summary>
        /// <returns></returns>
        public static QueuedEmail GetQueuedEmail()
        {
            return new QueuedEmail
            {
                PriorityId = 5,
                From = "From",
                FromName = "FromName",
                To = "To",
                ToName = "ToName",
                ReplyTo = "ReplyTo",
                ReplyToName = "ReplyToName",
                CC = "CC",
                Bcc = "Bcc",
                Subject = "Subject",
                Body = "Body",
                AttachmentFilePath = "some file path",
                AttachmentFileName = "some file name",
                AttachedDownloadId = 3,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                SentTries = 5,
                SentOnUtc = new DateTime(2010, 02, 02),
                DontSendBeforeDateUtc = new DateTime(2016, 2, 23)
            };
        }

        #endregion

        #region Nop.Core.Domain.News

        /// <summary>
        /// Get news comment
        /// </summary>
        /// <returns></returns>
        public static NewsComment GetNewsComment()
        {
            return new NewsComment
            {
                CommentText = "Comment text 1",
                CreatedOnUtc = new DateTime(2010, 01, 03),
                Customer = GetCustomer()
            };
        }

        /// <summary>
        /// Get news item
        /// </summary>
        /// <returns></returns>
        public static NewsItem GetNewsItem()
        {
            return new NewsItem
            {
                Title = "Title 1",
                Short = "Short 1",
                Full = "Full 1",
                Published = true,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                AllowComments = true,
                CommentCount = 1,
                LimitedToStores = true,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                MetaTitle = "MetaTitle 1",
                MetaDescription = "MetaDescription 1",
                MetaKeywords = "MetaKeywords 1",
                Language = GetLanguage()
            };
        }

        #endregion

        #region Nop.Core.Domain.Orders

        /// <summary>
        /// Get checkout attribute
        /// </summary>
        /// <returns></returns>
        public static CheckoutAttribute GetCheckoutAttribute()
        {
            return new CheckoutAttribute
            {
                Id = 1,
                Name = "Name 1",
                TextPrompt = "TextPrompt 1",
                IsRequired = true,
                ShippableProductRequired = true,
                IsTaxExempt = true,
                TaxCategoryId = 1,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2,
                LimitedToStores = true,
                ValidationMinLength = 3,
                ValidationMaxLength = 4,
                ValidationFileAllowedExtensions = "ValidationFileAllowedExtensions 1",
                ValidationFileMaximumSize = 5,
                DefaultValue = "DefaultValue 1",
                ConditionAttributeXml = "ConditionAttributeXml 1"
            };
        }

        /// <summary>
        /// Get checkout attribute value
        /// </summary>
        /// <returns></returns>
        public static CheckoutAttributeValue GetCheckoutAttributeValue()
        {
            return  new CheckoutAttributeValue
            {
                Id = 1,
                Name = "Name 2",
                PriceAdjustment = 1,
                WeightAdjustment = 2,
                IsPreSelected = true,
                DisplayOrder = 3,
                ColorSquaresRgb = "#112233"
            };
        }

        /// <summary>
        /// Get gift card
        /// </summary>
        /// <returns></returns>
        public static GiftCard GetGiftCard()
        {
            var giftCard = new GiftCard
            {
                Amount = 100,
                GiftCardType = GiftCardType.Physical,
                GiftCardCouponCode = "Secret",
                RecipientName = "RecipientName 1",
                RecipientEmail = "a@b.c",
                SenderName = "SenderName 1",
                SenderEmail = "d@e.f",
                Message = "Message 1",
                IsRecipientNotified = true,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                IsGiftCardActivated = true
            };

            return giftCard;
        }

        /// <summary>
        /// Get gift card usage history
        /// </summary>
        /// <returns></returns>
        public static GiftCardUsageHistory GetGiftCardUsageHistory()
        {
            return new GiftCardUsageHistory
            {
                UsedValue = 1.1M,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        /// <summary>
        /// Get order
        /// </summary>
        /// <returns></returns>
        public static Order GetOrder()
        {
            return new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetCustomer(),
                BillingAddress = GetAddress(),
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                StoreId = 1,
                OrderStatus = OrderStatus.Complete,
                ShippingStatus = ShippingStatus.Shipped,
                PaymentStatus = PaymentStatus.Paid,
                PaymentMethodSystemName = "PaymentMethodSystemName1",
                CustomerCurrencyCode = "RUR",
                CurrencyRate = 1.1M,
                CustomerTaxDisplayType = TaxDisplayType.ExcludingTax,
                VatNumber = "123456789",
                OrderSubtotalInclTax = 2.1M,
                OrderSubtotalExclTax = 3.1M,
                OrderSubTotalDiscountInclTax = 4.1M,
                OrderSubTotalDiscountExclTax = 5.1M,
                OrderShippingInclTax = 6.1M,
                OrderShippingExclTax = 7.1M,
                PaymentMethodAdditionalFeeInclTax = 8.1M,
                PaymentMethodAdditionalFeeExclTax = 9.1M,
                TaxRates = "1,3,5,7",
                OrderTax = 10.1M,
                OrderDiscount = 11.1M,
                OrderTotal = 12.1M,
                RefundedAmount = 13.1M,
                RewardPointsWereAdded = true,
                CheckoutAttributeDescription = "CheckoutAttributeDescription1",
                CheckoutAttributesXml = "CheckoutAttributesXml1",
                CustomerLanguageId = 14,
                CustomerIp = "CustomerIp1",
                AllowStoringCreditCardNumber = true,
                CardType = "Visa",
                CardName = "John Smith",
                CardNumber = "4111111111111111",
                MaskedCreditCardNumber = "************1111",
                CardCvv2 = "123",
                CardExpirationMonth = "12",
                CardExpirationYear = "2010",
                AuthorizationTransactionId = "AuthorizationTransactionId1",
                AuthorizationTransactionCode = "AuthorizationTransactionCode1",
                AuthorizationTransactionResult = "AuthorizationTransactionResult1",
                CaptureTransactionId = "CaptureTransactionId1",
                CaptureTransactionResult = "CaptureTransactionResult1",
                SubscriptionTransactionId = "SubscriptionTransactionId1",
                PaidDateUtc = new DateTime(2010, 01, 01),
                PickupAddress = GetAddress(),
                ShippingMethod = "ShippingMethod1",
                ShippingRateComputationMethodSystemName = "ShippingRateComputationMethodSystemName1",
                PickUpInStore = true,
                CustomValuesXml = "CustomValuesXml1"
            };
        }

        /// <summary>
        /// Get order item
        /// </summary>
        /// <returns></returns>
        public static OrderItem GetOrderItem()
        {
            return new OrderItem
            {
                Product = GetProduct(),
                Quantity = 1,
                UnitPriceInclTax = 1.1M,
                UnitPriceExclTax = 2.1M,
                PriceInclTax = 3.1M,
                PriceExclTax = 4.1M,
                DiscountAmountInclTax = 5.1M,
                DiscountAmountExclTax = 6.1M,
                OriginalProductCost = 7.1M,
                AttributeDescription = "AttributeDescription1",
                AttributesXml = "AttributesXml1",
                DownloadCount = 7,
                IsDownloadActivated = true,
                LicenseDownloadId = 8,
                ItemWeight = 9.87M,
                RentalStartDateUtc = new DateTime(2010, 01, 01),
                RentalEndDateUtc = new DateTime(2010, 01, 02)
            };
        }

        /// <summary>
        /// Get order note
        /// </summary>
        /// <returns></returns>
        public static OrderNote GetOrderNote()
        {
            return new OrderNote
            {
                Note = "Note1",
                DownloadId = 1,
                DisplayToCustomer = true,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        /// <summary>
        /// Get recurring payment
        /// </summary>
        /// <returns></returns>
        public static RecurringPayment GetRecurringPayment()
        {
            return new RecurringPayment
            {
                CycleLength = 2,
                CyclePeriod = RecurringProductCyclePeriod.Days,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = true,
                Deleted = false
            };
        }

        /// <summary>
        /// Get recurring payment history
        /// </summary>
        /// <returns></returns>
        public static RecurringPaymentHistory GetRecurringPaymentHistory()
        {
            return new RecurringPaymentHistory
            {
                CreatedOnUtc = new DateTime(2010, 01, 03)
            };
        }

        /// <summary>
        /// Get return request
        /// </summary>
        /// <returns></returns>
        public static ReturnRequest GetReturnRequest()
        {
            return new ReturnRequest
            {
                CustomNumber = "CustomNumber 1",
                StoreId = 1,
                Customer = GetCustomer(),
                Quantity = 2,
                ReasonForReturn = "Wrong product",
                RequestedAction = "Refund",
                CustomerComments = "Some comment",
                StaffNotes = "Some notes",
                ReturnRequestStatus = ReturnRequestStatus.ItemsRefunded,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        /// <summary>
        /// Get return request action
        /// </summary>
        /// <returns></returns>
        public static ReturnRequestAction GetReturnRequestAction()
        {
            return new ReturnRequestAction
            {
                Name = "Name 1",
                DisplayOrder = 1
            };
        }

        /// <summary>
        /// Get return request reason
        /// </summary>
        /// <returns></returns>
        public static ReturnRequestReason GetReturnRequestReason()
        {
            return new ReturnRequestReason
            {
                Name = "Name 1",
                DisplayOrder = 1
            };
        }

        /// <summary>
        /// Get shopping cart item
        /// </summary>
        /// <returns></returns>
        public static ShoppingCartItem GetShoppingCartItem()
        {
            return new ShoppingCartItem
            {
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                AttributesXml = "AttributesXml 1",
                CustomerEnteredPrice = 1,
                Quantity = 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                Product = GetProduct(),
                StoreId = 1,
                RentalStartDateUtc = new DateTime(2010, 01, 03),
                RentalEndDateUtc = new DateTime(2010, 01, 04)
            };
        }

        #endregion

        #region Nop.Core.Domain.Polls

        /// <summary>
        /// Get poll
        /// </summary>
        /// <returns></returns>
        public static Poll GetPoll()
        {
            return new Poll
            {
                Name = "Name 1",
                SystemKeyword = "SystemKeyword 1",
                Published = true,
                ShowOnHomePage = true,
                DisplayOrder = 1,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                Language = GetLanguage()
            };
        }

        /// <summary>
        /// Get poll answer
        /// </summary>
        /// <returns></returns>
        public static PollAnswer GetPollAnswer()
        {
            return new PollAnswer
            {
                Name = "Answer 1",
                NumberOfVotes = 1,
                DisplayOrder = 2,
            };
        }

        /// <summary>
        /// Get poll voting record
        /// </summary>
        /// <returns></returns>
        public static PollVotingRecord GetPollVotingRecord()
        {
            return new PollVotingRecord
            {
                Customer = GetCustomer(),
                CreatedOnUtc = DateTime.UtcNow
            };
        }

        #endregion

        #region Nop.Core.Domain.Security

        /// <summary>
        /// Get ACL record
        /// </summary>
        /// <returns></returns>
        public static AclRecord GetAclRecord()
        {
            return new AclRecord
            {
                EntityId = 1,
                EntityName = "EntityName 1",
                CustomerRole = GetCustomerRole("Administrators")
            };
        }

        /// <summary>
        /// Get permission record
        /// </summary>
        /// <returns></returns>
        public static PermissionRecord GetPermissionRecord()
        {
            return new PermissionRecord
            {
                Name = "Name 1",
                SystemName = "SystemName 2",
                Category = "Category 4",
            };
        }

        #endregion

        #region Nop.Core.Domain.Seo

        /// <summary>
        /// Get URL record
        /// </summary>
        /// <returns></returns>
        public static UrlRecord GetUrlRecord()
        {
            return new UrlRecord
            {
                EntityId = 1,
                EntityName = "EntityName 1",
                Slug = "Slug 1",
                LanguageId = 2
            };
        }

        #endregion

        #region Nop.Core.Domain.Shipping

        /// <summary>
        /// Get delivery date
        /// </summary>
        /// <returns></returns>
        public static DeliveryDate GetDeliveryDate()
        {
            return new DeliveryDate
            {
                Name = "Name 1",
                DisplayOrder = 1
            };
        }

        /// <summary>
        /// Get shipment
        /// </summary>
        /// <returns></returns>
        public static Shipment GetShipment()
        {
            return new Shipment
            {
                TrackingNumber = "TrackingNumber 1",
                ShippedDateUtc = new DateTime(2010, 01, 01),
                DeliveryDateUtc = new DateTime(2010, 01, 02),
                CreatedOnUtc = new DateTime(2010, 01, 03),
                TotalWeight = 9.87M,
                AdminComment = "AdminComment 1"
            };
        }

        /// <summary>
        /// Get shipment item
        /// </summary>
        /// <returns></returns>
        public static ShipmentItem GetShipmentItem()
        {
            return new ShipmentItem
            {
                OrderItemId = 2,
                Quantity = 3,
                WarehouseId = 4
            };
        }

        /// <summary>
        /// Get shipping method
        /// </summary>
        /// <returns></returns>
        public static ShippingMethod GetShippingMethod()
        {
            return new ShippingMethod
            {
                Name = "By train",
                Description = "Description 1",
                DisplayOrder = 1
            };
        }

        /// <summary>
        /// Get shipping options
        /// </summary>
        /// <returns></returns>
        public static IList<ShippingOption> GetShippingOptions()
        {
            return new List<ShippingOption>
            {
                new ShippingOption
                {
                    Name = "a1",
                    Description = "a2",
                    Rate = 3.57M,
                    ShippingRateComputationMethodSystemName = "a4"
                },
                new ShippingOption
                {
                    Name = "b1",
                    Description = "b2",
                    Rate = 7.00M,
                    ShippingRateComputationMethodSystemName = "b4"
                }
            };
        }

        /// <summary>
        /// Get warehouse
        /// </summary>
        /// <returns></returns>
        public static Warehouse GetWarehouse()
        {
            return new Warehouse
            {
                Name = "Name 2",
                AddressId = 1,
                AdminComment = "AdminComment 1"
            };
        }

        #endregion

        #region  Nop.Core.Domain.Stores

        /// <summary>
        /// Get store
        /// </summary>
        /// <returns></returns>
        public static Store GetStore()
        {
            return new Store
            {
                Id = 1,
                Hosts = "yourstore.com, www.yourstore.com, ",
                Name = "Store 1",
                DisplayOrder = 1,
                Url = "http://www.test.com",
                DefaultLanguageId = 1,
                CompanyName = "company name",
                CompanyAddress = "some address",
                CompanyPhoneNumber = "123456789",
                CompanyVat = "some vat",
            };
        }

        /// <summary>
        /// Get store mapping
        /// </summary>
        /// <returns></returns>
        public static StoreMapping GetStoreMapping()
        {
            return new StoreMapping
            {
                EntityId = 1,
                EntityName = "EntityName 1",
                Store = GetStore()
            };
        }

        #endregion

        #region Nop.Core.Domain.Tasks

        /// <summary>
        /// Get schedule task
        /// </summary>
        /// <returns></returns>
        public static ScheduleTask GetScheduleTask()
        {
            return new ScheduleTask
            {
                Name = "Task 1",
                Seconds = 1,
                Type = "some type 1",
                Enabled = true,
                StopOnError = true,
                LeasedByMachineName = "LeasedByMachineName 1",
                LeasedUntilUtc = new DateTime(2009, 01, 01),
                LastStartUtc = new DateTime(2010, 01, 01),
                LastEndUtc = new DateTime(2010, 01, 02),
                LastSuccessUtc = new DateTime(2010, 01, 03),
            };
        }

        #endregion

        #region Nop.Core.Domain.Tax

        /// <summary>
        /// Ge tax category
        /// </summary>
        /// <returns></returns>
        public static TaxCategory GeTaxCategory()
        {
            return new TaxCategory
            {
                Name = "Books",
                DisplayOrder = 1
            };
        }

        #endregion

        #region Nop.Core.Domain.Topics

        /// <summary>
        /// Get topic
        /// </summary>
        /// <returns></returns>
        public static Topic GetTopic()
        {
            return new Topic
            {
                SystemName = "SystemName 1",
                IncludeInSitemap = true,
                IncludeInTopMenu = true,
                IncludeInFooterColumn1 = true,
                IncludeInFooterColumn2 = true,
                IncludeInFooterColumn3 = true,
                DisplayOrder = 1,
                AccessibleWhenStoreClosed = true,
                IsPasswordProtected = true,
                Password = "password",
                Title = "Title 1",
                Body = "Body 1",
                Published = true,
                TopicTemplateId = 1,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                SubjectToAcl = true,
                LimitedToStores = true
            };
        }

        /// <summary>
        /// Get topic template
        /// </summary>
        /// <returns></returns>
        public static TopicTemplate GetTopicTemplate()
        {
            return new TopicTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

        #endregion

        #region Nop.Core.Domain.Vendors

        /// <summary>
        /// Get vendor
        /// </summary>
        /// <returns></returns>
        public static Vendor GetVendor()
        {
            return new Vendor
            {
                Name = "Name 1",
                Email = "Email 1",
                Description = "Description 1",
                AdminComment = "AdminComment 1",
                PictureId = 1,
                Active = true,
                Deleted = true,
                DisplayOrder = 2,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
            };
        }

        /// <summary>
        /// Get vendor note
        /// </summary>
        /// <returns></returns>
        public static VendorNote GetVendorNote()
        {
            return new VendorNote
            {
                Note = "Note1",
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        #endregion

        #endregion

        #region Nop.Services

        #region Nop.Services.Discounts

        /// <summary>
        /// Get discount for caching
        /// </summary>
        /// <returns></returns>
        public static DiscountForCaching GetDiscountForCaching()
        {
            return new DiscountForCaching
            {
                Id = 1,
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToShipping,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited,
            };
        }

        #endregion

        #region Nop.Services.Messages

        /// <summary>
        /// Get tokens
        /// </summary>
        /// <param name="quantity">Tokens quantity</param>
        /// <param name="neverHtmlEncoded">Indicates whether the tokens should not be HTML encoded</param>
        /// <returns></returns>
        public static IList<Token> GetTokens(int quantity = 1, bool neverHtmlEncoded = false)
        {
            var tokens = new List<Token>();
            for (var i = 1; i <= quantity; i++)
            {
                tokens.Add(new Token("Token" + i, string.Format("<Value{0}>", i), neverHtmlEncoded));
            }

            return tokens;
        }

        #endregion

        #endregion
        
        public static IQueryable<T> ToIQueryable<T>(params T[] list)
        {
            return new EnumerableQuery<T>(list);
        }
    }
}
