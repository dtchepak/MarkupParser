using System;
using System.Collections.Generic;

namespace MarkupParser.Nodes
{
    public static class NodeFactory
    {
        private static Dictionary<char,Func<INode>> _nodeTypes = new Dictionary<char, Func<INode>>();

        static NodeFactory()
        {
            RegisterNodeType('*', () => new BoldNode());
            RegisterNodeType('{', () => new BindingNode());
            RegisterNodeType('_', () => new ItalicNode());
        }

        public static void RegisterNodeType(char startCharacter, Func<INode> creationMethod)
        {
            _nodeTypes.Add(startCharacter, creationMethod);
        }

        public static bool CanCreateNodeFor(char c)
        {
            return _nodeTypes.ContainsKey(c);
        }

        public static INode CreateNodeOrText(char c)
        {
            Func<INode> createNode;
            if (!_nodeTypes.TryGetValue(c, out createNode))
            {
                createNode = () => new TextNode(c);
            }
            return createNode();
        }
    }
}