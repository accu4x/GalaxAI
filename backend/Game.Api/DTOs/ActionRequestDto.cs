namespace Game.Api.DTOs
{
    public class ActionRequestDto
    {
        public string PlayerId { get; set; }
        public string ActionId { get; set; }
        public object Params { get; set; }
    }
}