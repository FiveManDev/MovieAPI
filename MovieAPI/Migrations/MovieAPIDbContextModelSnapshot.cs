﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieAPI.Data.DbConfig;

#nullable disable

namespace MovieAPI.Migrations
{
    [DbContext(typeof(MovieAPIDbContext))]
    partial class MovieAPIDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MovieAPI.Data.Authorization", b =>
                {
                    b.Property<Guid>("AuthorizationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("AuthorizationName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuthorizationID");

                    b.ToTable("Authorization", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.Classification", b =>
                {
                    b.Property<Guid>("ClassID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("ClassName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ClassID");

                    b.ToTable("Classification", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.Genre", b =>
                {
                    b.Property<Guid>("GenreID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("GenreName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GenreID");

                    b.ToTable("Genre", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.MovieInformation", b =>
                {
                    b.Property<Guid>("MovieID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Actor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Age")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ClassID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CoverImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Director")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("GenreID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Language")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MovieName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MovieTypeID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MovieURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PublicationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Quality")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("RunningTime")
                        .HasColumnType("real");

                    b.Property<string>("Subtitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Thumbnail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("MovieID");

                    b.HasIndex("ClassID")
                        .IsUnique();

                    b.HasIndex("GenreID")
                        .IsUnique();

                    b.HasIndex("MovieTypeID")
                        .IsUnique();

                    b.HasIndex("UserID");

                    b.ToTable("MovieInformation", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.MovieType", b =>
                {
                    b.Property<Guid>("MovieTypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("MovieTypeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MovieTypeID");

                    b.ToTable("MovieType", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.Profile", b =>
                {
                    b.Property<Guid>("ProfileID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ClassID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EMail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ProfileID");

                    b.HasIndex("ClassID")
                        .IsUnique();

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("Profile", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.Review", b =>
                {
                    b.Property<Guid>("ReviewID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("MovieID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("ReviewContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReviewTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ReviewID");

                    b.HasIndex("MovieID");

                    b.HasIndex("UserID");

                    b.ToTable("Review", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.Token", b =>
                {
                    b.Property<Guid>("TokenID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpiredAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("IssuedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TokenID");

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("Token", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.User", b =>
                {
                    b.Property<Guid>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("AuthorizationID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.HasIndex("AuthorizationID")
                        .IsUnique();

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.MovieInformation", b =>
                {
                    b.HasOne("MovieAPI.Data.Classification", "Classification")
                        .WithOne("MovieInformation")
                        .HasForeignKey("MovieAPI.Data.MovieInformation", "ClassID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_MovieInformation_One_To_One_Classification");

                    b.HasOne("MovieAPI.Data.Genre", "Genre")
                        .WithOne("MovieInformation")
                        .HasForeignKey("MovieAPI.Data.MovieInformation", "GenreID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_MovieInformation_One_To_One_Genre");

                    b.HasOne("MovieAPI.Data.MovieType", "MovieType")
                        .WithOne("MovieInformation")
                        .HasForeignKey("MovieAPI.Data.MovieInformation", "MovieTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_MovieInformation_One_To_One_MovieType");

                    b.HasOne("MovieAPI.Data.User", "User")
                        .WithMany("MovieInformations")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_User_One_To_Many_MovieInformation");

                    b.Navigation("Classification");

                    b.Navigation("Genre");

                    b.Navigation("MovieType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Data.Profile", b =>
                {
                    b.HasOne("MovieAPI.Data.Classification", "Classification")
                        .WithOne("Profile")
                        .HasForeignKey("MovieAPI.Data.Profile", "ClassID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_Profile_One_To_One_Classification");

                    b.HasOne("MovieAPI.Data.User", "User")
                        .WithOne("Profile")
                        .HasForeignKey("MovieAPI.Data.Profile", "UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_Profile_One_To_One_User");

                    b.Navigation("Classification");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Data.Review", b =>
                {
                    b.HasOne("MovieAPI.Data.MovieInformation", "MovieInformation")
                        .WithMany("Reviews")
                        .HasForeignKey("MovieID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("PK_MovieInformation_One_To_Many_Review");

                    b.HasOne("MovieAPI.Data.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("PK_User_One_To_Many_Review");

                    b.Navigation("MovieInformation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Data.Token", b =>
                {
                    b.HasOne("MovieAPI.Data.User", "User")
                        .WithOne("Token")
                        .HasForeignKey("MovieAPI.Data.Token", "UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Data.User", b =>
                {
                    b.HasOne("MovieAPI.Data.Authorization", "Authorization")
                        .WithOne("User")
                        .HasForeignKey("MovieAPI.Data.User", "AuthorizationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_User_One_To_One_Authorization");

                    b.Navigation("Authorization");
                });

            modelBuilder.Entity("MovieAPI.Data.Authorization", b =>
                {
                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Data.Classification", b =>
                {
                    b.Navigation("MovieInformation");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("MovieAPI.Data.Genre", b =>
                {
                    b.Navigation("MovieInformation");
                });

            modelBuilder.Entity("MovieAPI.Data.MovieInformation", b =>
                {
                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("MovieAPI.Data.MovieType", b =>
                {
                    b.Navigation("MovieInformation");
                });

            modelBuilder.Entity("MovieAPI.Data.User", b =>
                {
                    b.Navigation("MovieInformations");

                    b.Navigation("Profile");

                    b.Navigation("Reviews");

                    b.Navigation("Token");
                });
#pragma warning restore 612, 618
        }
    }
}
