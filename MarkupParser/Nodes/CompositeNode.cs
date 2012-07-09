using System.Collections.Generic;
using System.Linq;

namespace MarkupParser.Nodes
{
    public class CompositeNode : INode
    {
        private readonly List<INode> _children = new List<INode>();
        private INode _currentChild;

        public void Add(INode node)
        {
            _children.Add(node);
        }

        public virtual ParseResult Parse(char c)
        {
            if (_currentChild == null)
            {
                _currentChild = NodeFactory.CreateNodeOrText(c);
                Add(_currentChild);
                return ParseResult.Continue;
            }

            var result = _currentChild.Parse(c);

            if (result == ParseResult.Closed)
            {
                _currentChild = null;
                return ParseResult.Continue;
            }
            if (result == ParseResult.Failed)
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