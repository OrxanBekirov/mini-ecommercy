namespace WebApi.Settings
{
    public class StripeSetting
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
        public string WebhookSecret { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
        public string FrontendBaseUrl { get; set; }
    }
}
