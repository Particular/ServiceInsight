namespace NServiceBus.Profiler.Core.Management
{
    public interface IManagementService
    {
        object GetAuditMessages();
        bool IsAlive(string connectedToService);
    }
}