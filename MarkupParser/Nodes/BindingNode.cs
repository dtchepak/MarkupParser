namespace MarkupParser.Nodes
{
    public class BindingNode : INode
    {
        private string Value { get; set; }

        public ParseResult Parse(char c)
        {
            if (c == '}')
            {
                return ParseResult.Closed;
            }
            Value += c;
            return ParseResult.Continue;
        }

        public override string ToString()
        {
            return "(BINDING: " + Value + ")";
        }
    }
}