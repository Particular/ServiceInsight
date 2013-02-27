using System.Collections.Generic;

namespace NServiceBus.Profiler.Desktop.MessageViewers.HexViewer
{
    public class HexPart
    {
        public HexPart(int lineNumber)
        {
            LineNumber = lineNumber;
            Numbers = new List<HexNumber>();
        }

        public long LineNumber { get; private set; }
        public List<HexNumber> Numbers { get; private set; }
    }
}