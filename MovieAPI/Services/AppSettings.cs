namespace MovieAPI.Services
{
    public static class AppSettings
    {
        public static string SecretKey { get; set; }
        public static string ConnectionString { get; set; }
        public static string AWSS3BucketName { get; set; }
        #region Mail
        public static string Mail { get; set; }
        public static string MailTile { get; set; }
        public static string MailAppPassword { get; set; }
        public static string Host { get; set; }
        public static string Port { get; set; }
        #endregion
        #region Momo
        public static string PartnerCode { get; set; }
        public static string MomoAccessKey { get; set; }
        public static string MomoSerectkey { get; set; }
        public static string Endpoint { get; set; }
        public static string ReturnUrl { get; set; }
        public static string NotifyUrl { get; set; }
        #endregion
    }
}
