using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Caching
{
    [TestFixture]
    public class MemoryCacheManagerTests
    {
        [Test]
        public void Can_set_and_get_object_from_cache()
        {
            TestHelper.GetMemoryCacheManager().Get<int>("some_key_1").ShouldEqual(3);
        }

        [Test]
        public void Can_validate_whetherobject_is_cached()
        {
            var cacheManager = TestHelper.GetMemoryCacheManager();
            cacheManager.IsSet("some_key_1").ShouldEqual(true);
            cacheManager.IsSet("some_key_3").ShouldEqual(false);
        }

        [Test]
        public void Can_clear_cache()
        {
            var cacheManager = TestHelper.GetMemoryCacheManager();
            cacheManager.Clear();
            cacheManager.IsSet("some_key_1").ShouldEqual(false);
        }
    }
}
