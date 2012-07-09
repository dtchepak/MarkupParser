using System;
using System.Collections.Generic;
using MarkupParser.Nodes;

namespace MarkupParser
{
    public static class NodeFactory
    {
        private static Dictionary<char,Func<TreeNode>> _nodeTypes = new Dictionary<char, Func<TreeNode>>();

        static NodeFactory()
        {
            RegisterNodeType('*', () => new BoldTreeNode());
            RegisterNodeType('{', () => new BindingTreeNode());
            RegisterNodeType('_', () => new ItalicTreeNode());
        }

        public static void RegisterNodeType(char startCharacter, Func<TreeNode> creationMethod)
        {
            _nodeTypes.Add(startCharacter, creationMethod);
        }

        public static bool CanCreateNodeFor(char c)
        {
            return _nodeTypes.ContainsKey(c);
        }

        public static TreeNode CreateNodeOrText(char c)
        {
            Func<TreeNode> createNode;
            if (!_nodeTypes.TryGetValue(c, out createNode))
            {
                createNode = () => new TextTreeNode(c);
            }
            return createNode();
        }
    }
}