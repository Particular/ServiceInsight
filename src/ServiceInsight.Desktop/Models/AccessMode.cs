namespace NServiceBus.Profiler.Desktop.Models
{
    public enum AccessMode
    {
        Receive = 1,
        Send = 2,
        SendAndReceive = 3,
        Peek = 32,
        ReceiveAndAdmin = 129,
        PeekAndAdmin = 160,
    }
}