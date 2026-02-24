namespace Game.Api.DTOs
{
    public class ActionResultDto
    {
        public bool Success { get; set; }
        public string ResultText { get; set; }
        public object StateDelta { get; set; }
    }
}