namespace MovieAPI.Helpers
{
    public static partial class Logging
    {
        public static string MethodStart(this string currentMethod)
        {
            return $"[{currentMethod}] Start.";
        }
        public static string MethodEnd(this string currentMethod)
        {
            return $"[{currentMethod}] End.";
        }
        public static string GetDataSuccess(this string currentMethod,string TableName,int NumberOfRecord)
        {
            return $"[{currentMethod}] Get information of {TableName} table successfully with {NumberOfRecord} result. [{currentMethod}] End.";
        }
        public static string GetDataError(this string currentMethod, string TableName, string Mes)
        {
            return $"[{currentMethod}] Get Data From {TableName} Error: {Mes}./n[{currentMethod}] End.";
        }
        public static string PostDataSuccess(this string currentMethod,string TableName)
        {
            return $"[{currentMethod}] Insert Data To {TableName} Success./n[{currentMethod}] End.";
        }
        public static string PostDataError(this string currentMethod, string TableName, string Mes)
        {
            return $"[{currentMethod}] Post Data To {TableName} Eror: {Mes}./n[{currentMethod}] End.";
        }
        public static string PutDataSuccess(this string currentMethod, string TableName, int NumberOfRecord)
        {
            return $"[{currentMethod}] Update {NumberOfRecord} {TableName} successfully./n[{currentMethod}] End.";
        }
        public static string PutDataError(this string currentMethod, string TableName, string Mes)
        {
            return $"[{currentMethod}] Put Data To {TableName} Error: {Mes}./n[{currentMethod}] End.";
        }
        public static string DeleteDataSuccess(this string currentMethod,string TableName, int NumberOfRecord)
        {
            return $"[{currentMethod}] Successfully deleted {NumberOfRecord} records of table {TableName}./n[{currentMethod}] End.";
        }
        public static string DeleteDataError(this string currentMethod, string TableName, string Mes)
        {
            return $"[{currentMethod}] Delete Data From {TableName} Error: {Mes}./n[{currentMethod}] End.";
        }
    }
}

