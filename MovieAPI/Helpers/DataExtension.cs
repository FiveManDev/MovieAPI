using MovieAPI.Data.DbConfig;

namespace MovieAPI.Helpers
{
    public static class DataExtension
    {
        public static bool isBanned(Guid userID)
        {
            using MovieAPIDbContext context = new MovieAPIDbContext() ;
            return true ;
        }
    }
}
