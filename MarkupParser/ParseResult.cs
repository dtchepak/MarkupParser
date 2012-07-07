namespace MarkupParser
{
    public class ParseResult<T>
    {
        public ParseResult(string remaining, T result)
        {
            Remaining = remaining;
            Value = result;
        }
        public string Remaining { get; private set; }
        public T Value { get; private set; }
    }
}