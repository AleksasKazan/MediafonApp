using MediafonApp.Models;
using MediafonApp.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace MediafonApp.Services
{
    public class SftpFileCheckService : BackgroundService // Provides a convenient framework for building long-running background tasks or services
    {
        private readonly SftpConfiguration _sftpConfiguration;
        private readonly IFileRecordRepository _fileRecordRepository;
        private readonly MediafonDbContext _dbContext;
        private readonly ILogger<SftpFileCheckService> _logger;

        public SftpFileCheckService(IOptions<SftpConfiguration> sftpConfiguration, IFileRecordRepository fileRecordRepository, MediafonDbContext dbContext, ILogger<SftpFileCheckService> logger)
        {
            _sftpConfiguration = sftpConfiguration.Value;
            _fileRecordRepository = fileRecordRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        // Method with the stoppingToken allows to define the core logic of background service and handle graceful shutdown when necessary
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SftpFileCheckService is starting.");

            // Ensures that the database is created
            await _dbContext.Database.EnsureCreatedAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var sftpClient = new SftpClient(_sftpConfiguration.ServerAddress, _sftpConfiguration.Port, _sftpConfiguration.Username, _sftpConfiguration.Password))
                    {
                        // Connects to the SFTP server
                        sftpClient.Connect();
                        _logger.LogInformation("SFTP connection established.");

                        // Checks for new files
                        await CheckForNewFilesAsync(sftpClient);
                        _logger.LogInformation("Checked for new files.");

                        // Disconnects from the SFTP server
                        sftpClient.Disconnect();
                        _logger.LogInformation("SFTP connection closed.");
                    }
                }
                catch (Exception ex)
                {
                    // Handles and logs any exceptions that occurs during SFTP or database operations
                    _logger.LogError(ex, "An error occurred during file or database operation.");
                }

                // Waits for 1 minute before checking again
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            _logger.LogInformation("SftpFileCheckService is stopped.");
        }

        // Checks for new files in the specified SFTP remote folders
        private async Task CheckForNewFilesAsync(SftpClient sftpClient)
        {
            foreach (var remoteFolderPath in _sftpConfiguration.RemoteFolderPaths)
            {
                var files = sftpClient.ListDirectory(remoteFolderPath);

                foreach (var file in files)
                {
                    if (!file.IsDirectory)
                    {
                        try
                        {
                            var isFileExists = await _fileRecordRepository.DoesFileRecordExistAsync(file.FullName, file.LastWriteTime);

                            if (!isFileExists)
                            {
                                var localFilePath = Path.Combine(_sftpConfiguration.LocalFolderPath, Path.GetFileName(file.FullName));

                                using (var localFile = File.Create(localFilePath))
                                {
                                    sftpClient.DownloadFile(file.FullName, localFile);
                                }

                                var fileModel = new FileRecordModel
                                {
                                    FilePath = file.FullName,
                                    CreationTime = file.LastWriteTime.ToUniversalTime()
                                };

                                await _fileRecordRepository.AddFileRecordAsync(fileModel);
                                _logger.LogInformation("New file downloaded: {FileName}", file.FullName);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handles and logs any exceptions that occur during file operations or database operations
                            _logger.LogError(ex, "An error occurred during file or database operation.");
                        }
                    }
                }
            }
        }
    }
}
