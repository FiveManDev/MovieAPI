using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    public partial class AddMovieGenreInformationTable : Migration
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
                    ReleaseYear = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublicationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CoverImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MovieURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RunningTime = table.Column<float>(type: "real", nullable: false),
                    Quality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "e4ae11e0-a9ad-4753-9616-d5f569e2b19e"),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "MovieGenreInformation",
                columns: table => new
                {
                    MovieID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenreInformation", x => new { x.MovieID, x.GenreID });
                    table.ForeignKey(
                        name: "PK_Genre_One_To_Many_MovieGenreInformation",
                        column: x => x.GenreID,
                        principalTable: "Genre",
                        principalColumn: "GenreID");
                    table.ForeignKey(
                        name: "PK_MovieInformation_One_To_Many_MovieGenreInformation",
                        column: x => x.MovieID,
                        principalTable: "MovieInformation",
                        principalColumn: "MovieID");
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
                    { new Guid("147aad7e-eefe-4b49-b637-7aa5dfaa38ab"), 3, "Admin" },
                    { new Guid("3db2b025-a20c-460d-8810-36aa273229be"), 1, "Normal User" },
                    { new Guid("eb09ecca-0779-4a06-80b3-3df64590d773"), 2, "Film Poducer" }
                });

            migrationBuilder.InsertData(
                table: "Classification",
                columns: new[] { "ClassID", "ClassLevel", "ClassName", "ClassPrice" },
                values: new object[,]
                {
                    { new Guid("84251a89-a458-46f8-ba28-83df593ed2a9"), 2, "Premium", 100.0 },
                    { new Guid("e360722f-7405-4278-a4b2-17497036cef0"), 1, "Basic", 0.0 }
                });

            migrationBuilder.InsertData(
                table: "Genre",
                columns: new[] { "GenreID", "GenreName" },
                values: new object[,]
                {
                    { new Guid("003a894b-36a8-4471-b906-dc627a6ce9c2"), "Crime & Gangster" },
                    { new Guid("15f25288-4ff7-4bcf-b9ae-11fc91074863"), "Documentary" },
                    { new Guid("22849f83-a5b4-49eb-93ed-e2d942254521"), "Romance" },
                    { new Guid("295c36f4-54d7-4183-b6b1-450054047200"), "Westerns" },
                    { new Guid("32ab71e0-a75d-4d39-8d5d-e66525477d48"), "Science Fiction" },
                    { new Guid("350af705-bd31-40ab-8b0f-3a064d4e3df9"), "Cartoon" },
                    { new Guid("63da4fe0-de4d-4c8e-b8c8-ec3202c20038"), "Sitcom" },
                    { new Guid("6ac68c37-0ae3-478f-9c22-9cf22fa3db1c"), "Tragedy" },
                    { new Guid("800101db-589a-48b7-9126-24ee55465a9d"), "Horror Films" },
                    { new Guid("8782bbd0-2f56-4fd4-89d6-081396549bfb"), "Drama Films" },
                    { new Guid("9ad897ac-89d2-4ef0-be64-a7d58dfd5f8d"), "Action" },
                    { new Guid("ac9c04ad-885e-4c73-b035-3e53b5e34284"), "Adventure" },
                    { new Guid("ca44f559-803f-4daf-b024-9c870c62318e"), "War (Anti-war)" },
                    { new Guid("cd3e7605-81ac-4a97-9ca3-58ed958fb4b6"), "Comedy" },
                    { new Guid("f2427dc3-481e-4e31-8f81-d06526bdaf58"), "Epics / Hisorical" },
                    { new Guid("f75bf4cb-64a6-4d25-af4d-712da6e3e9bf"), "Documentary" },
                    { new Guid("f8b486a9-6c27-4d34-be35-2f9a0cfa9999"), "Mucicals (Dance)" }
                });

            migrationBuilder.InsertData(
                table: "MovieType",
                columns: new[] { "MovieTypeID", "MovieTypeName" },
                values: new object[,]
                {
                    { new Guid("6a213e3d-6121-490e-87fc-747ba820491e"), "Short Video" },
                    { new Guid("7cb6ef5f-648a-477a-9172-5172ff4f7868"), "Movie Confession" },
                    { new Guid("99b8a1ab-1302-4443-9ef7-95fae52b4938"), "TV Show" },
                    { new Guid("c956e62b-5709-4a4a-85b0-6a3452b48abf"), "Exclusive movie" },
                    { new Guid("ebfd7919-7210-4c54-bb76-fa37dcc191a3"), "Movie Theater" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenreInformation_GenreID",
                table: "MovieGenreInformation",
                column: "GenreID");

            migrationBuilder.CreateIndex(
                name: "IX_MovieInformation_ClassID",
                table: "MovieInformation",
                column: "ClassID");

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
                name: "MovieGenreInformation");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Token");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "MovieInformation");

            migrationBuilder.DropTable(
                name: "Classification");

            migrationBuilder.DropTable(
                name: "MovieType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Authorization");
        }
    }
}
