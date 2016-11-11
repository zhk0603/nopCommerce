using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class CategoryServiceTests:ServiceTest
    {
        private CategoryService _categoryService;

        [SetUp]
        public new void SetUp()
        {
            var cacheManager = MockRepository.GenerateMock<ICacheManager>();
            var categoryRepository = MockRepository.GenerateMock<IRepository<Category>>();
            var productCategoryRepository = MockRepository.GenerateMock<IRepository<ProductCategory>>();
            var productRepository = MockRepository.GenerateMock<IRepository<Product>>();
            var aclRepository = MockRepository.GenerateMock<IRepository<AclRecord>>();
            var storeMappingRepository = MockRepository.GenerateMock<IRepository<StoreMapping>>();
            var workContext = MockRepository.GenerateMock<IWorkContext>();
            var storeContext = MockRepository.GenerateMock<IStoreContext>();
            var eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            var storeMappingService = MockRepository.GenerateMock<IStoreMappingService>();
            var aclService = MockRepository.GenerateMock<IAclService>();
            var catalogSettings = new CatalogSettings {IgnoreAcl = true};

            var category1 = TestHelper.GetCategory();
            category1.Id = 1;
            category1.Name = "Category 1";
            category1.ParentCategoryId = 0;
            category1.ShowOnHomePage = true;
            var category2 = TestHelper.GetCategory();
            category2.Id = 2;
            category2.ParentCategoryId = 1;
            var category3 = TestHelper.GetCategory();
            category3.Id = 3;
            category2.LimitedToStores = category3.LimitedToStores = true;
            category3.ParentCategoryId = 1;
            category3.ShowOnHomePage = true;
            var category4 = TestHelper.GetCategory();
            category4.Id = 4;
            category4.Published = false;
            category4.ParentCategoryId = 2;
            

            var storeMapping1 = TestHelper.GetStoreMapping();
            storeMapping1.EntityId = 2;
            storeMapping1.StoreId = 1;
            var storeMapping2 = TestHelper.GetStoreMapping();
            storeMapping2.EntityId = 3;
            storeMapping2.StoreId = 1;

            var storeMapping3 = TestHelper.GetStoreMapping();
            storeMapping3.EntityId = 1;
            storeMapping3.StoreId = 1;

            storeMapping1.EntityName = storeMapping2.EntityName = storeMapping3.EntityName = "Category";

            var product = TestHelper.GetProduct();
            product.Id = 1;

            var productCategory = TestHelper.GetProductCategory();
            productCategory.Id = 1;
            productCategory.CategoryId = 4;
            productCategory.Category = category4;
            productCategory.ProductId = 1;
            productCategory.Product = product;
            
            categoryRepository.Expect(x => x.Table)
                .Return(TestHelper.ToIQueryable(category1, category2, category3, category4));
            categoryRepository.Expect(x => x.GetById(1)).Return(category1);

            storeMappingRepository.Expect(x => x.Table).Return(TestHelper.ToIQueryable(storeMapping1, storeMapping2, storeMapping3));
            
            workContext.Expect(x => x.CurrentCustomer).Return(TestHelper.GetCustomer());
            storeContext.Expect(x => x.CurrentStore).Return(TestHelper.GetStore());

            aclService.Expect(x => x.Authorize(category1)).Return(true);
            storeMappingService.Expect(x => x.Authorize(category1)).Return(true);

            aclService.Expect(x => x.Authorize(category4)).Return(true);
            storeMappingService.Expect(x => x.Authorize(category4)).Return(true);

            productCategoryRepository.Expect(x => x.Table).Return(TestHelper.ToIQueryable(productCategory));
            productCategoryRepository.Expect(x => x.GetById(1)).Return(productCategory);
            productRepository.Expect(x => x.Table).Return(TestHelper.ToIQueryable(product));

            _categoryService = new CategoryService(cacheManager, categoryRepository, productCategoryRepository,
                productRepository, aclRepository, storeMappingRepository, workContext, storeContext, eventPublisher,
                storeMappingService, aclService, catalogSettings);
        }

        [Test]
        public void can_get_all_categories()
        {
            _categoryService.GetAllCategories().ShouldNotBeNull().Count.ShouldEqual(3);
            _categoryService.GetAllCategories(showHidden: true).ShouldNotBeNull().Count.ShouldEqual(4);
            _categoryService.GetAllCategories("Category 1").ShouldNotBeNull().Count.ShouldEqual(1);
            _categoryService.GetAllCategories(storeId: 1).ShouldNotBeNull().Count.ShouldEqual(3);
        }

        [Test]
        public void can_get_all_categories_by_parent_category_id()
        {
            _categoryService.GetAllCategoriesByParentCategoryId(0).ShouldNotBeNull().Count.ShouldEqual(1);
            _categoryService.GetAllCategoriesByParentCategoryId(2).ShouldNotBeNull().Count.ShouldEqual(0);
            _categoryService.GetAllCategoriesByParentCategoryId(2, true).ShouldNotBeNull().Count.ShouldEqual(1);

            _categoryService.GetAllCategoriesByParentCategoryId(1).ShouldNotBeNull().Count.ShouldEqual(2);
            _categoryService.GetAllCategoriesByParentCategoryId(1, true).ShouldNotBeNull().Count.ShouldEqual(2);
            _categoryService.GetAllCategoriesByParentCategoryId(1, true, true).ShouldNotBeNull().Count.ShouldEqual(3);
        }

        [Test]
        public void can_get_all_categories_displayed_on_home_page()
        {
            _categoryService.GetAllCategoriesDisplayedOnHomePage().ShouldNotBeNull().Count.ShouldEqual(1);
            _categoryService.GetAllCategoriesDisplayedOnHomePage(true).ShouldNotBeNull().Count.ShouldEqual(2);
        }

        [Test]
        public void can_get_category_by_id()
        {
            _categoryService.GetCategoryById(0).ShouldBeNull();
            _categoryService.GetCategoryById(1).ShouldNotBeNull().Name.ShouldEqual("Category 1");
            _categoryService.GetCategoryById(5).ShouldBeNull();
        }

        [Test]
        public void can_get_product_categories_by_category_id()
        {
            _categoryService.GetProductCategoriesByCategoryId(0).ShouldNotBeNull().Count.ShouldEqual(0);
            _categoryService.GetProductCategoriesByCategoryId(4).ShouldNotBeNull().Count.ShouldEqual(0);
            _categoryService.GetProductCategoriesByCategoryId(4, showHidden: true).ShouldNotBeNull().Count.ShouldEqual(1);
        }

        [Test]
        public void can_get_product_categories_by_product_id()
        {
            _categoryService.GetProductCategoriesByProductId(0).ShouldNotBeNull().Count.ShouldEqual(0);
            _categoryService.GetProductCategoriesByProductId(1).ShouldNotBeNull().Count.ShouldEqual(0);
            _categoryService.GetProductCategoriesByProductId(1, true).ShouldNotBeNull().Count.ShouldEqual(1);
        }

        [Test]
        public void can_get_product_category_by_id()
        {
            _categoryService.GetProductCategoryById(0).ShouldBeNull();
            _categoryService.GetProductCategoryById(1).ShouldNotBeNull().Category.ShouldNotBeNull().Name.ShouldEqual("Books");
            _categoryService.GetProductCategoryById(5).ShouldBeNull();
        }

        [Test]
        public void can_get_not_existing_categories()
        {
            _categoryService.GetNotExistingCategories(new[] { "Books", "Category 1" }).ShouldNotBeNull().Length.ShouldEqual(0);
            _categoryService.GetNotExistingCategories(new[] { "Books 1", "Category 1" }).ShouldNotBeNull().Length.ShouldEqual(1);
        }

        [Test]
        public void can_get_product_category_ids()
        {
            _categoryService.GetProductCategoryIds(new[] { 0 }).ShouldNotBeNull().Count.ShouldEqual(0);
            _categoryService.GetProductCategoryIds(new[] { 1 }).ShouldNotBeNull().Count.ShouldEqual(1);
        }
    }
}
