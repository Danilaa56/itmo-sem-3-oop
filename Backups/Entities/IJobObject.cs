namespace Backups.Entities
{
    public interface IJobObject
    {
        string FileName { get; }
        byte[] GetData();
    }
}