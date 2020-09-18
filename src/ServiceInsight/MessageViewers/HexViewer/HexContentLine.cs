namespace ServiceInsight.MessageViewers.HexViewer
{
    public struct HexContentLine
    {
        readonly byte[] data;

        public HexContentLine(byte[] data, int line)
            : this()
        {
            this.data = data;
            Line = line;
        }

        public int Line { get; }

        public byte? this[int i]
        {
            get
            {
                var index = (Line * 16) + i;
                if (index >= data.Length)
                {
                    return null;
                }

                return data[index];
            }
        }
    }
}