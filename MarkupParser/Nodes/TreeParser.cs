namespace MarkupParser.Nodes
{
    public class TreeParser
    {
        public static INode Parse(string s)
        {
            var root = new CompositeNode();

            foreach (var c in s)
            {
                root.Parse(c);
            }

            return root;
        }
    }
}