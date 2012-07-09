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
            var text = node.Children().First();
            var bold = node.Children().Skip(1).First();
            text.ToString().ShouldBe("(TEXT: hi )");
            bold.Children().Count().ShouldBe(1);
            bold.Children().First().ToString().ShouldBe("(TEXT: world)");
        }


        public class TreeNode
        {
            private readonly List<TreeNode> _children = new List<TreeNode>();
            public TreeNode Parent { get; set; }
            public IEnumerable<TreeNode> Children() { return _children; }
            public void Add(TreeNode node) { _children.Add(node); }
            public virtual bool IsEnd(char c) { return false; }
        }

        public class BoldTreeNode : TreeNode
        {
            public override bool IsEnd(char c) { return c == '*'; }
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
        }

        public TreeNode Parse(string s)
        {
            var root = new TreeNode();
            var stack = new Stack<TreeNode>();
            stack.Push(root);
            foreach (var c in s)
            {
                var current = stack.Peek();
                if (current.IsEnd(c))
                {
                    stack.Pop();
                }
                else if (IsStart(c))
                {
                    if (current.GetType() == typeof(TextTreeNode))
                    {
                        stack.Pop();
                        current = stack.Peek();
                    }
                    //create node of that type
                    var boldTreeNode = new BoldTreeNode();
                    current.Add(boldTreeNode);
                    stack.Push(boldTreeNode);
                }
                else
                {
                    if (current.GetType() != typeof(TextTreeNode))
                    {
                        var textNode = new TextTreeNode();
                        textNode.Add(c);
                        current.Add(textNode);
                        stack.Push(textNode);
                    }
                    else
                    {
                        var textNode = (TextTreeNode) current;
                        textNode.Add(c);
                    }
                }
            }

            return root;
        }

        private bool IsStart(char c)
        {
            return c == '*';
        }
    }
}