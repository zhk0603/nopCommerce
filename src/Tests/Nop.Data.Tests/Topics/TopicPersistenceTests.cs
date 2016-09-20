using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Topics
{
    [TestFixture]
    public class TopicPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_topic()
        {
            var topic = TestHelper.GetTopic();

            var fromDb = SaveAndLoadEntity(topic);
            fromDb.ShouldNotBeNull();
            fromDb.SystemName.ShouldEqual("SystemName 1");
            fromDb.IncludeInSitemap.ShouldEqual(true);
            fromDb.IncludeInTopMenu.ShouldEqual(true);
            fromDb.IncludeInFooterColumn1.ShouldEqual(true);
            fromDb.IncludeInFooterColumn2.ShouldEqual(true);
            fromDb.IncludeInFooterColumn3.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);
            fromDb.AccessibleWhenStoreClosed.ShouldEqual(true);
            fromDb.IsPasswordProtected.ShouldEqual(true);
            fromDb.Password.ShouldEqual("password");
            fromDb.Title.ShouldEqual("Title 1");
            fromDb.Body.ShouldEqual("Body 1");
            fromDb.Published.ShouldEqual(true);
            fromDb.TopicTemplateId.ShouldEqual(1);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.MetaTitle.ShouldEqual("Meta title");
            fromDb.SubjectToAcl.ShouldEqual(true);
            fromDb.LimitedToStores.ShouldEqual(true);
        }
    }
}