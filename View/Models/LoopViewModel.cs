namespace View.Models
{
    public class LoopViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> RouteStops { get; set; } = new List<string>();
    }
}
