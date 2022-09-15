namespace MovieAPI.Helpers
{
    public static partial class Logging
    {
        public static string StartMethod(this string currentMethod)
        {
            return $"[{currentMethod}] Start";
        }
        public static string EndMethod(this string currentMethod)
        {
            return $"[{currentMethod}] End";
        }
        public static string ErrorMethod(this string currentMethod, string Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
        public static string GetData(this string currentMethod, string Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
        public static string PostData(this string currentMethod, string Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
        public static string PutData(this string currentMethod, string Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
        public static string DeleteData(this string currentMethod, string Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
    }
}

