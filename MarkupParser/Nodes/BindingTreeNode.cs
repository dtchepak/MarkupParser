namespace MarkupParser.Nodes
{
    public class BindingTreeNode : TreeNode
    {
        private string Value { get; set; }

        public override Result Parse(char c)
        {
            if (c == '}')
            {
                return Result.Closed;
            }
            Value += c;
            return Result.Continue;
        }

        public override string ToString()
        {
            return "(BINDING: " + Value + ")";
        }
    }
}