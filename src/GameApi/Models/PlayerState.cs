namespace GameApi.Models
{
    public class PlayerState
    {
        public string PlayerId { get; set; }
        public string Location { get; set; }
        public int HP { get; set; }
        public List<string> Inventory { get; set; } = new List<string>();
        public List<string> AvailableActions { get; set; } = new List<string>();
        public string ETag { get; set; }
    }
}
