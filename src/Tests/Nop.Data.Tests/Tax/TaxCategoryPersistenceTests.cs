using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Tax
{
    [TestFixture]
    public class TaxCategoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_taxCategory()
        {
            var taxCategory = TestHelper.GeTaxCategory();

            var fromDb = SaveAndLoadEntity(taxCategory);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Books");
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}