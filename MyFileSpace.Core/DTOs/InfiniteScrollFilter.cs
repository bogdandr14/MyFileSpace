namespace MyFileSpace.Core.DTOs
{
    public class InfiniteScrollFilter
    {
        public int Skip { get; set; }
        public int Take { get; set; } = 20;
        public string Name { get; set; }
        public bool IncludeOwn { get; set; } = false;
    }
}
