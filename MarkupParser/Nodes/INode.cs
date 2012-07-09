namespace MarkupParser.Nodes
{
    public interface INode
    {
        ParseResult Parse(char c);
    }
}