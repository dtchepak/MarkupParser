using System;
using System.Collections.Generic;

namespace MarkupParser
{
    public class Parser<T>
    {
        private readonly Func<string, ParseResult<T>> _parse;

        public Parser(Func<string, ParseResult<T>> parse)
        {
            _parse = parse;
        }

        public ParseResult<T> Parse(string s)
        {
            return _parse(s);
        }

        public Parser<T> Or(Parser<T> alternate)
        {
            return new Parser<T>(s =>
            {
                var firstParse = Parse(s);
                return firstParse ?? alternate.Parse(s);
            });
        }

        public Parser<T1> Bind<T1>(Func<T, Parser<T1>> getNextParser)
        {
            return new Parser<T1>(s =>
            {
                var firstResult = Parse(s);
                if (firstResult == null) return null;
                var nextParser = getNextParser(firstResult.Value);
                return nextParser.Parse(firstResult.Remaining);
            });
        }

        public Parser<IEnumerable<T>> List()
        {
            return new Parser<IEnumerable<T>>(ParseMultiple);
        }

        private ParseResult<IEnumerable<T>> ParseMultiple(string input)
        {
            ParseResult<T> result;
            var remainingInput = input;
            var values = new List<T>();
            while ((result = Parse(remainingInput)) != null)
            {
                values.Add(result.Value);
                remainingInput = result.Remaining;
            }
            return new ParseResult<IEnumerable<T>>(remainingInput, values);
        }

        public static Parser<T> Fail()
        {
            return new Parser<T>(s => null);
        }

        public static Parser<T> Value(T value)
        {
            return new Parser<T>(s => new ParseResult<T>(s, value));
        }
    }

    public class Parser
    {
        public static Parser<char> Char()
        {
            return new Parser<char>(s => string.IsNullOrEmpty(s) ? null : new ParseResult<char>(s.Substring(1), s[0]));
        }

        public static Parser<char> Satisfies(Func<char, bool> predicate)
        {
            return Char().Bind(c => predicate(c) ? Parser<char>.Value(c) : Parser<char>.Fail());
        }
    }
}