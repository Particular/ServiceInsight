namespace Particular.ServiceInsight.Desktop.MessageViewers.HexViewer
{
    public class HexContentLine
    {
        private readonly byte[] data;

        public HexContentLine(byte[] data, int line)
        {
            this.data = data;
            Line = line;
        }

        public int Line { get; private set; }

        public byte this[int i]
        {
            get { return data[Line * 16 + i]; }
        }
    }
}