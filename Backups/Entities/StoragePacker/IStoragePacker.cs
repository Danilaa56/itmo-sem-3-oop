namespace Backups.Entities.StoragePacker
{
    public interface IStoragePacker
    {
        byte[] Pack(StorageBlank storageBlank);
        StorageBlank Unpack(byte[] packedData);
    }
}