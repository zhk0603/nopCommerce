using System;
using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.News
{
    [TestFixture]
    public class NewsPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_newsItem()
        {
            var news = TestHelper.GetNewsItem();

            var fromDb = SaveAndLoadEntity(news);
            fromDb.ShouldNotBeNull();
            fromDb.Title.ShouldEqual("Title 1");
            fromDb.Short.ShouldEqual("Short 1");
            fromDb.Full.ShouldEqual("Full 1");
            fromDb.Published.ShouldEqual(true);
            fromDb.StartDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.EndDateUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.AllowComments.ShouldEqual(true);
            fromDb.CommentCount.ShouldEqual(1);
            fromDb.LimitedToStores.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 03));
            fromDb.MetaTitle.ShouldEqual("MetaTitle 1");
            fromDb.MetaDescription.ShouldEqual("MetaDescription 1");
            fromDb.MetaKeywords.ShouldEqual("MetaKeywords 1");

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }

        [Test]
        public void Can_save_and_load_newsItem_with_comments()
        {
            var news = TestHelper.GetNewsItem();

            news.NewsComments.Add(TestHelper.GetNewsComment());
            var fromDb = SaveAndLoadEntity(news);
            fromDb.ShouldNotBeNull();

            fromDb.NewsComments.ShouldNotBeNull();
            (fromDb.NewsComments.Count == 1).ShouldBeTrue();
            fromDb.NewsComments.First().CommentText.ShouldEqual("Comment text 1");
        }
    }
}
