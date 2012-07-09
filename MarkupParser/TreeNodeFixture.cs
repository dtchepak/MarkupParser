using NUnit.Framework;
using Shouldly;

namespace MarkupParser
{
    public class TreeNodeFixture
    {
        [Test]
        public void PlainText()
        {
            var node = TreeNode.Parse("hello");
            node.ToString().ShouldBe("(TEXT: hello)");
        }

        [Test]
        public void BoldText()
        {
            var node = TreeNode.Parse("*hi*");
            node.ToString().ShouldBe("(BOLD: (TEXT: hi))");
        }

        [Test]
        public void BoldAndPlainText()
        {
            var node = TreeNode.Parse("hi *world*");
            node.ToString().ShouldBe("(TEXT: hi )(BOLD: (TEXT: world))");
        }

        [Test]
        public void ItalicText()
        {
            var node = TreeNode.Parse("_hi_");
            node.ToString().ShouldBe("(ITALIC: (TEXT: hi))");
        }

        [Test]
        public void ItalicAndPlainText()
        {
            var node = TreeNode.Parse("hi _world_");
            node.ToString().ShouldBe("(TEXT: hi )(ITALIC: (TEXT: world))");
        }

        [Test]
        public void BoldAndPlainText2()
        {
            var node = TreeNode.Parse("hi *world* 2");
            node.ToString().ShouldBe("(TEXT: hi )(BOLD: (TEXT: world))(TEXT:  2)");
        }

        [Test]
        public void Bindings()
        {
            var node = TreeNode.Parse("{MyBinding}");
            node.ToString().ShouldBe("(BINDING: MyBinding)");
        }

        [Test]
        public void PlainTextAndBindings()
        {
            var node = TreeNode.Parse("Text {MyBinding}");
            node.ToString().ShouldBe("(TEXT: Text )(BINDING: MyBinding)");
        }
        
        [Test]
        public void BoldAndTextAndBinding()
        {
            var result = TreeNode.Parse("hello *world _and_ {binding}*!");
            result.ToString().ShouldBe("(TEXT: hello )(BOLD: (TEXT: world )(ITALIC: (TEXT: and))(TEXT:  )(BINDING: binding))(TEXT: !)");
        }

        [Test]
        public void InvalidInput()
        {
            var result = TreeNode.Parse("this is *unterminated bold");
            result.ToString().ShouldBe("(TEXT: this is )(BOLD: (TEXT: unterminated bold))");
        }
    }
}