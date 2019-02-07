namespace Aden.Web.ViewModels
{
    public class DocumentViewDto
    {
        public int Id { get; set; }

        public string Filename { get; set; }

        public int Version { get; set; }

        public long FileSize { get; set; }
        public long FileSizeMb { get; set; }

        public string FileSizeInMb { get; set; }  //=> ByteSize.FromBytes(FileSize);
    }
}
