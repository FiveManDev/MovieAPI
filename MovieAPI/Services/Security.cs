namespace MovieAPI.Services
{
    public static class Security
    {
        public static string Encode(string? text)
        {
            return BCrypt.Net.BCrypt.HashPassword(text);
        }
        public static Boolean Decode(string text1, String text2)
        {
            return BCrypt.Net.BCrypt.Verify(text1, text2);
        }
    }
}
