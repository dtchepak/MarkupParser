using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkupParser
{
    public class Parser<T>
    {
        private readonly Func<string, ParseResult<T>> _parse;
        public Parser(Func<string, ParseResult<T>> parse) { _parse = parse; }
        public ParseResult<T> Parse(string s) { return _parse(s); }

        public static Parser<T> Fail() { return new Parser<T>(s => null); }
        public static Parser<T> Value(T value) { return Parser.Value(value); }

        public Parser<T> Or(Parser<T> alternate)
        {
            return new Parser<T>(s => Parse(s) ?? alternate.Parse(s));
        }

        public Parser<T1> Then<T1>(Func<T, Parser<T1>> getNextParser)
        {
            return new Parser<T1>(s =>
            {
                var firstResult = Parse(s);
                if (firstResult == null) { return null; }
                var nextParser = getNextParser(firstResult.Value);
                return nextParser.Parse(firstResult.Remaining);
            });
        }

        public Parser<IEnumerable<T>> AtLeastOne()
        {
            return this
                .Then(x => Many()
                .Then(xs => Parser.Value((new[] { x }).Concat(xs))));
        }

        public Parser<IEnumerable<T>> Many() {
            //return AtLeastOne().Or(Parser<IEnumerable<T>>.Value(new T[0]));
            return new Parser<IEnumerable<T>>(ParseMultiple); 
        }

        private ParseResult<IEnumerable<T>> ParseMultiple(string input)
        {
            var remainingInput = input;
            var values = new List<T>();
            while (remainingInput != "")
            {
                var result = Parse(remainingInput);
                if (result == null) break;
                values.Add(result.Value);
                remainingInput = result.Remaining;
            }
            return new ParseResult<IEnumerable<T>>(remainingInput, values);
        }
    }

    public class Parser
    {
        public static Parser<T> Value<T>(T value)
        {
            return new Parser<T>(s => new ParseResult<T>(s, value));
        }
        public static Parser<char> Char()
        {
            return new Parser<char>(s => string.IsNullOrEmpty(s) ? null : new ParseResult<char>(s.Substring(1), s[0]));
        }
        public static Parser<char> Satisfies(Func<char, bool> predicate)
        {
            return Char().Then(c => predicate(c) ? Value(c) : Parser<char>.Fail());
        }
        public static Parser<char> Is(char c) { return Satisfies(c.Equals); }
        public static Parser<char> IsNot(char c) { return Satisfies(parsed => c != parsed); }
        public static Parser<string> DelimitedText(char delimiter)
        {
            return DelimitedText(delimiter, delimiter);
        }
        public static Parser<string> DelimitedText(char start, char end)
        {
            return new Parser<string>(s =>
                                          {
                                              var pattern = string.Format(@"^{0}([^{1}]*)?{1}?(.*)$", Regex.Escape(start.ToString()), Regex.Escape(end.ToString()));
                                              var regex = new Regex(pattern);
                                              var match = regex.Match(s);
                                              var result = match.Groups[1].Value;
                                              return String.IsNullOrEmpty(result) ? null : new ParseResult<string>(match.Groups[2].Value, result);
                                          });
        }
    }
}