namespace Backups.Entities
{
    public interface IJobObject
    {
        public string FileName { get; }

        public byte[] GetData();
    }
}