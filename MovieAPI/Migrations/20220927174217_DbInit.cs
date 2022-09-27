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
                    ReleaseYear = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublicationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CoverImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MovieURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RunningTime = table.Column<float>(type: "real", nullable: false),
                    Quality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "1b1d12ce-7ba5-40a8-9ffb-8249e21812d6"),
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
                name: "MovieTypeInformation",
                columns: table => new
                {
                    MovieID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieTypeInformation", x => new { x.MovieID, x.MovieTypeID });
                    table.ForeignKey(
                        name: "PK_MovieInformation_One_To_Many_MovieTypeInformation",
                        column: x => x.MovieID,
                        principalTable: "MovieInformation",
                        principalColumn: "MovieID");
                    table.ForeignKey(
                        name: "PK_MovieType_One_To_Many_MovieTypeInformation",
                        column: x => x.MovieTypeID,
                        principalTable: "MovieType",
                        principalColumn: "MovieTypeID");
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
                    { new Guid("2932094d-2529-4913-8ca9-319be843e172"), 2, "Film Poducer" },
                    { new Guid("6558f909-ca15-49dd-91bf-d73b6e4565d1"), 1, "Normal User" },
                    { new Guid("a025a153-97ea-46d1-ba30-f468cb44f4f0"), 3, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Classification",
                columns: new[] { "ClassID", "ClassLevel", "ClassName", "ClassPrice" },
                values: new object[,]
                {
                    { new Guid("51d085df-c6c5-47c5-bf94-3ee7ac5f0a75"), 2, "Premium", 100.0 },
                    { new Guid("8a77b1fd-dee9-41e9-a3ee-0302b14fa77b"), 1, "Basic", 0.0 }
                });

            migrationBuilder.InsertData(
                table: "Genre",
                columns: new[] { "GenreID", "GenreName" },
                values: new object[,]
                {
                    { new Guid("197594fa-a4f9-4f2a-84fa-7028d52e9200"), "Comedy" },
                    { new Guid("2356ff0f-d132-4b35-a959-67ecfcfb32ce"), "Action" },
                    { new Guid("23a5c7fd-7e2c-440f-bc3d-f7900676c0fd"), "Sitcom" },
                    { new Guid("25f3db4c-dd66-4e95-a2b9-c9d4b85808c0"), "Romance" },
                    { new Guid("417b95a0-ff35-4799-ae9d-a3d131aaeddc"), "Cartoon" },
                    { new Guid("47256245-5f4e-419f-9495-b1960d1c8b56"), "Mucicals (Dance)" },
                    { new Guid("476c9346-c09c-455e-93a4-b0874a36e8eb"), "Crime & Gangster" },
                    { new Guid("5f2f1b35-311e-4bd9-b0b5-5e3452f50add"), "Westerns" },
                    { new Guid("63da580f-031e-45bb-bf7b-cf5416ee0105"), "Adventure" },
                    { new Guid("63f63d7c-d5b9-46ae-96b7-b6c2a5d00c39"), "War (Anti-war)" },
                    { new Guid("7561b2ef-c258-4dca-b0b1-99cf461bba80"), "Drama Films" },
                    { new Guid("98fd3476-9fbb-4c43-b43f-e674a9766093"), "Documentary" },
                    { new Guid("bd7a8bcc-fcdf-433b-a756-d24ee794dd79"), "Horror Films" },
                    { new Guid("cbc5754b-bac4-4679-9909-bfedc4d26dcd"), "Documentary" },
                    { new Guid("cd8a26c7-61eb-4a84-ac7c-9f342ad8f735"), "Science Fiction" },
                    { new Guid("db471aff-ea54-496a-836d-b95bb8099c1f"), "Epics / Hisorical" },
                    { new Guid("fa7ad1c6-c4fb-4cf3-9466-a794d23efcac"), "Tragedy" }
                });

            migrationBuilder.InsertData(
                table: "MovieType",
                columns: new[] { "MovieTypeID", "MovieTypeName" },
                values: new object[,]
                {
                    { new Guid("3f503564-20b1-4300-b19b-14c3ed1f529f"), "Short Video" },
                    { new Guid("68a8910d-a771-492f-a952-954a49e4fd84"), "Movie Confession" },
                    { new Guid("6b6d3e0b-189b-4fc4-bed7-1cbf3eaf5e24"), "TV Show" },
                    { new Guid("b81c4808-b104-4c55-aae0-43bc563c77bf"), "Movie Theater" },
                    { new Guid("d2982a45-4dbe-48da-b9e0-aa5cbe6e0687"), "Exclusive movie" }
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
                name: "IX_MovieInformation_UserID",
                table: "MovieInformation",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MovieTypeInformation_MovieTypeID",
                table: "MovieTypeInformation",
                column: "MovieTypeID");

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
                name: "MovieTypeInformation");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Token");

            migrationBuilder.DropTable(
                name: "MovieType");

            migrationBuilder.DropTable(
                name: "MovieInformation");

            migrationBuilder.DropTable(
                name: "Classification");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Authorization");
        }
    }
}
