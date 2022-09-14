namespace MovieAPI.Services
{
    public static partial class Logging
    {
        public static string StartMethod(this String currentMethod)
        {
            return $"[{currentMethod}] Start";
        }
        public static string EndMethod(this String currentMethod)
        {
            return $"[{currentMethod}] End";
        }
        public static string ErrorMethod(this String currentMethod, String Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
        public static string GetData(this String currentMethod, String Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
        public static string PostData(this String currentMethod, String Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
        public static string PutData(this String currentMethod, String Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
        public static string DeleteData(this String currentMethod, String Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
    }
}

