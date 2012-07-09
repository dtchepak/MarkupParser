namespace MarkupParser.Nodes
{
    public class TextNode : INode
    {
        public TextNode(char c) { Value = c.ToString(); }
        public string Value { get; set; }
        public void Add(char c) { Value += c; }
        public override string ToString()
        {
            return "(TEXT: " + Value + ")";
        }

        public ParseResult Parse(char c)
        {
            if (NodeFactory.CanCreateNodeFor(c))
            {
                return ParseResult.Failed;
            }
            Value += c;
            return ParseResult.Continue;
        }
    }
}