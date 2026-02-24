namespace GameApi.Models
{
    public class ActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PlayerState NewState { get; set; }
    }
}
