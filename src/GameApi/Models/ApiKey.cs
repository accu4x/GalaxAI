namespace GameApi.Models
{
    public class ApiKey
    {
        public string Id { get; set; }
        public string PlaintextKey { get; set; }
        public string CreatedAt { get; set; }
        public bool Revoked { get; set; }
    }
}
