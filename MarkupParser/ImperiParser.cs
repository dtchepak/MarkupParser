using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace MarkupParser
{
    public class ImperiParser
    {
        [Test]
        public void PlainText()
        {
            var node = Parse("hello");

            node.Children().Count().ShouldBe(1);
            node.Children().First().ToString().ShouldBe("(TEXT: hello)");
        }

        [Test]
        public void BoldText()
        {
            var node = Parse("*hi*");

            node.Children().Count().ShouldBe(1);
            var bold = node.Children().First();
            bold.Children().Count().ShouldBe(1);
            bold.Children().First().ToString().ShouldBe("(TEXT: hi)");
        }

        [Test]
        public void BoldAndPlainText()
        {
            var node = Parse("hi *world*");

            node.Children().Count().ShouldBe(2);
            var bold = node.Children().Skip(1).First();
            var text = node.Children().First();
            text.ToString().ShouldBe("(TEXT: hi )");
            bold.Children().Count().ShouldBe(1);
            bold.Children().First().ToString().ShouldBe("(TEXT: world)");
        }

        [Test]
        public void BoldAndPlainText2()
        {
            var node = Parse("hi *world* 2");

            node.Children().Count().ShouldBe(3);
            var text = node.Children().First();
            var bold = node.Children().Skip(1).First();
            var text2 = node.Children().Skip(2).First();
            text.ToString().ShouldBe("(TEXT: hi )");
            bold.Children().Count().ShouldBe(1);
            bold.Children().First().ToString().ShouldBe("(TEXT: world)");
            text2.ToString().ShouldBe("(TEXT:  2)");
        }

        [Test]
        public void Bindings()
        {
            var node = Parse("{MyBinding}");

            node.Children().Count().ShouldBe(1);
            var binding = node.Children().First();
            binding.ToString().ShouldBe("(BINDING: MyBinding)");
        }

        [Test]
        public void PlainTextAndBindings()
        {
            var node = Parse("Text {MyBinding}");

            node.Children().Count().ShouldBe(2);
            var text = node.Children().First();
            var binding = node.Children().Skip(1).First();
            text.ToString().ShouldBe("(TEXT: Text )");
            binding.ToString().ShouldBe("(BINDING: MyBinding)");
        }

        [Test]
        public void BoldAndTextAndBinding()
        {
            const string expected = "(TEXT: hello )(BOLD: (TEXT: world and )(BINDING: binding))(TEXT: !)";

            var result = Parse("hello *world and {binding}*!");

            result.ToString().ShouldBe(expected);
        }

        [Test]
        public void InvalidInput()
        {
            var result = Parse("this is *unterminated bold");

            result.ToString().ShouldBe("(TEXT: this is )(BOLD: (TEXT: unterminated bold))");
        }


        public class TreeNode
        {
            public enum Result
            {
                Continue,
                Failed,
                Closed
            }

            private readonly List<TreeNode> _children = new List<TreeNode>();
            private TreeNode _currentChild;
            public TreeNode Parent { get; set; }
            public IEnumerable<TreeNode> Children() { return _children; }
            public void Add(TreeNode node)
            {
                _children.Add(node);
                node.Parent = this;
            }
            public virtual bool IsEnd(char c) { return false; }

            public virtual Result Parse(char c)
            {
                if (_currentChild == null)
                {
                    if (c == '*')
                    {
                        _currentChild = new BoldTreeNode();
                        Add(_currentChild);
                        return Result.Continue;
                    }

                    if (c == '{')
                    {
                        _currentChild = new BindingTreeNode();
                        Add(_currentChild);
                        return Result.Continue;
                    }

                    _currentChild = new TextTreeNode();
                    Add(_currentChild);
                }

                var result = _currentChild.Parse(c);

                if (result == Result.Closed)
                {
                    _currentChild = null;
                    return Result.Continue;
                }
                if (result == Result.Failed)
                {
                    _currentChild = null;
                    return Parse(c);
                }
                return result;
            }
            public override string ToString()
            {
                return string.Join("", Children().Select(x => x.ToString()));
            }
        }

        public class BoldTreeNode : TreeNode
        {
            public override Result Parse(char c)
            {
                return c == '*' ? Result.Closed : base.Parse(c);
            }

            public override string ToString()
            {
                return "(BOLD: " + base.ToString() + ")";
            }
        }

        public class BindingTreeNode : TextTreeNode
        {
            public override Result Parse(char c)
            {
                return c == '}' ? Result.Closed : base.Parse(c);
            }

            public override string ToString()
            {
                return "(BINDING: " + Value + ")";
            }
        }

        public class TextTreeNode : TreeNode
        {
            public TextTreeNode() { Value = ""; }
            public string Value { get; set; }
            public void Add(char c) { Value += c; }
            public override string ToString()
            {
                return "(TEXT: " + Value + ")";
            }

            public override bool IsEnd(char c)
            {
                return Parent.IsEnd(c);
            }

            public override Result Parse(char c)
            {
                if ((c == '*') || (c == '{'))
                {
                    return Result.Failed;
                }
                Value += c;
                return Result.Continue;
            }
        }

        public TreeNode Parse(string s)
        {
            var root = new TreeNode();

            foreach (var c in s)
            {
                root.Parse(c);
            }

            return root;
        }

        private bool IsStart(char c)
        {
            return c == '*';
        }
    }
}