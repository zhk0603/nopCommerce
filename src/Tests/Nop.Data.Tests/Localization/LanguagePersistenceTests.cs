using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Localization
{
    [TestFixture]
    public class LanguagePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_language()
        {
            var lang = TestHelper.GetLanguage();

            var fromDb = SaveAndLoadEntity(lang);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("English");
            fromDb.LanguageCulture.ShouldEqual("en-Us");
            fromDb.UniqueSeoCode.ShouldEqual("en");
            fromDb.FlagImageFileName.ShouldEqual("us.png");
            fromDb.Rtl.ShouldEqual(true);
            fromDb.DefaultCurrencyId.ShouldEqual(1);
            fromDb.Published.ShouldEqual(true);
            fromDb.LimitedToStores.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);
        }

        [Test]
        public void Can_save_and_load_language_with_localeStringResources()
        {
            var lang = TestHelper.GetLanguage();
            lang.LocaleStringResources.Add(TestHelper.GetLocaleStringResource());

            var fromDb = SaveAndLoadEntity(lang);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("English");

            fromDb.LocaleStringResources.ShouldNotBeNull();
            (fromDb.LocaleStringResources.Count == 1).ShouldBeTrue();
            fromDb.LocaleStringResources.First().ResourceName.ShouldEqual("ResourceName1");
        }
    }
}
