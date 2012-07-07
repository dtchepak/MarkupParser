using System;
using System.Collections.Generic;
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
            var result = Node.TextParser().Parse("hello world");

            result.Remaining.ShouldBeEmpty();
            result.Value.ToString().ShouldBe("hello world");
        }

        [Test]
        public void OrCombinatorWhenFirstParserSucceeds()
        {
            var parser = Node.Int().Or(Node.TextParser());
            var result = parser.Parse("1hello");

            result.Remaining.ShouldBe("hello");
            result.Value.ToString().ShouldBe(new IntNode(1).ToString());
        }

        [Test]
        public void OrCombinatorWhenFirstParserFails()
        {
            var parser = Node.Int().Or(Node.TextParser());
            var result = parser.Parse("hello");

            result.Remaining.ShouldBeEmpty();
            result.Value.ToString().ShouldBe("hello");
        }

        [Test]
        public void ListParser()
        {
            var parser = Node.Int().List();
            var result = parser.Parse("123abc");

            result.Remaining.ShouldBe("abc");
            result.Value.Select(x => ((IntNode) x).Value).ShouldBe(new[] {1,2,3});
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
        public void BoldAndText()
        {
            var text = "Hello *world*";
            var result = Node.NodeParser().Parse(text);

            result.Value.ToString().ShouldBe("asdf");
        }
    }

    public abstract class Node
    {
        public static Parser<Node> TextParser()
        {
            return Parser.Satisfies(c => c != '*').List().Bind(s => Parser<Node>.Value(new TextNode(s)));
        }

        public static Parser<Node> Int()
        {
            return new Parser<Node>(s =>
            {
                if (!string.IsNullOrEmpty(s) && Char.IsDigit(s[0]))
                {
                    return Result(s.Substring(1), new IntNode(int.Parse(char.ToString(s[0]))));
                }
                return null;
            });
        }

        public static Parser<Node> RootNodeParser()
        {
            //NOT RIGHT: return NodeParser().List().Bind(nodes => Parser<Node>.Value(new RootNode(nodes)));
            return null;
        }

        public static Parser<Node> NodeParser()
        {
            return BoldParser().Or(TextParser());
        }

        public static Parser<Node> BoldParser()
        {
            var boldSymbolParser = Parser.Satisfies(c => c == '*');
            return boldSymbolParser
                .Bind(c => NodeParser().List()
                    .Bind(nodes => boldSymbolParser
                        .Bind(c2 => Parser<Node>.Value(new BoldNode(nodes))
                        )
                    )
                );
        }

        private static ParseResult<Node> Result(string rest, Node node)
        {
            return new ParseResult<Node>(rest, node);
        }

    }
    public class TextNode : Node
    {
        public TextNode(string text) { Text = text; }

        public TextNode(IEnumerable<char> text) { Text = new String(text.ToArray()); }

        public string Text { get; private set; }
        public override string ToString() { return Text; }
    }
    public class IntNode : Node
    {
        public IntNode(int i) { Value = i; }
        public int Value { get; private set; }
        public override string ToString() { return "(INT: " + Value + ")"; }
    }

    public class RootNode : Node
    {
        public IEnumerable<Node> Nodes { get; private set; }
        public RootNode(IEnumerable<Node> nodes) { Nodes = nodes; }
        public override string ToString() { return "(ROOT: " + Nodes.SelectMany(x => x.ToString()) + ")"; }
    }

    public class BoldNode : Node
    {
        public IEnumerable<Node> Nodes { get; private set; }
        public BoldNode(IEnumerable<Node> nodes) { Nodes = nodes; }
        public override string ToString() { return "(BOLD: " + Nodes.SelectMany(x => x.ToString()) + ")"; }
    }

    public class BindingNode : Node
    {
        public string BindingExpression { get; private set; }
        public BindingNode(string bindingExpression) { BindingExpression = bindingExpression; }
    }
}
