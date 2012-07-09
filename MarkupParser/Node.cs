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

        private static bool IsReserved(char arg) { return "{}*_".Any(arg.Equals); }
        private static bool IsNotReserved(char arg) { return !IsReserved(arg); }

        public static Parser<Node> TextNodeParser()
        {
            return TextParser().Then(s => Parser<Node>.Value(new TextNode(s)));
        }

        public static Parser<Node> NodeParser()
        {
            return BoldParser().Or(ItalicsParser).Or(BindingParser).Or(TextNodeParser);
        }

        public static Parser<Node> BoldParser()
        {
            return Parser.DelimitedText('*').Then(innerText => Parser<Node>.Value(new BoldNode(NodeParser().Many().Parse(innerText).Value)));
        }

        public static Parser<Node> BindingParser()
        {
            return Parser.DelimitedText('{', '}').Then(text => Parser<Node>.Value(new BindingNode(text)));
        }

        public static string ToString(IEnumerable<Node> nodes)
        {
            return String.Join("", nodes);
        }

        public static Parser<Node> ItalicsParser()
        {
            return Parser.DelimitedText('_').Then(innerText => Parser<Node>.Value(new ItalicsNode(NodeParser().Many().Parse(innerText).Value)));
        }
    }

    public class TextNode : Node
    {
        public TextNode(string text) { Text = text; }
        public TextNode(IEnumerable<char> text) { Text = new String(text.ToArray()); }
        public string Text { get; private set; }
        public override string ToString() { return Text; }
    }
    public class BoldNode : Node
    {
        public IEnumerable<Node> Nodes { get; private set; }
        public BoldNode(IEnumerable<Node> nodes)
        {
            Nodes = nodes;
        }
        public BoldNode(Node node) { Nodes = new[] { node }; }
        public override string ToString() { return "(BOLD: " + ToString(Nodes) + ")"; }
    }
    public class ItalicsNode : Node
    {
        public IEnumerable<Node> Nodes { get; private set; }
        public ItalicsNode(IEnumerable<Node> nodes)
        {
            Nodes = nodes;
        }
        public ItalicsNode(Node node) { Nodes = new[] { node }; }
        public override string ToString() { return "(ITALICS: " + ToString(Nodes) + ")"; }
    }

    public class BindingNode : Node
    {
        public string BindingExpression { get; private set; }
        public BindingNode(string bindingExpression) { BindingExpression = bindingExpression; }
        public override string ToString() { return "(BINDING: " + BindingExpression + ")"; }
    }
}