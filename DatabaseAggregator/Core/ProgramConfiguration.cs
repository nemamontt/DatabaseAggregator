namespace DatabaseAggregator.Core
{
    public class ProgramConfiguration
    {
        private string UrlFsteс {  get; set; }
        private string UrlNvd { get; set; }
        private string ApiKeyNvd { get; set; }
        private string UrlJvn { get; set; }
        private string SettingText { get; set; }

        public ProgramConfiguration(string urlFsteс, string urlNvd, string apiKeyNvd, string urlJvn)
        {
            UrlFsteс = urlFsteс;
            UrlNvd = urlNvd;
            ApiKeyNvd = apiKeyNvd;
            UrlJvn = urlJvn;
        }
    }
}
