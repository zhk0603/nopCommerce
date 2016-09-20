using System.Collections.Generic;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Messages
{
    [TestFixture]
    public class TokenizerTests : ServiceTest
    {
        [Test]
        public void Can_replace_tokens_case_sensitive()
        {
            var messageTemplatesSettings = new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false
            };
            var tokenizer = new Tokenizer(messageTemplatesSettings);

            var tokens = TestHelper.GeTokens();

            //correct case
            tokenizer
                .Replace("Some text %Token1%", tokens, false)
                .ShouldEqual("Some text <Value1>");
            //wrong case
            tokenizer
                .Replace("Some text %TOKeN1%", tokens, false)
                .ShouldEqual("Some text %TOKeN1%");
        }

        [Test]
        public void Can_replace_tokens_case_invariant()
        {
            var messageTemplatesSettings = new MessageTemplatesSettings
            {
                CaseInvariantReplacement = true
            };
            var tokenizer = new Tokenizer(messageTemplatesSettings);
            
            tokenizer
                .Replace("Some text %TOKEn1%", TestHelper.GeTokens(), false)
                .ShouldEqual("Some text <Value1>");
        }

        [Test]
        public void Can_html_encode()
        {
            var messageTemplatesSettings = new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false
            };
            var tokenizer = new Tokenizer(messageTemplatesSettings);
            
            tokenizer
                .Replace("Some text %Token1%", TestHelper.GeTokens(), true)
                .ShouldEqual("Some text &lt;Value1&gt;");
        }

        [Test]
        public void Should_not_html_encode_if_token_doesnt_allow_it()
        {
            var messageTemplatesSettings = new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false
            };
            var tokenizer = new Tokenizer(messageTemplatesSettings);
            
            tokenizer
                .Replace("Some text %Token1%", TestHelper.GeTokens(neverHtmlEncoded: true), true)
                .ShouldEqual("Some text <Value1>");
        }
    }
}
