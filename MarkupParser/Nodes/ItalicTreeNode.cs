namespace MarkupParser.Nodes
{
    public class ItalicTreeNode : TreeNode
    {
        public override Result Parse(char c)
        {
            return c == '_' ? Result.Closed : base.Parse(c);
        }

        public override string ToString()
        {
            return "(ITALIC: " + base.ToString() + ")";
        }
    }
}