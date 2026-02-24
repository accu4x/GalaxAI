namespace GameApi.Models
{
    public class ActionRequest
    {
        public string ActionId { get; set; }
        public Dictionary<string,string> Params { get; set; }
        public string IdempotencyKey { get; set; }
    }
}
