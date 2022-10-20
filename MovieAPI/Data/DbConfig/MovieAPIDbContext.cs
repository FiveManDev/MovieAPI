using Microsoft.EntityFrameworkCore;
using MovieAPI.Services;
using System.Security.Principal;

namespace MovieAPI.Data.DbConfig
{
    public class MovieAPIDbContext : DbContext
    {
        public MovieAPIDbContext() { }
        #region Dbset
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<Classification> Classifications { get; set; }
        public DbSet<MovieInformation> MovieInformations { get; set; }
        public DbSet<MovieGenreInformation> MovieGenreInformations { get; set; }
        public DbSet<MovieType> MovieTypes { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        #endregion

        public MovieAPIDbContext(DbContextOptions<MovieAPIDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region DataCongig
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("User");
                e.HasKey(user => user.UserID);
                e.Property(user => user.UserID).HasDefaultValueSql("NEWID()").IsRequired();
                e.Property(user => user.UserName).IsRequired();
                e.Property(user => user.PasswordHash).IsRequired();
                e.Property(user => user.PasswordSalt).IsRequired();
                e.Property(user => user.Status).IsRequired();
                e.Property(user => user.CreateAt).IsRequired();
                e.HasMany(user => user.Reviews)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserID)
                    .HasConstraintName("PK_User_One_To_Many_Review")
                    .OnDelete(DeleteBehavior.NoAction);
                e.HasMany(user => user.TicketForSenders)
                    .WithOne(ticket => ticket.Sender)
                    .HasForeignKey(ticket => ticket.SenderId)
                    .HasConstraintName("PK_User_One_To_Many_TicketForSender")
                    .OnDelete(DeleteBehavior.NoAction);
                e.HasMany(user => user.TicketForReceivers)
                    .WithOne(ticket => ticket.Receiver)
                    .HasForeignKey(ticket => ticket.ReceiverId)
                    .HasConstraintName("PK_User_One_To_Many_TicketForReceiver")
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Profile>(e =>
            {
                e.ToTable("Profile");
                e.HasKey(pro => pro.ProfileID);
                e.Property(pro => pro.ProfileID).HasDefaultValueSql("NEWID()");
                e.Property(pro => pro.FirstName).HasDefaultValue("User_");
                e.Property(pro => pro.LastName).HasDefaultValue(Guid.NewGuid().ToString());
                e.Property(pro => pro.Avatar);
                e.Property(pro => pro.Email);
                e.HasOne(pro => pro.User)
                    .WithOne(user => user.Profile)
                    .HasForeignKey<Profile>(pro => pro.UserID)
                    .HasConstraintName("PK_Profile_One_To_One_User");
            });
            modelBuilder.Entity<Token>(e =>
            {
                e.ToTable("Token");
                e.HasKey(token => token.TokenID);
                e.Property(token => token.TokenID).HasDefaultValueSql("NEWID()");
                e.Property(token => token.AccessToken);
                e.Property(token => token.RefreshToken);
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
                e.Property(auth => auth.AuthorizationLevel);
                e.HasMany(auth => auth.User)
                   .WithOne(user => user.Authorization)
                   .HasForeignKey(user => user.AuthorizationID)
                   .HasConstraintName("PK_User_Many_To_One_Authorization")
                   .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Classification>(e =>
            {
                e.ToTable("Classification");
                e.HasKey(clf => clf.ClassID);
                e.Property(clf => clf.ClassID).HasDefaultValueSql("NEWID()");
                e.Property(clf => clf.ClassName);
                e.Property(clf => clf.ClassLevel);
                e.HasMany(clf => clf.Profiles)
                    .WithOne(pro => pro.Classification)
                    .HasForeignKey(pro => pro.ClassID)
                    .HasConstraintName("PK_Profile_One_To_One_Classification")
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(clf => clf.MovieInformations)
                    .WithOne(mi => mi.Classification)
                    .HasForeignKey(mi => mi.ClassID)
                    .HasConstraintName("PK_MovieInformation_Many_To_One_Classification")
                    .OnDelete(DeleteBehavior.Cascade);
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
                e.Property(mi => mi.ReleaseTime);
                e.Property(mi => mi.PublicationTime);
                e.Property(mi => mi.CoverImage);
                e.Property(mi => mi.Age);
                e.Property(mi => mi.MovieURL);
                e.Property(mi => mi.RunningTime);
                e.Property(mi => mi.Quality);
                e.Property(mi => mi.IsVisible);
                e.HasOne(mi => mi.User)
                    .WithMany(user => user.MovieInformations)
                    .HasForeignKey(mi => mi.UserID)
                    .HasConstraintName("PK_User_One_To_Many_MovieInformation");
                e.HasMany(mi => mi.Reviews)
                   .WithOne(movie => movie.MovieInformation)
                   .HasForeignKey(r => r.MovieID)
                   .HasConstraintName("PK_MovieInformation_One_To_Many_Review")
                   .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(mi => mi.MovieGenreInformations)
                    .WithOne(mgi => mgi.MovieInformation)
                    .HasForeignKey(mgi => mgi.MovieID)
                    .HasConstraintName("PK_MovieInformation_One_To_Many_MovieGenreInformation")
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<MovieType>(e =>
            {
                e.ToTable("MovieType");
                e.HasKey(mt => mt.MovieTypeID);
                e.Property(mt => mt.MovieTypeID).HasDefaultValueSql("NEWID()");
                e.Property(mt => mt.MovieTypeName);
                e.HasMany(mt => mt.MovieInformations)
                    .WithOne(mi => mi.MovieType)
                    .HasForeignKey(mi => mi.MovieTypeID)
                    .HasConstraintName("PK_MovieInformation_One_To_One_MovieType")
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Genre>(e =>
            {
                e.ToTable("Genre");
                e.HasKey(g => g.GenreID);
                e.Property(g => g.GenreID).HasDefaultValueSql("NEWID()");
                e.Property(g => g.GenreName);
                e.HasMany(g => g.MovieGenreInformations)
                    .WithOne(mgi => mgi.Genre)
                    .HasForeignKey(mgi => mgi.GenreID)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("PK_Genre_One_To_Many_MovieGenreInformation");
            });
            modelBuilder.Entity<MovieGenreInformation>(e =>
            {
                e.ToTable("MovieGenreInformation");
                e.HasKey(mgi => new { mgi.MovieID, mgi.GenreID });
                e.Property(mgi => mgi.MovieID);
                e.Property(mgi => mgi.GenreID);
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
            });
            modelBuilder.Entity<Ticket>(e =>
            {
                e.ToTable("Ticket");
                e.HasKey(ticket => ticket.TicketID);
                e.Property(ticket => ticket.TicketID).HasDefaultValueSql("NEWID()");
                e.Property(ticket => ticket.GroupID).HasDefaultValueSql("NEWID()");
                e.Property(ticket => ticket.IsFromAdmin);
                e.Property(ticket => ticket.MessageContent);
                e.Property(ticket => ticket.MessageTime);
            });
            #endregion
            #region DataInit
            //modelBuilder.Entity<Authorization>().HasData(
            //    new Authorization { AuthorizationID = Guid.NewGuid(), AuthorizationName = "Normal User", AuthorizationLevel = 1 },
            //    new Authorization { AuthorizationID = Guid.NewGuid(), AuthorizationName = "Film Poducer", AuthorizationLevel = 2 },
            //    new Authorization { AuthorizationID = Guid.NewGuid(), AuthorizationName = "Admin", AuthorizationLevel = 3 }
            //    );
            //modelBuilder.Entity<Classification>().HasData(
            //    new Classification { ClassID = Guid.NewGuid(), ClassName = "Basic", ClassLevel = 1, ClassPrice = 0 },
            //    new Classification { ClassID = Guid.NewGuid(), ClassName = "Premium", ClassLevel = 2, ClassPrice = 100 }
            //   );
            //modelBuilder.Entity<Genre>().HasData(
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Action" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Adventure" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Comedy" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Cartoon" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Crime & Gangster" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Drama Films" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Epics / Hisorical" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Horror Films" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Mucicals (Dance)" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Science Fiction" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "War (Anti-war)" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Westerns" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Romance" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Tragedy" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Documentary" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Sitcom" },
            //    new Genre { GenreID = Guid.NewGuid(), GenreName = "Documentary" }
            //  );
            //modelBuilder.Entity<MovieType>().HasData(
            //    new MovieType { MovieTypeID = Guid.NewGuid(), MovieTypeName = "Short Video" },
            //    new MovieType { MovieTypeID = Guid.NewGuid(), MovieTypeName = "Movie Theater" },
            //    new MovieType { MovieTypeID = Guid.NewGuid(), MovieTypeName = "TV Show" },
            //    new MovieType { MovieTypeID = Guid.NewGuid(), MovieTypeName = "Movie Confession" },
            //    new MovieType { MovieTypeID = Guid.NewGuid(), MovieTypeName = "Exclusive movie" }
            //   );
            #endregion

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(AppSettings.ConnectionString);
        }
    }
}
