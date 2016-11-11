using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Services.Blogs;
using Nop.Services.Events;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Blogs
{
    [TestFixture]
    class BlogServiceTests: ServiceTest
    {
        private BlogService _blogService;

        [SetUp]
        public new void SetUp()
        {
            var blogPostRepository = MockRepository.GenerateMock<IRepository<BlogPost>>();
            var blogCommentRepository = MockRepository.GenerateMock<IRepository<BlogComment>>();
            var storeMappingRepository = MockRepository.GenerateMock<IRepository<StoreMapping>>();
            var catalogSettings = new CatalogSettings
            {
                IgnoreStoreLimitations = false
            };
            var eventPublisher = MockRepository.GenerateMock<IEventPublisher>();

            var blogPost1 = TestHelper.GetBlogPost();
            blogPost1.Id = 1;
            blogPost1.Tags += ", Tags 2";

            var blogPost2 = TestHelper.GetBlogPost();
            blogPost2.Id = 2;
            blogPost2.LanguageId = 1;

            var blogPost3 = TestHelper.GetBlogPost();
            blogPost3.Id = 3;
            blogPost3.Tags += ", Tags 3";

            blogPost1.StartDateUtc = blogPost2.StartDateUtc = DateTime.UtcNow;
            blogPost1.EndDateUtc = blogPost2.EndDateUtc = DateTime.UtcNow.AddDays(1);

            var blogPosts = TestHelper.ToIQueryable(blogPost1, blogPost2, blogPost3);

            var blogComment1 = TestHelper.GetBlogComment();
            blogComment1.Id = 1;
            blogComment1.CustomerId = 1;
            var blogComment2 = TestHelper.GetBlogComment();
            blogComment2.Id = 2;
            var blogComment3 = TestHelper.GetBlogComment();
            blogComment3.Id = 3;

            var blogComments = TestHelper.ToIQueryable(blogComment1, blogComment2, blogComment3);

            var storeMapping1 = TestHelper.GetStoreMapping();
            storeMapping1.EntityId = 1;
            storeMapping1.EntityName = "BlogPost";
            storeMapping1.StoreId = 1;

            var storeMappings = TestHelper.ToIQueryable(storeMapping1);

            blogPostRepository.Expect(x => x.GetById(1)).Return(blogPost1);
            blogPostRepository.Expect(x => x.Table).Return(blogPosts);

            storeMappingRepository.Expect(x => x.Table).Return(storeMappings);
            blogCommentRepository.Expect(x => x.Table).Return(blogComments);
            blogCommentRepository.Expect(x => x.GetById(1)).Return(blogComment1);
                
            _blogService = new BlogService(blogPostRepository, blogCommentRepository, storeMappingRepository, catalogSettings, eventPublisher);
        }

        [Test]
        public void can_get_blog_post_by_id()
        {
            _blogService.GetBlogPostById(0).ShouldBeNull();
            var blogPost = _blogService.GetBlogPostById(1);
            blogPost.ShouldNotBeNull();
            blogPost.Title.ShouldEqual("Title 1");
        }

        [Test]
        public void can_get_blog_posts_by_ids()
        {
            _blogService.GetBlogPostsByIds(new[] {4, 5}).ShouldNotBeNull().Count.ShouldEqual(0);
            var blogPost = _blogService.GetBlogPostsByIds(new[] { 1, 2 });
            blogPost.ShouldNotBeNull().Count.ShouldEqual(2);
        }

        [Test]
        public void can_get_all_blog_posts()
        {
            //all without hidden
            _blogService.GetAllBlogPosts().ShouldNotBeNull().Count.ShouldEqual(2);
            //all with hidden
            _blogService.GetAllBlogPosts(showHidden: true).ShouldNotBeNull().Count.ShouldEqual(3);
            //filter by store
            _blogService.GetAllBlogPosts(1).ShouldNotBeNull().Count.ShouldEqual(1);
            //filter by language
            _blogService.GetAllBlogPosts(languageId: 1).ShouldNotBeNull().Count.ShouldEqual(1);
            //filter by date to
            _blogService.GetAllBlogPosts(dateTo:DateTime.UtcNow.AddDays(-1)).ShouldNotBeNull().Count.ShouldEqual(0);
            //filter by date from
            _blogService.GetAllBlogPosts(dateFrom: DateTime.UtcNow.AddDays(2)).ShouldNotBeNull().Count.ShouldEqual(0);
        }

        [Test]
        public void can_get_all_blog_posts_by_tag()
        {
            //all without hidden
            _blogService.GetAllBlogPostsByTag(tag: "Tags 1").ShouldNotBeNull().Count.ShouldEqual(2);
            //all with hidden
            _blogService.GetAllBlogPostsByTag(tag: "Tags 1", showHidden: true).ShouldNotBeNull().Count.ShouldEqual(3);
            //filter by store
            _blogService.GetAllBlogPostsByTag(1, tag: "Tags 1").ShouldNotBeNull().Count.ShouldEqual(1);
            //filter by language
            _blogService.GetAllBlogPostsByTag(languageId: 1, tag: "Tags 1").ShouldNotBeNull().Count.ShouldEqual(1);
            //filter by tag
            _blogService.GetAllBlogPostsByTag(tag: "Tags 2").ShouldNotBeNull().Count.ShouldEqual(1);
        }

        [Test]
        public void can_get_all_blog_post_tags()
        {
            //all without hidden
            _blogService.GetAllBlogPostTags(0, 0).ShouldNotBeNull().Count.ShouldEqual(2);
            //all with hidden
            _blogService.GetAllBlogPostTags(0, 0, true).ShouldNotBeNull().Count.ShouldEqual(3);
            //filter by store
            _blogService.GetAllBlogPostTags(1, 0).ShouldNotBeNull().Count.ShouldEqual(2);
            //filter by language
            _blogService.GetAllBlogPostTags(0, 1).ShouldNotBeNull().Count.ShouldEqual(1);
        }

        [Test]
        public void can_get_all_comments()
        {
            //all
            _blogService.GetAllComments(0).ShouldNotBeNull().Count.ShouldEqual(3);
            //filter by customer
            _blogService.GetAllComments(1).ShouldNotBeNull().Count.ShouldEqual(1);
        }

        [Test]
        public void can_get_blog_comment_by_id()
        {
            _blogService.GetBlogCommentById(0).ShouldBeNull();
            _blogService.GetBlogCommentById(1).ShouldNotBeNull().Customer.ShouldNotBeNull().SystemName.ShouldEqual("SystemName 1");
        }

        [Test]
        public void can_get_blog_comments_by_ids()
        {
            _blogService.GetBlogPostsByIds(new[] { 1 }).ShouldNotBeNull().Count.ShouldEqual(1);
            _blogService.GetBlogPostsByIds(new[] { 1, 2, 3 }).ShouldNotBeNull().Count.ShouldEqual(3);
            _blogService.GetBlogPostsByIds(new[] { 1, 3, 5, 6 }).ShouldNotBeNull().Count.ShouldEqual(2);
        }
    }
}
