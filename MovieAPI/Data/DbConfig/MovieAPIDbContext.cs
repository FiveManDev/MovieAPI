using Microsoft.EntityFrameworkCore;

namespace MovieAPI.Data.DbConfig
{
    public class MovieAPIDbContext:DbContext
    {
        public MovieAPIDbContext(DbContextOptions options) : base(options) { }
        #region Dbset
        
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
