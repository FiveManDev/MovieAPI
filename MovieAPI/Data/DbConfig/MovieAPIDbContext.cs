using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace MovieAPI.Data.DbConfig
{
    public class MovieAPIDbContext : DbContext
    {
        public MovieAPIDbContext(DbContextOptions options) : base(options) { }
        #region Dbset
        public DbSet<User>? Users { get; set; }
        public DbSet<Profile>? Profiles { get; set; }
        public DbSet<Token>? Tokens { get; set; }
        public DbSet<Authorization>? Authorizations { get; set; }
        public DbSet<Classification>? Classifications { get; set; }
        public DbSet<MovieInformation>? MovieInformations { get; set; }
        public DbSet<MovieType>? MovieTypes { get; set; }
        public DbSet<Genre>? Categories { get; set; }
        public DbSet<Review>? Reviews { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("User");
                e.HasKey(user => user.UserID);
                e.Property(user => user.UserID).HasDefaultValueSql("NEWID()").IsRequired();
                e.Property(user => user.UserName).IsRequired();
                e.Property(user => user.Password).IsRequired();
                //e.Property(user => user.AuthorizationID).HasConversion<string>();
                e.HasOne(user => user.Authorization)
                    .WithMany(auth => auth.User)
                    .HasForeignKey(user => user.AuthorizationID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("PK_User_Many_To_One_Authorization");
                
            });
            modelBuilder.Entity<Profile>(e =>
            {
                e.ToTable("Profile");
                e.HasKey(pro => pro.ProfileID);
                e.Property(pro => pro.ProfileID).HasDefaultValueSql("NEWID()");
                e.Property(pro => pro.FirstName);
                e.Property(pro => pro.LastName);
                e.Property(pro => pro.Avatar);
                e.Property(pro => pro.EMail);
                e.HasOne(pro => pro.User)
                    .WithOne(user => user.Profile)
                    .HasForeignKey<Profile>(pro => pro.UserID)
                    .HasConstraintName("PK_Profile_One_To_One_User");
                e.HasOne(pro => pro.Classification)
                    .WithMany(cl => cl.Profiles)
                    .HasForeignKey(pro => pro.ClassID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("PK_Profile_One_To_One_Classification");
            });
            modelBuilder.Entity<Token>(e =>
            {
                e.ToTable("Token");
                e.HasKey(token => token.TokenID);
                e.Property(token => token.TokenID).HasDefaultValueSql("NEWID()");
                e.Property(token => token.AccessToken);
                e.Property(token => token.RefreshToken);
                e.Property(token => token.IsUsed);
                e.Property(token => token.IsRevoked);
                e.Property(token => token.IssuedAt);
                e.Property(token => token.ExpiredAt);
                e.HasOne(token => token.User)
                    .WithOne(user => user.Token)
                    .HasForeignKey<Token>(token => token.UserID)
                    .HasConstraintName("PK_User_One_To_One_Token");
            });
            modelBuilder.Entity<Authorization>(e =>
            {
                e.ToTable("Authorization");
                e.HasKey(auth => auth.AuthorizationID);
                e.Property(auth => auth.AuthorizationID).HasDefaultValueSql("NEWID()");
                e.Property(auth => auth.AuthorizationName);
            });
            modelBuilder.Entity<Classification>(e =>
            {
                e.ToTable("Classification");
                e.HasKey(clf => clf.ClassID);
                e.Property(clf => clf.ClassID).HasDefaultValueSql("NEWID()");
                e.Property(clf => clf.ClassName);
            });
            modelBuilder.Entity<MovieInformation>(e =>
            {
                e.ToTable("MovieInformation");
                e.HasKey(mi => mi.MovieID);
                e.Property(mi => mi.MovieID).HasDefaultValueSql("NEWID()");
                e.Property(mi => mi.MovieName);
                e.Property(mi => mi.Description);
                e.Property(mi => mi.Thumbnail);
                e.Property(mi => mi.Country);
                e.Property(mi => mi.Director);
                e.Property(mi => mi.Actor);
                e.Property(mi => mi.Language);
                e.Property(mi => mi.Subtitle);
                e.Property(mi => mi.PublicationTime);
                e.Property(mi => mi.CoverImage);
                e.Property(mi => mi.Age);
                e.Property(mi => mi.MovieURL);
                e.Property(mi => mi.RunningTime);
                e.Property(mi => mi.Quality);
                e.HasOne(mi => mi.User)
                    .WithMany(user => user.MovieInformations)
                    .HasForeignKey(mi => mi.UserID)
                    .HasConstraintName("PK_User_One_To_Many_MovieInformation");
                e.HasOne(mi => mi.Classification)
                    .WithMany(cl => cl.MovieInformations)
                    .HasForeignKey(mi => mi.ClassID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("PK_MovieInformation_Many_To_One_Classification");
                e.HasOne(mi => mi.Genre)
                    .WithMany(g => g.MovieInformations)
                    .HasForeignKey(mi => mi.GenreID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("PK_MovieInformation_One_To_One_Genre");
                e.HasOne(mi => mi.MovieType)
                    .WithMany(mt => mt.MovieInformations)
                    .HasForeignKey(mi => mi.MovieTypeID)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("PK_MovieInformation_One_To_One_MovieType");
            });
            modelBuilder.Entity<MovieType>(e =>
            {
                e.ToTable("MovieType");
                e.HasKey(mt => mt.MovieTypeID);
                e.Property(mt => mt.MovieTypeID).HasDefaultValueSql("NEWID()");
                e.Property(mt => mt.MovieTypeName);
            });
            modelBuilder.Entity<Genre>(e =>
            {
                e.ToTable("Genre");
                e.HasKey(g => g.GenreID);
                e.Property(g => g.GenreID).HasDefaultValueSql("NEWID()");
                e.Property(g => g.GenreName);
            });
            modelBuilder.Entity<Review>(e =>
            {
                e.ToTable("Review");
                e.HasKey(r => r.ReviewID);
                e.Property(r => r.ReviewID).HasDefaultValueSql("NEWID()");
                e.Property(r => r.Title);
                e.Property(r => r.ReviewContent);
                e.Property(r => r.Rating);
                e.Property(r => r.ReviewTime);
                e.HasOne(r => r.User)
                    .WithMany(user => user.Reviews)
                    .HasForeignKey(r => r.UserID)
                    .HasConstraintName("PK_User_One_To_Many_Review")
                    .OnDelete(DeleteBehavior.NoAction);
                e.HasOne(r => r.MovieInformation)
                    .WithMany(movie => movie.Reviews)
                    .HasForeignKey(r => r.MovieID)
                    .HasConstraintName("PK_MovieInformation_One_To_Many_Review")
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
