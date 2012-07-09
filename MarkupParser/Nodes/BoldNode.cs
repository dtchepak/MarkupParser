namespace MarkupParser.Nodes
{
    public class BoldNode : CompositeNode
    {
        public override ParseResult Parse(char c)
        {
            return c == '*' ? ParseResult.Closed : base.Parse(c);
        }

        public override string ToString()
        {
            return "(BOLD: " + base.ToString() + ")";
        }
    }
}