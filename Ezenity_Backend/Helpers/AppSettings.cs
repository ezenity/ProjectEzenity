namespace Ezenity_Backend.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }

        // Refresh token time to live (in days_, inactive tokens are
        // automatically deletedfro mthe database after this time
        public int RefreshTokenTTL { get; set; }

        public string EmailFrom { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public bool SmtpEnableSsl { get; set; }
        public string AccessToken { get; set; }
    }
}
