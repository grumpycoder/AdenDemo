namespace Aden.Core.Models
{
    public class WorkItemImage
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public int WorkItemId { get; set; }
    }
}
