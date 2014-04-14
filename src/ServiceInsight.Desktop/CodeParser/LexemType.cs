namespace NServiceBus.Profiler.Desktop.CodeParser
{
    public enum LexemType
    {
        Error,
        Block,
        Symbol,
        Object,
        Property,
        Value,
        Space,
        LineBreak,
        Complex,
        Comment,
        PlainText,
        String,
        KeyWord,
        Quotes,
    }
}