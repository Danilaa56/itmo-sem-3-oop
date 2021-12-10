namespace Backups.Entities.DataObjects
{
    public interface IJobObject
    {
        BackupObject BackupObject { get; }
        byte[] GetData();
    }
}