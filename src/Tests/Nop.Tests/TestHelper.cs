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

        public static BlogComment GetBlogComment()
        {
            return new BlogComment
            {
                CreatedOnUtc = new DateTime(2010, 01, 03),
                Customer = GetCustomer()
            };
        }

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

        public static Product AddTierPrices(this Product product, decimal price, int quantity,
            CustomerRole customerRole = null)
        {
            product.TierPrices.Add(GetTierPrice(product, customerRole, price, quantity));
            return product;
        }

        public static BackInStockSubscription GetBackInStockSubscription()
        {
            return new BackInStockSubscription
            {
                Product = GetProduct(),
                Customer = GetCustomer(),
                CreatedOnUtc = new DateTime(2010, 01, 02)
            };
        }

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

        public static CategoryTemplate GetCategoryTemplate()
        {
            return new CategoryTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

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

        public static ManufacturerTemplate GetManufacturerTemplate()
        {
            return new ManufacturerTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

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

        public static Product GetProduct(bool setAvailableStartDateTimeUtc = true,
            bool setAvailableEndDateTimeUtc = true,
            bool customerEntersPrice = true,
            decimal price = 21.1M,
            bool useMultipleWarehouses = true,
            RentalPricePeriod rentalPricePeriod = 0,
            decimal weight = 26.1M,
            decimal length = 27.1M,
            decimal width = 28.1M,
            decimal height = 29.1M)
        {
            var availableStartDateTimeUtc = setAvailableStartDateTimeUtc
                ? new DateTime(2010, 01, 01) as DateTime?
                : null;
            var availableEndDateTimeUtc = setAvailableEndDateTimeUtc ? new DateTime(2010, 01, 03) as DateTime? : null;

            return new Product
            {
                Id = 1,
                Name = "Product name 1",
                AvailableStartDateTimeUtc = availableStartDateTimeUtc,
                RequiredProductIds = "1, 4,7 ,a,",
                AvailableEndDateTimeUtc = availableEndDateTimeUtc,
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
                RentalPricePeriodId = (int) rentalPricePeriod,
                RentalPricePeriod = rentalPricePeriod,
                IsShipEnabled = true,
                IsFreeShipping = true,
                ShipSeparately = true,
                AdditionalShippingCharge = 10.1M,
                DeliveryDateId = 5,
                IsTaxExempt = true,
                TaxCategoryId = 11,
                IsTelecommunicationsOrBroadcastingOrElectronicServices = true,
                ManageInventoryMethodId = 1,
                UseMultipleWarehouses = useMultipleWarehouses,
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
                Price = price,
                OldPrice = 22.1M,
                ProductCost = 23.1M,
                SpecialPrice = 32.1M,
                SpecialPriceStartDateTimeUtc = new DateTime(2010, 01, 05),
                SpecialPriceEndDateTimeUtc = new DateTime(2010, 01, 06),
                CustomerEntersPrice = customerEntersPrice,
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
                Weight = weight,
                Length = length,
                Width = width,
                Height = height,
                RequireOtherProducts = true,
                AutomaticallyAddRequiredProducts = true,
                DisplayOrder = 30,
                Published = true,
                Deleted = false
            };
        }

        public static ProductAttribute GetProductAttribute(int id = 1, string name = "Name 1")
        {
            return new ProductAttribute
            {
                Id = id,
                Name = name,
                Description = "Description 1"
            };
        }

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

        public static ProductAttributeMapping GetProductAttributeMapping(Product product = null, 
            int id = 1,
            string textPrompt = "TextPrompt 1", 
            AttributeControlType controlType = AttributeControlType.DropdownList,
            ProductAttribute productAttribute = null)
        {
            product = product ?? GetProduct();
            productAttribute = productAttribute ?? GetProductAttribute();

            return new ProductAttributeMapping
            {
                Id = id,
                TextPrompt = textPrompt,
                IsRequired = true,
                AttributeControlType = controlType,
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

        public static ProductAttributeValue GetProductAttributeValue(int id = 1, string name = "Name 1",
            ProductAttributeMapping productAttributeMapping = null)
        {
            productAttributeMapping = productAttributeMapping ?? GetProductAttributeMapping();
            return new ProductAttributeValue
            {
                Id = id,
                AttributeValueType = AttributeValueType.AssociatedToProduct,
                AssociatedProductId = 10,
                Name = name,
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

        public static ProductCategory GetProductCategory(Product product = null)
        {
            return new ProductCategory
            {
                IsFeaturedProduct = true,
                DisplayOrder = 1,
                Product = product ?? GetProduct(),
                Category = GetCategory()
            };
        }

        public static ProductManufacturer GetProductManufacturer(Product product = null)
        {
            return new ProductManufacturer
            {
                IsFeaturedProduct = true,
                DisplayOrder = 1,
                Product = product ?? GetProduct(),
                Manufacturer = GetManufacturer()
            };
        }

        public static ProductPicture GetProductPicture(Product product = null)
        {
            return new ProductPicture
            {
                DisplayOrder = 1,
                Product = product ?? GetProduct(),
                Picture = GetPicture()
            };
        }

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

        public static ProductTag GetProductTag()
        {
            return new ProductTag
            {
                Name = "Name 1"
            };
        }

        public static ProductTemplate GetProductTemplate()
        {
            return new ProductTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

        public static ProductWarehouseInventory GetProductWarehouseInventory(Product product = null,
            Warehouse warehouse = null, 
            int warehouseId = 0, 
            int stockQuantity = 3)
        {
            product = product ?? GetProduct();

            var productWarehouseInventory = new ProductWarehouseInventory
            {
                Product = product,

                StockQuantity = stockQuantity,
                ReservedQuantity = 4
            };

            if (warehouseId == 0)
            {
                warehouse = warehouse ?? GetWarehouse();
                productWarehouseInventory.Warehouse = warehouse;
                productWarehouseInventory.WarehouseId = warehouse.Id;
            }
            else
            {
                productWarehouseInventory.WarehouseId = warehouseId;
                if (warehouse != null)
                    productWarehouseInventory.Warehouse = warehouse;
            }

            return productWarehouseInventory;
        }

        public static SpecificationAttribute GetSpecificationAttribute()
        {
            return new SpecificationAttribute
            {
                Name = "SpecificationAttribute name 1",
                DisplayOrder = 2
            };
        }

        public static SpecificationAttributeOption GetSpecificationAttributeOption(bool setSpecificationAttribute = true)
        {
            var specificationAttributeOption = new SpecificationAttributeOption
            {
                Name = "SpecificationAttributeOption name 1",
                DisplayOrder = 1,
                ColorSquaresRgb = "ColorSquaresRgb 2",

            };

            if (setSpecificationAttribute)
                specificationAttributeOption.SpecificationAttribute = GetSpecificationAttribute();

            return specificationAttributeOption;
        }
        
        public static TierPrice GetTierPrice(Product product = null, 
            CustomerRole customerRole = null,
            decimal price = 2.1M, 
            int quantity = 1, 
            int id = 1)
        {
            var tierPrice = new TierPrice
            {
                Id = id,
                StoreId = 1,
                Quantity = quantity,
                Price = price
            };

            if (product != null)
                tierPrice.Product = product;

            if (customerRole != null)
                tierPrice.CustomerRole = customerRole;

            return tierPrice;
        }

        #endregion

        #region Nop.Core.Domain.Common

        public static Address GetAddress(int? stateProvinceId = null, int? countryId = null)
        {
            var country = GetCountry(countryId);
            var state = GetStateProvince(stateProvinceId, country);

            return new Address
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "Email 1",
                Company = "Company 1",
                Country = country,
                CountryId = countryId ?? 0,
                StateProvince = state,
                StateProvinceId = stateProvinceId ?? 0,
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
        
        public static AddressAttribute GetAddressAttribute(bool addAddressAttributeValues = true)
        {
            var addressAttribute = new AddressAttribute
            {
                Name = "Name 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };

            if (addAddressAttributeValues)
                addressAttribute.AddressAttributeValues.Add(GetAddressAttributeValue());

            return addressAttribute;
        }

        public static AddressAttributeValue GetAddressAttributeValue(bool setAddressAttribute = false)
        {
            var addess = new AddressAttributeValue
            {
                Name = "Name 2",
                IsPreSelected = true,
                DisplayOrder = 1
            };

            if (setAddressAttribute)
                addess.AddressAttribute = GetAddressAttribute(false);

            return addess;
        }

        public static GenericAttribute GetGenericAttribute(int entityId = 1, string value = "Value 1", string key = "Key 1", int storeId = 2)
        {
            key = key ?? SystemCustomerAttributeNames.DiscountCouponCode;
            return new GenericAttribute
            {
                EntityId = entityId,
                Key = key,
                KeyGroup = "Customer",
                Value = value,
                StoreId = storeId
            };
        }

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
        
        public static CustomerAttribute GetCustomerAttribute(bool addCustomerAttribute = true)
        {
            var customerAttribute = new CustomerAttribute
            {
                Name = "Name 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };

            if (addCustomerAttribute)
                customerAttribute.CustomerAttributeValues.Add(GetCustomerAttributeValue());

            return customerAttribute;
        }

        public static CustomerAttributeValue GetCustomerAttributeValue(bool setCustomerAttribute = false)
        {
            var customerAttributeValue = new CustomerAttributeValue
            {
                Name = "Name 2",
                IsPreSelected = true,
                DisplayOrder = 1,
            };

            if (setCustomerAttribute)
                customerAttributeValue.CustomerAttribute = GetCustomerAttribute(false);

            return customerAttributeValue;
        }

        public static Customer GetCustomerByName(string name, 
            string systemNames = "", 
            string password = "password",
            PasswordFormat passwordFormat = PasswordFormat.Clear)
        {
            var customer = systemNames.Any() ? GetCustomer(systemNames) : GetCustomer();

            customer.PasswordFormat = passwordFormat;
            customer.Password = password;
            customer.Username = name + "@test.com";
            customer.Email = customer.Username;

            return customer;
        }
        
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

        public static ExternalAuthenticationRecord GetExternalAuthenticationRecord(Customer customer = null)
        {
            return new ExternalAuthenticationRecord
            {
                ExternalIdentifier = "ExternalIdentifier 1",
                ExternalDisplayIdentifier = "ExternalDisplayIdentifier 1",
                OAuthToken = "OAuthToken 1",
                OAuthAccessToken = "OAuthAccessToken 1",
                ProviderSystemName = "ProviderSystemName 1",
                Email = "Email 1",
                Customer = customer
            };
        }

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

        public static Country GetCountry(int? id = null)
        {
            return new Country
            {
                Id = id ?? _id++,
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

        public static Currency GetCurrency(int id = 1, 
            string name = "US Dollar", 
            string currencyCode = "USD",
            decimal rate = 1.1M, 
            string displayLocale = "en-US", 
            string customFormatting = "")
        {
            return new Currency
            {
                Id = id,
                Name = name,
                CurrencyCode = currencyCode,
                Rate = rate,
                DisplayLocale = displayLocale,
                CustomFormatting = customFormatting,
                LimitedToStores = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };
        }

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
        
        public static StateProvince GetStateProvince(int? id = null, Country country = null)
        {
            var stateProvince = new StateProvince
            {
                Id = id ?? 0,
                Name = "Louisiana",
                Abbreviation = "LA",
                DisplayOrder = 1,
                Published = true,
                Country = country
            };

            if (country != null)
                stateProvince.CountryId = country.Id;

            return stateProvince;
        }

        #endregion
        
        #region Nop.Core.Domain.Discounts

        public static Discount GetDiscount(DiscountType discountType = DiscountType.AssignedToCategories,
            bool usePercentage = true,
            bool setEndDateUtc = true,
            bool requiresCouponCode = true,
            string couponCode = "SecretCode",
            decimal discountAmount = 2.1M,
            decimal discountPercentage = 1.1M,
            decimal maximumDiscountAmount = 208.1M)
        {
            var endDateUtc = setEndDateUtc ? new DateTime(2010, 01, 02) as DateTime? : null;
            return new Discount
            {
                DiscountType = discountType,
                Name = "Discount 1",
                UsePercentage = usePercentage,
                DiscountPercentage = discountPercentage,
                DiscountAmount = discountAmount,
                MaximumDiscountAmount = maximumDiscountAmount,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = endDateUtc,
                RequiresCouponCode = requiresCouponCode,
                CouponCode = couponCode,
                IsCumulative = true,
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 3,
                MaximumDiscountedQuantity = 4,
                AppliedToSubCategories = true
            };
        }

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

        public static DiscountRequirement GetDiscountRequirement(Discount discount = null)
        {
            return new DiscountRequirement
            {
                DiscountRequirementRuleSystemName = "BillingCountryIs",
                Discount = discount
            };
        }

        public static DiscountUsageHistory GetDiscountUsageHistory(Order order = null)
        {
            return new DiscountUsageHistory
            {
                Discount = GetDiscount(),
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Order = order
            };
        }

        #endregion

        #region Nop.Core.Domain.Forums

        public static Forum GetForum(ForumGroup forumGroup = null)
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

            if (forumGroup != null)
            {
                forum.ForumGroup = forumGroup;
                forum.ForumGroupId = forumGroup.Id;
            }

            return forum;
        }

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

        public static ForumPost GetForumPost(ForumTopic forumTopic, Customer customer)
        {
            var forumPost = new ForumPost
            {
                Text = "Forum Post 1 Text",
                IPAddress = "127.0.0.1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            if (forumTopic != null)
            {
                forumPost.ForumTopic = forumTopic;
                forumPost.TopicId = forumTopic.Id;
            }

            if (customer != null)
                forumPost.CustomerId = customer.Id;

            return forumPost;
        }

        public static ForumSubscription GetForumSubscription(Customer customer, Forum forum = null)
        {
            var forumSubscription = new ForumSubscription
            {
                CreatedOnUtc = DateTime.UtcNow,
                SubscriptionGuid = new Guid("11111111-2222-3333-4444-555555555555")
            };

            if (customer != null)
                forumSubscription.CustomerId = customer.Id;
            if (forum != null)
                forumSubscription.ForumId = forum.Id;

            return forumSubscription;
        }

        public static ForumTopic GetForumTopic(Customer customer, Forum forum)
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

            if (customer != null)
                forumTopic.CustomerId = customer.Id;
            if (forum != null)
                forumTopic.ForumId = forum.Id;

            return forumTopic;
        }
        
        public static PrivateMessage GetPrivateMessage(Customer fromCustomer, Customer toCustomer, Store store)
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

            if (fromCustomer != null)
                privateMessage.FromCustomerId = fromCustomer.Id;
            if (toCustomer != null)
                privateMessage.ToCustomerId = toCustomer.Id;
            if (store != null)
                privateMessage.StoreId = store.Id;

            return privateMessage;
        }

        #endregion

        #region Nop.Core.Domain.Localization

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

        public static LocaleStringResource GetLocaleStringResource(Language language = null)
        {
            return new LocaleStringResource
            {
                ResourceName = "ResourceName1",
                ResourceValue = "ResourceValue2",
                Language = language
            };
        }

        public static LocalizedProperty GetLocalizedProperty(Language language = null)
        {
            return new LocalizedProperty
            {
                EntityId = 1,
                LocaleKeyGroup = "LocaleKeyGroup 1",
                LocaleKey = "LocaleKey 1",
                LocaleValue = "LocaleValue 1",
                Language = language
            };
        }

        #endregion

        #region Nop.Core.Domain.Logging

        public static ActivityLog GetActivityLog(int id = 1, 
            Customer customer = null,
            ActivityLogType activityLogType = null)
        {
            customer = customer ?? GetCustomer();
            activityLogType = activityLogType ?? GetActivityLogType();

            return new ActivityLog
            {
                Id = id,
                ActivityLogType = activityLogType,
                CustomerId = customer.Id,
                Customer = customer
            };
        }

        public static ActivityLogType GetActivityLogType()
        {
            return new ActivityLogType
            {
                SystemKeyword = "SystemKeyword 1",
                Name = "Name 1",
                Enabled = true
            };
        }
        
        public static Log GetLog(Customer customer = null)
        {
            return new Log
            {
                LogLevel = LogLevel.Error,
                ShortMessage = "ShortMessage1",
                FullMessage = "FullMessage1",
                IpAddress = "127.0.0.1",
                PageUrl = "http://www.someUrl1.com",
                ReferrerUrl = "http://www.someUrl2.com",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Customer = customer
            };
        }

        #endregion
        
        #region Nop.Core.Domain.Media

        public static Download GetGeDownload()
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

        public static NewsComment GetNewsComment()
        {
            return new NewsComment
            {
                CommentText = "Comment text 1",
                CreatedOnUtc = new DateTime(2010, 01, 03),
                Customer = GetCustomer()
            };
        }

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

        public static CheckoutAttribute GetCheckoutAttribute(int id = 1, bool shippableProductRequired = true)
        {
            return new CheckoutAttribute
            {
                Id = id,
                Name = "Name 1",
                TextPrompt = "TextPrompt 1",
                IsRequired = true,
                ShippableProductRequired = shippableProductRequired,
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

        public static CheckoutAttributeValue GetCheckoutAttributeValue(int id = 1, 
            string name = "Name 2",
            CheckoutAttribute checkoutAttribute = null)
        {
            var checkoutAttributeValue = new CheckoutAttributeValue
            {
                Id = id,
                Name = name,
                PriceAdjustment = 1,
                WeightAdjustment = 2,
                IsPreSelected = true,
                DisplayOrder = 3,
                ColorSquaresRgb = "#112233"
            };

            if (checkoutAttribute != null)
            {
                checkoutAttributeValue.CheckoutAttribute = checkoutAttribute;
                checkoutAttributeValue.CheckoutAttributeId = checkoutAttribute.Id;
            }

            return checkoutAttributeValue;
        }

        public static GiftCard GetGiftCard(bool isGiftCardActivated, bool addGiftCardUsageHistory = true)
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
            };

            if (isGiftCardActivated)
                giftCard.IsGiftCardActivated = true;

            if (addGiftCardUsageHistory)
            {
                giftCard.GiftCardUsageHistory.Add(GetGiftCardUsageHistory(usedValue: 30));
                giftCard.GiftCardUsageHistory.Add(GetGiftCardUsageHistory(usedValue: 20));
                giftCard.GiftCardUsageHistory.Add(GetGiftCardUsageHistory(usedValue: 5));
            }

            return giftCard;
        }

        public static GiftCardUsageHistory GetGiftCardUsageHistory(Order usedWithOrder = null, GiftCard giftCard = null, decimal usedValue = 1.1M)
        {
            return new GiftCardUsageHistory
            {
                UsedValue = usedValue,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UsedWithOrder = usedWithOrder,
                GiftCard = giftCard
            };
        }
        
        public static Order GetOrder(Address shippingAddress = null,
            RewardPointsHistory redeemedRewardPointsEntry = null)
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
                ShippingAddress = shippingAddress,
                PickupAddress = GetAddress(),
                ShippingMethod = "ShippingMethod1",
                ShippingRateComputationMethodSystemName = "ShippingRateComputationMethodSystemName1",
                PickUpInStore = true,
                CustomValuesXml = "CustomValuesXml1",
                RedeemedRewardPointsEntry = redeemedRewardPointsEntry
            };
        }

        public static OrderItem GetOrderItem(Order order = null)
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
                RentalEndDateUtc = new DateTime(2010, 01, 02),
                Order = order
            };
        }

        public static OrderNote GetOrderNote(Order order = null)
        {
            return new OrderNote
            {
                Note = "Note1",
                DownloadId = 1,
                DisplayToCustomer = true,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Order = order
            };
        }

        public static RecurringPayment GetRecurringPayment(RecurringProductCyclePeriod recurringProductCyclePeriod,
           int cycleLength = 2, 
           bool isActive = true, 
           Order initialOrder = null, 
           bool deleted = false)
        {
            return new RecurringPayment
            {
                CycleLength = cycleLength,
                CyclePeriod = recurringProductCyclePeriod,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 3, 1),
                CreatedOnUtc = new DateTime(2010, 1, 1),
                IsActive = isActive,
                InitialOrder = initialOrder,
                Deleted = deleted
            };
        }

        public static RecurringPaymentHistory GetRecurringPaymentHistory(RecurringPayment recurringPayment = null)
        {
            return new RecurringPaymentHistory
            {
                CreatedOnUtc = new DateTime(2010, 01, 03),
                RecurringPayment = recurringPayment
            };
        }

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

        public static ReturnRequestAction GetReturnRequestAction()
        {
            return new ReturnRequestAction
            {
                Name = "Name 1",
                DisplayOrder = 1
            };
        }

        public static ReturnRequestReason GetReturnRequestReason()
        {
            return new ReturnRequestReason
            {
                Name = "Name 1",
                DisplayOrder = 1
            };
        }

        public static ShoppingCartItem GetShoppingCartItem(Product product = null, Customer customer = null, int quantity = 2, string attributesXml = "AttributesXml 1")
        {
            return new ShoppingCartItem
            {
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                AttributesXml = attributesXml,
                CustomerEnteredPrice = 1,
                Quantity = quantity,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                Product = product ?? GetProduct(),
                Customer = customer,
                StoreId = 1,
                RentalStartDateUtc = new DateTime(2010, 01, 03),
                RentalEndDateUtc = new DateTime(2010, 01, 04)
            };
        }

        public static ShoppingCartItem GetShoppingCartItem(int quantity, 
            decimal additionalShippingCharge,
            bool isShipEnabled = true, 
            Customer customer = null)
        {
            customer = customer ?? new Customer();
            var shoppingCartItem = GetShoppingCartItem(new Product
            {
                IsShipEnabled = isShipEnabled,
                IsFreeShipping = false,
                AdditionalShippingCharge = additionalShippingCharge
            }, customer);

            shoppingCartItem.Quantity = quantity;

            return shoppingCartItem;
        }

        #endregion

        #region Nop.Core.Domain.Polls

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

        public static PollAnswer GetPollAnswer()
        {
            return new PollAnswer
            {
                Name = "Answer 1",
                NumberOfVotes = 1,
                DisplayOrder = 2,
            };
        }

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

        public static AclRecord GetAclRecord()
        {
            return new AclRecord
            {
                EntityId = 1,
                EntityName = "EntityName 1",
                CustomerRole = GetCustomerRole("Administrators")
            };
        }

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

        public static DeliveryDate GetDeliveryDate()
        {
            return new DeliveryDate
            {
                Name = "Name 1",
                DisplayOrder = 1
            };
        }

        public static Shipment GetShipment(Order order = null)
        {
            return new Shipment
            {
                TrackingNumber = "TrackingNumber 1",
                ShippedDateUtc = new DateTime(2010, 01, 01),
                DeliveryDateUtc = new DateTime(2010, 01, 02),
                CreatedOnUtc = new DateTime(2010, 01, 03),
                TotalWeight = 9.87M,
                AdminComment = "AdminComment 1",
                Order = order
            };
        }

        public static ShipmentItem GetShipmentItem(Shipment shipment = null)
        {
            return new ShipmentItem
            {
                OrderItemId = 2,
                Quantity = 3,
                WarehouseId = 4,
                Shipment = shipment
            };
        }

        public static ShippingMethod GetShippingMethod()
        {
            return new ShippingMethod
            {
                Name = "By train",
                Description = "Description 1",
                DisplayOrder = 1
            };
        }

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

        public static VendorNote GetVendorNote(Vendor vendor = null)
        {
            return new VendorNote
            {
                Note = "Note1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Vendor = vendor
            };
        }
        
        #endregion
        
        #endregion
        
        #region Nop.Services.Messages

        public static IList<Token> GeTokens(int count = 1, bool neverHtmlEncoded = false)
        {
            var tokens = new List<Token>();
            for (var i = 1; i <= count; i++)
            {
                tokens.Add(new Token("Token" + i, string.Format("<Value{0}>", i), neverHtmlEncoded));
            }

            return tokens;
        }

        #endregion
    }
}
