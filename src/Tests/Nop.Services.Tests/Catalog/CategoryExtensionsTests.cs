using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Tests;
using NUnit.Framework;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Stores;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class CategoryExtensionsTests:ServiceTest
    {
        private CategoryService _categoryService;
        private List<Category> _categorys;
        private IAclService _aclService;
        private IStoreMappingService _storeMappingService;

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
            _storeMappingService = MockRepository.GenerateMock<IStoreMappingService>();
            _aclService = MockRepository.GenerateMock<IAclService>();
           
            var catalogSettings = new CatalogSettings();

            var category1 = TestHelper.GetCategory();
            category1.Id = 1;
            category1.ParentCategoryId = 0;
            category1.Name = "Parent categoty";
            var category2 = TestHelper.GetCategory();
            category2.Id = 2;
            category2.ParentCategoryId = 1;
            var category3 = TestHelper.GetCategory();
            category3.Id = 3;
            category3.ParentCategoryId = 4;

            _aclService.Expect(x => x.Authorize(category1)).Return(true);
            _storeMappingService.Expect(x => x.Authorize(category1)).Return(true);
            _aclService.Expect(x => x.Authorize(category2)).Return(true);
            _storeMappingService.Expect(x => x.Authorize(category2)).Return(true);

            _categorys = new List<Category> {category1, category2, category3};

            categoryRepository.Expect(x => x.GetById(1)).Return(category1);

            _categoryService = new CategoryService(cacheManager, categoryRepository, productCategoryRepository,
                productRepository, aclRepository, storeMappingRepository, workContext, storeContext, eventPublisher,
                _storeMappingService, _aclService, catalogSettings);
        }

        [Test]
        public void can_sort_categories_for_tree()
        {
            _categorys.SortCategoriesForTree().ShouldNotBeNull().Count.ShouldEqual(3);
            _categorys.SortCategoriesForTree(ignoreCategoriesWithoutExistingParent:true).ShouldNotBeNull().Count.ShouldEqual(2);
            _categorys.SortCategoriesForTree(1, true).ShouldNotBeNull().Count.ShouldEqual(1);
        }

        [Test]
        public void can_find_product_category()
        {
            var productCategory1 = TestHelper.GetProductCategory();
            productCategory1.CategoryId = 1;
            productCategory1.ProductId = 1;
            var productCategory2 = TestHelper.GetProductCategory();
            productCategory2.CategoryId = 2;
            productCategory2.ProductId = 2;
            var productCategory3 = TestHelper.GetProductCategory();
            productCategory3.CategoryId = 1;
            productCategory3.ProductId = 3;

            var productCategorys = new List<ProductCategory> { productCategory1, productCategory2, productCategory3 };

            productCategorys.FindProductCategory(1, 1).ShouldNotBeNull();
            productCategorys.FindProductCategory(1, 2).ShouldBeNull();
            productCategorys.FindProductCategory(3, 1).ShouldNotBeNull();
            productCategorys.FindProductCategory(2, 1).ShouldBeNull();
        }

        [Test]
        public void can_get_formatted_bread_crumb()
        {
            var category = TestHelper.GetCategory();
            category.Id = 2;
            category.ParentCategoryId = 1;

            category.GetFormattedBreadCrumb(_categoryService).ShouldNotBeNull().ShouldEqual("Parent categoty >> Books");
            category.GetFormattedBreadCrumb(_categoryService, "->").ShouldNotBeNull().ShouldEqual("Parent categoty -> Books");

            category.GetFormattedBreadCrumb(_categorys).ShouldNotBeNull().ShouldEqual("Parent categoty >> Books");
            category.GetFormattedBreadCrumb(_categorys, "->").ShouldNotBeNull().ShouldEqual("Parent categoty -> Books");

            category.GetCategoryBreadCrumb(_categoryService, _aclService, _storeMappingService).ShouldNotBeNull().Count.ShouldEqual(2);
        }
    }
}
