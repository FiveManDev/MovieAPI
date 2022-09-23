namespace MovieAPI.Services
{
    public static class AppSettings
    {
        public static string? SecretKey { get; set; }
        public static string? ConnectionString { get; set; }
        public static string? AWSS3BucketName { get; set; }
    }
}
