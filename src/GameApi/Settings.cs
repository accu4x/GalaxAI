namespace GameApi
{
    public class Settings
    {
        public string AzureTablesConnectionString { get; set; }
        public string TableNames_Players { get; set; } = "players";
        public string TableNames_Keys { get; set; } = "apikeys";
    }
}
