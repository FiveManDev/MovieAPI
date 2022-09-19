using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    public partial class DBInit : Migration
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
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "7956afd6-3597-4c58-9a73-a1fda0533446"),
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
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    { new Guid("26a16182-8f45-4911-b143-a4f5d12abcba"), 2, "Film Poducer" },
                    { new Guid("df57c066-37c8-4ea3-a5f0-842e90da4bc5"), 3, "Admin" },
                    { new Guid("f72b7ba0-0357-4728-9e05-6ff9f523aba1"), 1, "Normal User" }
                });

            migrationBuilder.InsertData(
                table: "Classification",
                columns: new[] { "ClassID", "ClassLevel", "ClassName", "ClassPrice" },
                values: new object[,]
                {
                    { new Guid("5e9fa6f3-fbd7-46ce-9194-0d4342d26a47"), 2, "Premium", 100.0 },
                    { new Guid("fcc5e61d-d2c5-4688-b6ca-d53b9937252f"), 1, "Basic", 0.0 }
                });

            migrationBuilder.InsertData(
                table: "Genre",
                columns: new[] { "GenreID", "GenreName" },
                values: new object[,]
                {
                    { new Guid("0cf840ee-b757-4491-ad56-af8ddc9e60fa"), "Crime & Gangster" },
                    { new Guid("0f6c4b1d-f667-4cb0-be31-698b2261c182"), "Epics / Hisorical" },
                    { new Guid("4106ce85-99c1-4838-b88a-59a03aeafaf0"), "Cartoon" },
                    { new Guid("474c3cad-933d-457b-b03f-d4329176ecba"), "Horror Films" },
                    { new Guid("49d5a98a-7a97-4ac6-b4dc-2aa852867612"), "War (Anti-war)" },
                    { new Guid("4cc76a6e-b15d-4d02-b9e7-25dab9d2a684"), "Romance" },
                    { new Guid("6579a651-c8cd-4a70-930a-270971e65169"), "Adventure" },
                    { new Guid("68467152-d46c-4b91-851c-9d8ec1c094a4"), "Drama Films" },
                    { new Guid("68fffc55-c6d4-4d60-9198-01daa7246d3b"), "Science Fiction" },
                    { new Guid("6cafee67-7357-44a8-97cd-5526765819d1"), "Comedy" },
                    { new Guid("9801a120-0ab3-460f-81e9-1f8d05ee356d"), "Sitcom" },
                    { new Guid("a1027cb7-d4f4-4c0c-9092-5df7935becb5"), "Documentary" },
                    { new Guid("a4df0fc5-8c37-499d-8b32-3f7657b0f890"), "Westerns" },
                    { new Guid("ba11ea3b-017e-419d-9bdd-2bcbf138be20"), "Action" },
                    { new Guid("be76f7eb-b98c-4081-9e7c-a7ff51e14ab2"), "Mucicals (Dance)" },
                    { new Guid("eb537f9e-ee73-4eb0-9d00-23a1964bae39"), "Documentary" },
                    { new Guid("ed31a14d-c0be-4e33-ae97-91dd1f903b9d"), "Tragedy" }
                });

            migrationBuilder.InsertData(
                table: "MovieType",
                columns: new[] { "MovieTypeID", "MovieTypeName" },
                values: new object[,]
                {
                    { new Guid("25af33ba-965b-41cc-b961-4d4afdc659b0"), "Movie Confession" },
                    { new Guid("26c28429-a9ad-4f92-bbd3-12261f9bb9b2"), "TV Show" },
                    { new Guid("ccd9a085-321e-4d64-9298-71c0c6544035"), "Exclusive movie" },
                    { new Guid("e0100654-cf05-48df-bcd3-bd6196141d32"), "Movie Theater" },
                    { new Guid("e667cee1-ff61-4dbc-bf57-ed4f4d2878c6"), "Short Video" }
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
