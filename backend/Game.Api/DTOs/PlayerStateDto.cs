namespace Game.Api.DTOs
{
    public class PlayerStateDto
    {
        public string PlayerId { get; set; }
        public string Location { get; set; }
        public int Hp { get; set; }
        public string[] Inventory { get; set; }
    }
}