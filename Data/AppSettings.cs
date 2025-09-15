namespace be_devextreme_starter.Data
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ConnString { get; set; }

        public string RecaptchaSecret { get; set; }
        public string RecaptchaSiteKey { get; set; }

        public string ExtraSalt { get; set; }

        public string SuperAdminRolesCSV { get; set; }

        public string LoginTimeOut { get; set; }

        public string GoogleAPIKey { get; set; }
    }
}
