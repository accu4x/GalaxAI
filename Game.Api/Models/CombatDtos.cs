namespace Game.Api.Models
{
    public class CombatRequestDto
    {
        public string PlayerId { get; set; }
        public string PlayerShipId { get; set; }
        public string NpcShipId { get; set; }
        public string Action { get; set; }
    }

    public class CombatResponseDto
    {
        public string ResultJson { get; set; }
    }
}
