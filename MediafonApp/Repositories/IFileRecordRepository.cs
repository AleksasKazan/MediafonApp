using MediafonApp.Models;

namespace MediafonApp.Repositories
{
    public interface IFileRecordRepository
    {
        Task AddFileRecordAsync(FileRecordModel file);
        Task<bool> DoesFileRecordExistAsync(string filePath, DateTime creationTime);
    }
}
