namespace Particular.ServiceInsight.Desktop.CodeParser
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{Remaining}")]
    public class SourcePart
    {
        string Source
        {
            get; set;
        }

        int StartIndex
        {
            get; set;
        }

        string Remaining
        {
            get { return Source.Substring(StartIndex); }
        }

        public int Length
        {
            get { return Source.Length - StartIndex; }
        }

        public char this[int index]
        {
            get { return Source[StartIndex + index]; }
        }

        public SourcePart(string source, int startIndex)
        {
            if (startIndex > source.Length)
                throw new ArgumentException();
        
            Source = source;
            StartIndex = startIndex;
        }

        public int IndexOf(string text)
        {
            var index = Source.IndexOf(text, StartIndex, StringComparison.Ordinal);
            if (index < 0)
                return index;
            
            return index - StartIndex;
        }

        public int IndexOf(string value, int startIndex)
        {
            var index = Source.IndexOf(value, StartIndex + startIndex, StringComparison.Ordinal);
            if (index < 0)
                return index;
            
            return index - StartIndex;
        }

        public int IndexOfAny(char[] anyOf)
        {
            var index = Source.IndexOfAny(anyOf, StartIndex);
            if (index < 0)
                return index;
            
            return index - StartIndex;
        }

        public int IndexOf(char value)
        {
            var index = Source.IndexOf(value, StartIndex);
            if (index < 0)
                return index;
            
            return index - StartIndex;
        }

        public string Substring(int startIndex, int length)
        {
            return Source.Substring(StartIndex + startIndex, length);
        }

        public SourcePart Substring(int startIndex)
        {
            return new SourcePart(Source, StartIndex + startIndex);
        }

        public bool StartsWith(string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            return String.Compare(Source, StartIndex, value, 0, value.Length, comparisonType) == 0;
        }
    }
}