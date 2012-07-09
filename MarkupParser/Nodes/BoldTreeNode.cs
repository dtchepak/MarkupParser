namespace MarkupParser.Nodes
{
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
}