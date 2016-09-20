using System;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Forums
{
    [TestFixture]
    public class ForumPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_forum()
        {
            var forum = TestHelper.GetForum();
            forum.ForumGroup = TestHelper.GetForumGroup();

            var fromDb = SaveAndLoadEntity(forum);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Forum 1");
            fromDb.Description.ShouldEqual("Forum 1 Description");
            fromDb.DisplayOrder.ShouldEqual(10);
            fromDb.NumTopics.ShouldEqual(15);
            fromDb.NumPosts.ShouldEqual(25);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}