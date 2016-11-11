using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class CategoryTemplateServiceTests:ServiceTest
    {
        private CategoryTemplateService _categoryTemplateService;

        [SetUp]
        public new void SetUp()
        {
            var categoryTemplateRepository = MockRepository.GenerateMock<IRepository<CategoryTemplate>>();
            var eventPublisher = MockRepository.GenerateMock<IEventPublisher>();

            var categoryTemplate1 = TestHelper.GetCategoryTemplate();
            categoryTemplate1.Id = 1;
            var categoryTemplate2 = TestHelper.GetCategoryTemplate();
            var categoryTemplate3 = TestHelper.GetCategoryTemplate();

            categoryTemplateRepository.Expect(x => x.Table).Return(TestHelper.ToIQueryable(categoryTemplate1, categoryTemplate2, categoryTemplate3));
            categoryTemplateRepository.Expect(x => x.GetById(1)).Return(categoryTemplate1);

            _categoryTemplateService =new CategoryTemplateService(categoryTemplateRepository, eventPublisher);
        }

        [Test]
        public void can_get_all_category_templates()
        {
            _categoryTemplateService.GetAllCategoryTemplates().ShouldNotBeNull().Count.ShouldEqual(3);
        }

        [Test]
        public void can_get_category_template_by_id()
        {
            _categoryTemplateService.GetCategoryTemplateById(0).ShouldBeNull();
            _categoryTemplateService.GetCategoryTemplateById(1).ShouldNotBeNull().Name.ShouldEqual("Name 1");
        }
    }
}
