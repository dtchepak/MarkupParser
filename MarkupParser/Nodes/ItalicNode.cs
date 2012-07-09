namespace MarkupParser.Nodes
{
    public class ItalicNode : CompositeNode
    {
        public override ParseResult Parse(char c)
        {
            return c == '_' ? ParseResult.Closed : base.Parse(c);
        }

        public override string ToString()
        {
            return "(ITALIC: " + base.ToString() + ")";
        }
    }
}