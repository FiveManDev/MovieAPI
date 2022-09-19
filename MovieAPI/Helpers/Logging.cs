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
        public static string GetDataSuccess(this string currentMethod)
        {
            return $"[{currentMethod}]  Message: Get Information Success";
        }
        public static string GetDataError(this string currentMethod, string Mes)
        {
            return $"[{currentMethod}]  Message: {Mes}";
        }
        public static string PostDataSuccess(this string currentMethod)
        {
            return $"[{currentMethod}]  Message: Insert Success";
        }
        public static string PostDataError(this string currentMethod, string Mes)
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

