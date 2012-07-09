using MarkupParser.Nodes;
using NUnit.Framework;
using Shouldly;

namespace MarkupParser
{
    public class TreeNodeFixture
    {
        [Test]
        public void PlainText()
        {
            var node = TreeParser.Parse("hello");
            node.ToString().ShouldBe("(TEXT: hello)");
        }

        [Test]
        public void BoldText()
        {
            var node = TreeParser.Parse("*hi*");
            node.ToString().ShouldBe("(BOLD: (TEXT: hi))");
        }

        [Test]
        public void BoldAndPlainText()
        {
            var node = TreeParser.Parse("hi *world*");
            node.ToString().ShouldBe("(TEXT: hi )(BOLD: (TEXT: world))");
        }

        [Test]
        public void ItalicText()
        {
            var node = TreeParser.Parse("_hi_");
            node.ToString().ShouldBe("(ITALIC: (TEXT: hi))");
        }

        [Test]
        public void ItalicAndPlainText()
        {
            var node = TreeParser.Parse("hi _world_");
            node.ToString().ShouldBe("(TEXT: hi )(ITALIC: (TEXT: world))");
        }

        [Test]
        public void BoldAndPlainText2()
        {
            var node = TreeParser.Parse("hi *world* 2");
            node.ToString().ShouldBe("(TEXT: hi )(BOLD: (TEXT: world))(TEXT:  2)");
        }

        [Test]
        public void Bindings()
        {
            var node = TreeParser.Parse("{MyBinding}");
            node.ToString().ShouldBe("(BINDING: MyBinding)");
        }

        [Test]
        public void PlainTextAndBindings()
        {
            var node = TreeParser.Parse("Text {MyBinding}");
            node.ToString().ShouldBe("(TEXT: Text )(BINDING: MyBinding)");
        }
        
        [Test]
        public void BoldAndTextAndBinding()
        {
            var result = TreeParser.Parse("hello *world _and_ {binding}*!");
            result.ToString().ShouldBe("(TEXT: hello )(BOLD: (TEXT: world )(ITALIC: (TEXT: and))(TEXT:  )(BINDING: binding))(TEXT: !)");
        }

        [Test]
        public void InvalidInput()
        {
            var result = TreeParser.Parse("this is *unterminated bold");
            result.ToString().ShouldBe("(TEXT: this is )(BOLD: (TEXT: unterminated bold))");
        }
    }
}