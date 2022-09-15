using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

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
                e.Property(user => user.UserID).HasDefaultValueSql("NEWID()");
                e.Property(user => user.UserName);
                e.Property(user => user.Password);
                e.HasOne(user => user.Profile)
                    .WithOne(p => p.User)
                    .HasForeignKey<User>(user => user.UserID)
                    .HasConstraintName("PK_User_One_To_One_Profile");
                e.HasOne(user => user.Authorization)
                    .WithOne(aut => aut.Users)
                    .HasForeignKey<Authorization>(aut => aut.AuthorizationID)
                    .HasConstraintName("PK_User_One_To_One_Authorization");
                e.HasOne(user => user.Token)
                    .WithOne(token => token.User)
                    .HasForeignKey<User>(user => user.UserID)
                    .HasConstraintName("PK_User_One_To_One_Token");
                e.HasMany(user => user.MovieInformations)
                    .WithOne(movie => movie.User)
                    .HasConstraintName("PK_User_One_To_Many_MovieInformation");
                e.HasMany(user => user.Reviews)
                    .WithOne(r => r.User)
                    .HasConstraintName("PK_User_One_To_Many_Review");
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
                e.HasOne(pro => pro.Classification)
                    .WithOne(cl => cl.Profile)
                    .HasForeignKey<Classification>(cl => cl.ClassID)
                    .HasConstraintName("PK_Profile_One_To_One_Classification");
            });
            modelBuilder.Entity<Token>(e =>
            {
                e.ToTable("Token");
                e.HasKey(token => token.TokenID);
                e.Property(token => token.TokenID).HasDefaultValueSql("NEWID()");
                e.Property(token => token.TokenString);
                e.Property(token => token.JwtID);
                e.Property(token => token.IsUsed);
                e.Property(token => token.IsRevoked);
                e.Property(token => token.IssuedAt);
                e.Property(token => token.ExpiredAt);
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
                e.HasOne(mi => mi.Classification)
                    .WithOne(cl => cl.MovieInformation)
                    .HasForeignKey<MovieInformation>(mi => mi.MovieID)
                    .HasConstraintName("PK_MovieInformation_One_To_One_Classification");
                e.HasMany(mi => mi.Reviews)
                    .WithOne(r => r.MovieInformation)
                    .HasConstraintName("PK_MovieInformation_One_To_Many_Review");
                e.HasOne(mi => mi.Genre)
                    .WithOne(g => g.MovieInformation)
                    .HasForeignKey<Genre>(g => g.GenreID)
                    .HasConstraintName("PK_MovieInformation_One_To_One_Genre");
                e.HasOne(mi => mi.MovieType)
                    .WithOne(mt => mt.MovieInformation)
                    .HasForeignKey<MovieType>(mt => mt.MovieTypeID)
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
                e.Property(r => r.Content);
                e.Property(r => r.Rating);
                e.Property(r => r.ReviewTime);
            });
        }
    }
}
