using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    public partial class DbInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authorization",
                columns: table => new
                {
                    AuthorizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AuthorizationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorizationLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorization", x => x.AuthorizationID);
                });

            migrationBuilder.CreateTable(
                name: "Classification",
                columns: table => new
                {
                    ClassID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassLevel = table.Column<int>(type: "int", nullable: false),
                    ClassPrice = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classification", x => x.ClassID);
                });

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    GenreID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    GenreName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.GenreID);
                });

            migrationBuilder.CreateTable(
                name: "MovieType",
                columns: table => new
                {
                    MovieTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    MovieTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieType", x => x.MovieTypeID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                    table.ForeignKey(
                        name: "PK_User_Many_To_One_Authorization",
                        column: x => x.AuthorizationID,
                        principalTable: "Authorization",
                        principalColumn: "AuthorizationID");
                });

            migrationBuilder.CreateTable(
                name: "MovieInformation",
                columns: table => new
                {
                    MovieID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    MovieName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Actor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Director = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subtitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CoverImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MovieURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RunningTime = table.Column<float>(type: "real", nullable: false),
                    Quality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieInformation", x => x.MovieID);
                    table.ForeignKey(
                        name: "PK_MovieInformation_Many_To_One_Classification",
                        column: x => x.ClassID,
                        principalTable: "Classification",
                        principalColumn: "ClassID");
                    table.ForeignKey(
                        name: "PK_MovieInformation_One_To_One_Genre",
                        column: x => x.GenreID,
                        principalTable: "Genre",
                        principalColumn: "GenreID");
                    table.ForeignKey(
                        name: "PK_MovieInformation_One_To_One_MovieType",
                        column: x => x.MovieTypeID,
                        principalTable: "MovieType",
                        principalColumn: "MovieTypeID");
                    table.ForeignKey(
                        name: "PK_User_One_To_Many_MovieInformation",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    ProfileID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "User_"),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "5168b7e1-085f-45bd-be8b-cac77549aea9"),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EMail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.ProfileID);
                    table.ForeignKey(
                        name: "PK_Profile_One_To_One_Classification",
                        column: x => x.ClassID,
                        principalTable: "Classification",
                        principalColumn: "ClassID");
                    table.ForeignKey(
                        name: "PK_Profile_One_To_One_User",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Token",
                columns: table => new
                {
                    TokenID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.TokenID);
                    table.ForeignKey(
                        name: "PK_User_One_To_One_Token",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    ReviewID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ReviewTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.ReviewID);
                    table.ForeignKey(
                        name: "PK_MovieInformation_One_To_Many_Review",
                        column: x => x.MovieID,
                        principalTable: "MovieInformation",
                        principalColumn: "MovieID");
                    table.ForeignKey(
                        name: "PK_User_One_To_Many_Review",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.InsertData(
                table: "Authorization",
                columns: new[] { "AuthorizationID", "AuthorizationLevel", "AuthorizationName" },
                values: new object[,]
                {
                    { new Guid("63e9479f-308e-4b40-809f-4a36197e7194"), 1, "Normal User" },
                    { new Guid("9ab24764-9eca-4a5c-8dd3-6fbbf49c2b80"), 3, "Admin" },
                    { new Guid("f4d7ab68-398d-441d-aec9-ab3f96fa5e40"), 2, "Film Poducer" }
                });

            migrationBuilder.InsertData(
                table: "Classification",
                columns: new[] { "ClassID", "ClassLevel", "ClassName", "ClassPrice" },
                values: new object[,]
                {
                    { new Guid("37edf2c1-3cc3-4ed1-bf93-929f424736e6"), 1, "Basic", 0.0 },
                    { new Guid("f1a39d16-eaa3-4022-8ce3-6ce7835a55b9"), 2, "Premium", 100.0 }
                });

            migrationBuilder.InsertData(
                table: "Genre",
                columns: new[] { "GenreID", "GenreName" },
                values: new object[,]
                {
                    { new Guid("02b3e5a7-c72c-48f8-a9cd-b99df8355bca"), "Documentary" },
                    { new Guid("1286a972-7aae-433e-87d9-14935a8ea7b2"), "Mucicals (Dance)" },
                    { new Guid("3e25a731-d9f0-4ce3-9d42-00ee3b617c9d"), "Science Fiction" },
                    { new Guid("41282142-2f66-43d5-91ec-9e86ad78e4b3"), "Tragedy" },
                    { new Guid("415cc598-f313-4a3a-b0a8-355b69857fa7"), "Action" },
                    { new Guid("4f478506-f01e-49a3-9837-e653bdf72124"), "War (Anti-war)" },
                    { new Guid("4fe2d57d-6569-4acc-a97f-fe4ed7535510"), "Comedy" },
                    { new Guid("62ade6a6-b2ef-4dfb-86df-d86a90ed15bd"), "Cartoon" },
                    { new Guid("79387353-3d4b-4dc0-bc50-9f6161c8f88e"), "Documentary" },
                    { new Guid("b0e32d17-8381-41b3-a978-a2f18a63c6f7"), "Crime & Gangster" },
                    { new Guid("c027ce73-bd7b-4630-b17f-683f6160d69c"), "Romance" },
                    { new Guid("dda59f1d-b580-492b-8547-58213874c323"), "Epics / Hisorical" },
                    { new Guid("e8ff2466-d57f-481b-9419-d0072f1433cc"), "Sitcom" },
                    { new Guid("eb538398-298f-4e3a-bdf1-efb2ac9d8e14"), "Drama Films" },
                    { new Guid("edf567b4-c02e-4d76-861b-577b9bad2e3c"), "Adventure" },
                    { new Guid("f21fce82-cc03-434e-ac26-95e26b7cfa78"), "Horror Films" },
                    { new Guid("fc59c8fc-f627-44b6-b2ff-e40333675d3f"), "Westerns" }
                });

            migrationBuilder.InsertData(
                table: "MovieType",
                columns: new[] { "MovieTypeID", "MovieTypeName" },
                values: new object[,]
                {
                    { new Guid("1e69e16c-31fb-4059-8264-146416c0fc4b"), "TV Show" },
                    { new Guid("4ad82e82-b4f6-4a9d-a94e-df3761c06c72"), "Short Video" },
                    { new Guid("dde622e4-8c18-4215-a476-c7d4e4bf8c28"), "Exclusive movie" },
                    { new Guid("de39405d-4235-4926-840b-760338cdb421"), "Movie Confession" },
                    { new Guid("e4e84026-8a3d-4d8e-a47d-d6ee51060601"), "Movie Theater" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieInformation_ClassID",
                table: "MovieInformation",
                column: "ClassID");

            migrationBuilder.CreateIndex(
                name: "IX_MovieInformation_GenreID",
                table: "MovieInformation",
                column: "GenreID");

            migrationBuilder.CreateIndex(
                name: "IX_MovieInformation_MovieTypeID",
                table: "MovieInformation",
                column: "MovieTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_MovieInformation_UserID",
                table: "MovieInformation",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_ClassID",
                table: "Profile",
                column: "ClassID");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_UserID",
                table: "Profile",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Review_MovieID",
                table: "Review",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserID",
                table: "Review",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Token_UserID",
                table: "Token",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_AuthorizationID",
                table: "User",
                column: "AuthorizationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Token");

            migrationBuilder.DropTable(
                name: "MovieInformation");

            migrationBuilder.DropTable(
                name: "Classification");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "MovieType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Authorization");
        }
    }
}
