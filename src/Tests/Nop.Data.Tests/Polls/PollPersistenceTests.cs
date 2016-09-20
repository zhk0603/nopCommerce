using System;
using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Polls
{
    [TestFixture]
    public class PollPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_poll()
        {
            var poll = TestHelper.GetPoll();

            var fromDb = SaveAndLoadEntity(poll);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.SystemKeyword.ShouldEqual("SystemKeyword 1");
            fromDb.Published.ShouldEqual(true);
            fromDb.ShowOnHomePage.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);
            fromDb.StartDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.EndDateUtc.ShouldEqual(new DateTime(2010, 01, 02));

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }

        [Test]
        public void Can_save_and_load_poll_with_answers()
        {
            var poll = TestHelper.GetPoll();
            poll.PollAnswers.Add(TestHelper.GetPollAnswer());
            var fromDb = SaveAndLoadEntity(poll);
            fromDb.ShouldNotBeNull();

            fromDb.PollAnswers.ShouldNotBeNull();
            (fromDb.PollAnswers.Count == 1).ShouldBeTrue();
            fromDb.PollAnswers.First().Name.ShouldEqual("Answer 1");
            fromDb.PollAnswers.First().NumberOfVotes.ShouldEqual(1);
            fromDb.PollAnswers.First().DisplayOrder.ShouldEqual(2);
        }

        [Test]
        public void Can_save_and_load_poll_with_answer_and_votingrecord()
        {
            var poll = TestHelper.GetPoll();
            poll.PollAnswers.Add(TestHelper.GetPollAnswer());
            poll.PollAnswers.First().PollVotingRecords.Add(TestHelper.GetPollVotingRecord());
            var fromDb = SaveAndLoadEntity(poll);
            fromDb.ShouldNotBeNull();
            
            fromDb.PollAnswers.ShouldNotBeNull();
            (fromDb.PollAnswers.Count == 1).ShouldBeTrue();

            fromDb.PollAnswers.First().PollVotingRecords.ShouldNotBeNull();
            (fromDb.PollAnswers.First().PollVotingRecords.Count == 1).ShouldBeTrue();
        }
    }
}
