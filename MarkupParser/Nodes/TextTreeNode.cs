namespace MarkupParser.Nodes
{
    public class TextTreeNode : TreeNode
    {
        public TextTreeNode(char c) { Value = c.ToString(); }
        public string Value { get; set; }
        public void Add(char c) { Value += c; }
        public override string ToString()
        {
            return "(TEXT: " + Value + ")";
        }

        public override Result Parse(char c)
        {
            if (NodeFactory.CanCreateNodeFor(c))
            {
                return Result.Failed;
            }
            Value += c;
            return Result.Continue;
        }
    }
}