using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Common
{
    [TestFixture]
    public class SearchTermPeristenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_searchTerm()
        {
            var searchTerm = TestHelper.GetSearchTerm();

            var fromDb = SaveAndLoadEntity(searchTerm);
            fromDb.ShouldNotBeNull();

            fromDb.Keyword.ShouldEqual("Keyword 1");
            fromDb.StoreId.ShouldEqual(1);
            fromDb.Count.ShouldEqual(2);
        }
    }
}
