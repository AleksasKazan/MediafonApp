namespace MediafonApp.Models
{
    public class SftpConfiguration
    {
        public string ServerAddress { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string[] RemoteFolderPaths { get; set; }
        public string LocalFolderPath { get; set; }
    }
}
