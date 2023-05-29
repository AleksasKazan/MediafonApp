using MediafonApp.Models;
using MediafonApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediafonApp.Repositories
{
    public class FileRecordRepository : IFileRecordRepository
    {
        private readonly MediafonDbContext _dbContext;
        private readonly ILogger<SftpFileCheckService> _logger;

        public FileRecordRepository(MediafonDbContext dbContext, ILogger<SftpFileCheckService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Adds a new record about downloaded file to the database
        public async Task AddFileRecordAsync(FileRecordModel fileRecord)
        {
            try
            {
                await _dbContext.Files.AddAsync(fileRecord);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AddFileRecordAsync.");
                throw;
            }
        }

        // Checks if a downloaded file record with the given file path and creation time exists in the database
        public async Task<bool> DoesFileRecordExistAsync(string filePath, DateTime creationTime)
        {
            try
            {
                var isFileExists = await _dbContext.Files.AnyAsync(f =>
                    f.FilePath == filePath && f.CreationTime == creationTime.ToUniversalTime());
                return isFileExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DoesFileRecordExistAsync.");
                throw;
            }
        }
    }
}