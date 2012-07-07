using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkupParser
{
    public abstract class Node
    {
        public static Parser<String> TextParser()
        {
            return Parser.Satisfies(IsNotReserved).Many().Then(cs => Parser.Value(String.Join("", cs)));
        }

        private static bool IsReserved(char arg) { return "{}*".Any(arg.Equals); }
        private static bool IsNotReserved(char arg) { return !IsReserved(arg); }

        public static Parser<Node> TextNodeParser()
        {
            return TextParser().Then(s => Parser<Node>.Value(new TextNode(s)));
        }

        /*
         doc := { expr }
         expr := bold | text | binding
         bold := * {expr} *
         
         samples:
         "hello" = (text)
         "hello *world*" = (text )(bold)
         */

        public static Parser<Node> RootNodeParser()
        {
            //NOT RIGHT: return NodeParser().Many().Then(nodes => Parser<Node>.Value(new RootNode(nodes)));
            return null;
        }

        public static Parser<Node> NodeParser()
        {
            return BoldParser().Or(BindingParser()).Or(TextNodeParser());
        }

        public static Parser<Node> BoldParser()
        {
            var boldSymbolParser = Parser.Satisfies('*'.Equals);
            return boldSymbolParser
                .Then(c => BindingParser().Or(TextNodeParser())
                   .Then(nodes => boldSymbolParser
                     .Then(c2 => Parser<Node>.Value(new BoldNode(nodes))
                 ) ) );
        }

        public static Parser<Node> BindingParser()
        {
            return Parser.Satisfies('{'.Equals)
                .Then(c => TextParser()
                   .Then(text => Parser.Satisfies('}'.Equals)
                     .Then(c2 => Parser<Node>.Value(new BindingNode(text))
                 ) ) );
        }

        public static string ToString(IEnumerable<Node> nodes)
        {
            return String.Join("", nodes);
        }
    }

    public class TextNode : Node
    {
        public TextNode(string text) { Text = text; }
        public TextNode(IEnumerable<char> text) { Text = new String(text.ToArray()); }
        public string Text { get; private set; }
        public override string ToString() { return Text; }
    }
    public class RootNode : Node
    {
        public IEnumerable<Node> Nodes { get; private set; }
        public RootNode(IEnumerable<Node> nodes) { Nodes = nodes; }
        public override string ToString() { return "(ROOT: " + ToString(Nodes) + ")"; }
    }

    public class BoldNode : Node
    {
        public IEnumerable<Node> Nodes { get; private set; }
        public BoldNode(IEnumerable<Node> nodes) { Nodes = nodes; }
        public BoldNode(Node node) { Nodes = new[] { node }; }
        public override string ToString() { return "(BOLD: " + ToString(Nodes) + ")"; }
    }

    public class BindingNode : Node
    {
        public string BindingExpression { get; private set; }
        public BindingNode(string bindingExpression) { BindingExpression = bindingExpression; }
        public override string ToString() { return "(BINDING: " + BindingExpression + ")"; }
    }
}