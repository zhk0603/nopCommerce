using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Localization
{
    [TestFixture]
    public class LocalizedPropertyPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_localizedProperty()
        {
            var localizedProperty = TestHelper.GetLocalizedProperty(TestHelper.GetLanguage());

            var fromDb = SaveAndLoadEntity(localizedProperty);
            fromDb.ShouldNotBeNull();
            fromDb.EntityId.ShouldEqual(1);
            fromDb.LocaleKeyGroup.ShouldEqual("LocaleKeyGroup 1");
            fromDb.LocaleKey.ShouldEqual("LocaleKey 1");
            fromDb.LocaleValue.ShouldEqual("LocaleValue 1");
            
            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }
    }
}
