using System;
using System.Collections.Generic;
using Nop.Core.Domain.Blogs;
using Nop.Tests;
using NUnit.Framework;
using Nop.Services.Blogs;

namespace Nop.Services.Tests.Blogs
{
    [TestFixture]
    public class BlogExtensionsTests
    {
        [Test]
        public void can_Get_Posts_By_Date()
        {
            var dateFrom = new DateTime(2010, 01, 01);
            var dateTo = new DateTime(2010, 01, 02);

            var post1 = TestHelper.GetBlogPost();
            var post2 = TestHelper.GetBlogPost();
            var post3 = TestHelper.GetBlogPost();
            post3.StartDateUtc = new DateTime(2010, 04, 01);
            post3.CreatedOnUtc = new DateTime(2010, 01, 02);
            var posts = new List<BlogPost> {post1, post2, post3};
            //get posts by sate StartDateUtc
            var rez  = posts.GetPostsByDate(dateFrom, dateTo);
            rez.ShouldNotBeNull();
            rez.Count.ShouldEqual(2);

            //get posts by sate CreatedOnUtc
            post1.StartDateUtc = post2.StartDateUtc = post3.StartDateUtc = null;
            rez = posts.GetPostsByDate(new DateTime(2010, 01, 01), new DateTime(2010, 01, 02));
            rez.ShouldNotBeNull();
            rez.Count.ShouldEqual(1);
        }
    }
}
