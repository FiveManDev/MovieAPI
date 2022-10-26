﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieAPI.Data.DbConfig;

#nullable disable

namespace MovieAPI.Migrations
{
    [DbContext(typeof(MovieAPIDbContext))]
    [Migration("20221026135209_addCollumIsRead")]
    partial class addCollumIsRead
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<int>("AuthorizationLevel")
                        .HasColumnType("int");

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

                    b.Property<int>("ClassLevel")
                        .HasColumnType("int");

                    b.Property<string>("ClassName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ClassPrice")
                        .HasColumnType("float");

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

            modelBuilder.Entity("MovieAPI.Data.MovieGenreInformation", b =>
                {
                    b.Property<Guid>("MovieID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GenreID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("MovieID", "GenreID");

                    b.HasIndex("GenreID");

                    b.ToTable("MovieGenreInformation", (string)null);
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

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

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

                    b.Property<DateTime>("ReleaseTime")
                        .HasColumnType("datetime2");

                    b.Property<float>("RunningTime")
                        .HasColumnType("real");

                    b.Property<string>("Subtitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Thumbnail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("MovieID");

                    b.HasIndex("ClassID");

                    b.HasIndex("MovieTypeID");

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

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("User_");

                    b.Property<string>("LastName")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("644405e3-412f-4ccd-8de9-cf2912000006");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ProfileID");

                    b.HasIndex("ClassID");

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

            modelBuilder.Entity("MovieAPI.Data.Ticket", b =>
                {
                    b.Property<Guid>("TicketID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("GroupID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<bool>("IsFromAdmin")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("MessageContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("MessageTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ReceiverId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TicketID");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("Ticket", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.Token", b =>
                {
                    b.Property<Guid>("TokenID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

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

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.HasIndex("AuthorizationID");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("MovieAPI.Data.MovieGenreInformation", b =>
                {
                    b.HasOne("MovieAPI.Data.Genre", "Genre")
                        .WithMany("MovieGenreInformations")
                        .HasForeignKey("GenreID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_Genre_One_To_Many_MovieGenreInformation");

                    b.HasOne("MovieAPI.Data.MovieInformation", "MovieInformation")
                        .WithMany("MovieGenreInformations")
                        .HasForeignKey("MovieID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_MovieInformation_One_To_Many_MovieGenreInformation");

                    b.Navigation("Genre");

                    b.Navigation("MovieInformation");
                });

            modelBuilder.Entity("MovieAPI.Data.MovieInformation", b =>
                {
                    b.HasOne("MovieAPI.Data.Classification", "Classification")
                        .WithMany("MovieInformations")
                        .HasForeignKey("ClassID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_MovieInformation_Many_To_One_Classification");

                    b.HasOne("MovieAPI.Data.MovieType", "MovieType")
                        .WithMany("MovieInformations")
                        .HasForeignKey("MovieTypeID")
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

                    b.Navigation("MovieType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Data.Profile", b =>
                {
                    b.HasOne("MovieAPI.Data.Classification", "Classification")
                        .WithMany("Profiles")
                        .HasForeignKey("ClassID")
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
                        .OnDelete(DeleteBehavior.Cascade)
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

            modelBuilder.Entity("MovieAPI.Data.Ticket", b =>
                {
                    b.HasOne("MovieAPI.Data.User", "Receiver")
                        .WithMany("TicketForReceivers")
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_User_One_To_Many_TicketForReceiver");

                    b.HasOne("MovieAPI.Data.User", "Sender")
                        .WithMany("TicketForSenders")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("PK_User_One_To_Many_TicketForSender");

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("MovieAPI.Data.Token", b =>
                {
                    b.HasOne("MovieAPI.Data.User", "User")
                        .WithOne("Token")
                        .HasForeignKey("MovieAPI.Data.Token", "UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_User_One_To_One_Token");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Data.User", b =>
                {
                    b.HasOne("MovieAPI.Data.Authorization", "Authorization")
                        .WithMany("User")
                        .HasForeignKey("AuthorizationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("PK_User_Many_To_One_Authorization");

                    b.Navigation("Authorization");
                });

            modelBuilder.Entity("MovieAPI.Data.Authorization", b =>
                {
                    b.Navigation("User");
                });

            modelBuilder.Entity("MovieAPI.Data.Classification", b =>
                {
                    b.Navigation("MovieInformations");

                    b.Navigation("Profiles");
                });

            modelBuilder.Entity("MovieAPI.Data.Genre", b =>
                {
                    b.Navigation("MovieGenreInformations");
                });

            modelBuilder.Entity("MovieAPI.Data.MovieInformation", b =>
                {
                    b.Navigation("MovieGenreInformations");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("MovieAPI.Data.MovieType", b =>
                {
                    b.Navigation("MovieInformations");
                });

            modelBuilder.Entity("MovieAPI.Data.User", b =>
                {
                    b.Navigation("MovieInformations");

                    b.Navigation("Profile");

                    b.Navigation("Reviews");

                    b.Navigation("TicketForReceivers");

                    b.Navigation("TicketForSenders");

                    b.Navigation("Token");
                });
#pragma warning restore 612, 618
        }
    }
}
