namespace MovieAPI.Helpers
{
    public static partial class Logging
    {
        public static string GetDataSuccess(this string currentMethod,string TableName,int NumberOfRecord)
        {
            return $"[{currentMethod}] Get information of {TableName} table successfully with {NumberOfRecord} result";
        }
        public static string GetDataError(this string currentMethod, string TableName, string Mes)
        {
            return $"[{currentMethod}] Get Data From {TableName} Error: {Mes}";
        }
        public static string PostDataSuccess(this string currentMethod,string TableName)
        {
            return $"[{currentMethod}] Insert Data To {TableName} Success";
        }
        public static string PostDataError(this string currentMethod, string TableName, string Mes)
        {
            return $"[{currentMethod}] Post Data To {TableName} Eror: {Mes}";
        }
        public static string PutDataSuccess(this string currentMethod, string TableName, int NumberOfRecord)
        {
            return $"[{currentMethod}] Update {NumberOfRecord} {TableName} successfully";
        }
        public static string PutDataError(this string currentMethod, string TableName, string Mes)
        {
            return $"[{currentMethod}] Put Data To {TableName} Error: {Mes}";
        }
        public static string DeleteDataSuccess(this string currentMethod,string TableName, int NumberOfRecord)
        {
            return $"[{currentMethod}] Successfully deleted {NumberOfRecord} records of table {TableName}";
        }
        public static string DeleteDataError(this string currentMethod, string TableName, string Mes)
        {
            return $"[{currentMethod}] Delete Data From {TableName} Error: {Mes}";
        }
    }
}

