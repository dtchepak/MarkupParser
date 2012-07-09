using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace MarkupParser
{
    public class ParserTests
    {
        [Test]
        public void TextParsing()
        {
            var result = Node.TextNodeParser().Parse("hello world");

            result.Remaining.ShouldBeEmpty();
            result.Value.ToString().ShouldBe("hello world");
        }

        [Test]
        public void OrCombinatorWhenFirstParserSucceeds()
        {
            var digitParser = Parser.Satisfies(char.IsDigit);
            var alphaStringParser = Parser.Satisfies(char.IsLetter);
            var parser = digitParser.Or(alphaStringParser);
            var result = parser.Parse("1hello");

            result.Remaining.ShouldBe("hello");
            result.Value.ShouldBe('1');
        }

        [Test]
        public void OrCombinatorWhenFirstParserFails()
        {
            var digitParser = Parser.Satisfies(char.IsDigit);
            var alphaStringParser = Parser.Satisfies(char.IsLetter);
            var parser = digitParser.Or(alphaStringParser);
            var result = parser.Parse("hello");

            result.Remaining.ShouldBe("ello");
            result.Value.ShouldBe('h');
        }

        [Test]
        public void AtLeastOneParser()
        {
            var parser = Parser.Satisfies(char.IsDigit).AtLeastOne();
            var result = parser.Parse("123abc");
            result.Remaining.ShouldBe("abc");
            result.Value.ShouldBe("123");
        }

        [Test]
        public void IsSatisfied()
        {
            var digitParser = Parser.Satisfies(char.IsDigit);
            var validResult = digitParser.Parse("1a");
            var invalidResult = digitParser.Parse("a");
            validResult.Value.ShouldBe('1');
            validResult.Remaining.ShouldBe("a");
            invalidResult.ShouldBe(null);
        }

        [Test]
        public void ParseAllCharacters()
        {
            const string text = "hello";
            var parser = Parser.Char().Many();
            var result = parser.Parse(text);
            result.Value.ShouldBe("hello");
        }

        [Test]
        public void Binding()
        {
            var result = Node.BindingParser().Parse("{binding}");
            result.Value.ToString().ShouldBe("(BINDING: binding)");
        }

        [Test]
        public void BindingAndText()
        {
            var parser = Node.BindingParser().Or(Node.TextNodeParser()).Many();
            var result = parser.Parse("this {is} a test");
            Node.ToString(result.Value).ShouldBe("this (BINDING: is) a test");
            result.Value.Count().ShouldBe(3);
        }

        [Test]
        public void DelimitedTextWithStartAndEndDelimiters()
        {
            var p = Parser.DelimitedText('<', '>');
            var result = p.Parse("<hi> and other stuff");
            result.Value.ShouldBe("hi");
        }

        [Test]
        public void DelimitedTextWithSameStartAndEndDelimiter()
        {
            var p = Parser.DelimitedText('*');
            var result = p.Parse("*hi* and other stuff");
            result.Value.ShouldBe("hi");
        }

        [Test]
        public void TryParseDelimitedTextFromUndelimitedInput()
        {
            var p = Parser.DelimitedText('*');
            var result = p.Parse("asdf");
            result.ShouldBe(null);
        }

        [Test]
        public void TryParseDelimitedTextWithoutTerminatingDelimiter()
        {
            var p = Parser.DelimitedText('*');
            var result = p.Parse("*a");
            result.Value.ShouldBe("a");
            result.Remaining.ShouldBeEmpty();
        }

        [Test]
        public void Bold()
        {
            var result = Node.BoldParser().Parse("*hello*");
            result.Value.ToString().ShouldBe("(BOLD: hello)");
        }

        [Test]
        public void BoldAndText()
        {
            const string text = "Hello *world*";
            var result = Node.NodeParser().Many().Parse(text);
            Node.ToString(result.Value).ShouldBe("Hello (BOLD: world)");
        }

        [Test]
        public void BoldAndTextAndBinding()
        {
            const string expected = "hello (BOLD: world and (BINDING: binding))!";

            var result = Node.NodeParser().Many().Parse("hello *world and {binding}*!");

            Node.ToString(result.Value).ShouldBe(expected);
        }

        [Test]
        public void InvalidInput()
        {
            var result = Node.NodeParser().Many().Parse("this is *unterminated bold");

            Node.ToString(result.Value).ShouldBe("this is (BOLD: unterminated bold)");
        }
    }
}
