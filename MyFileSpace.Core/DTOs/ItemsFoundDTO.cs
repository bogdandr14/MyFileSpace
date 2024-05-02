namespace MyFileSpace.Core.DTOs
{
    public abstract class ItemsFoundDTO<T> where T : class
    {
        public ItemsFoundDTO()
        {
            Items = new List<T>();
        }
        public List<T> Items { get; set; }
        public int Skipped { get; set; }
        public int Taken { get; set; } = 15;
        public bool AreLast { get; set; }
    }
}
