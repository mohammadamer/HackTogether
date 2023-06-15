namespace IntranetSearchBot.Models.App
{
    public class AppConfigOptions
    {
        public string BOT_ID { get; set; }
        public string BOT_PASSWORD { get; set; }
        public string TenantId { get; set; }
        public string ConnectionName { get; set; }
        public int SearchSizeThreshold { get; set; }
        public int SearchPageSize { get; set; }
    }
}
