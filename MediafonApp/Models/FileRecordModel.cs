namespace MediafonApp.Models
{
    public class FileRecordModel
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
