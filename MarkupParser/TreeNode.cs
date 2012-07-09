using System.Collections.Generic;
using System.Linq;

namespace MarkupParser
{
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

        public void Add(TreeNode node)
        {
            _children.Add(node);
            node.Parent = this;
        }

        public static TreeNode Parse(string s)
        {
            var root = new TreeNode();

            foreach (var c in s)
            {
                root.Parse(c);
            }

            return root;
        }

        public virtual Result Parse(char c)
        {
            if (_currentChild == null)
            {
                _currentChild = NodeFactory.CreateNodeOrText(c);
                Add(_currentChild);
                return Result.Continue;
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
            return string.Join("", _children.Select(x => x.ToString()));
        }
    }
}