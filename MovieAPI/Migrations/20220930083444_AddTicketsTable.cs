using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    public partial class AddTicketsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Authorization",
                keyColumn: "AuthorizationID",
                keyValue: new Guid("147aad7e-eefe-4b49-b637-7aa5dfaa38ab"));

            migrationBuilder.DeleteData(
                table: "Authorization",
                keyColumn: "AuthorizationID",
                keyValue: new Guid("3db2b025-a20c-460d-8810-36aa273229be"));

            migrationBuilder.DeleteData(
                table: "Authorization",
                keyColumn: "AuthorizationID",
                keyValue: new Guid("eb09ecca-0779-4a06-80b3-3df64590d773"));

            migrationBuilder.DeleteData(
                table: "Classification",
                keyColumn: "ClassID",
                keyValue: new Guid("84251a89-a458-46f8-ba28-83df593ed2a9"));

            migrationBuilder.DeleteData(
                table: "Classification",
                keyColumn: "ClassID",
                keyValue: new Guid("e360722f-7405-4278-a4b2-17497036cef0"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("003a894b-36a8-4471-b906-dc627a6ce9c2"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("15f25288-4ff7-4bcf-b9ae-11fc91074863"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("22849f83-a5b4-49eb-93ed-e2d942254521"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("295c36f4-54d7-4183-b6b1-450054047200"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("32ab71e0-a75d-4d39-8d5d-e66525477d48"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("350af705-bd31-40ab-8b0f-3a064d4e3df9"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("63da4fe0-de4d-4c8e-b8c8-ec3202c20038"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("6ac68c37-0ae3-478f-9c22-9cf22fa3db1c"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("800101db-589a-48b7-9126-24ee55465a9d"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("8782bbd0-2f56-4fd4-89d6-081396549bfb"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("9ad897ac-89d2-4ef0-be64-a7d58dfd5f8d"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("ac9c04ad-885e-4c73-b035-3e53b5e34284"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("ca44f559-803f-4daf-b024-9c870c62318e"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("cd3e7605-81ac-4a97-9ca3-58ed958fb4b6"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("f2427dc3-481e-4e31-8f81-d06526bdaf58"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("f75bf4cb-64a6-4d25-af4d-712da6e3e9bf"));

            migrationBuilder.DeleteData(
                table: "Genre",
                keyColumn: "GenreID",
                keyValue: new Guid("f8b486a9-6c27-4d34-be35-2f9a0cfa9999"));

            migrationBuilder.DeleteData(
                table: "MovieType",
                keyColumn: "MovieTypeID",
                keyValue: new Guid("6a213e3d-6121-490e-87fc-747ba820491e"));

            migrationBuilder.DeleteData(
                table: "MovieType",
                keyColumn: "MovieTypeID",
                keyValue: new Guid("7cb6ef5f-648a-477a-9172-5172ff4f7868"));

            migrationBuilder.DeleteData(
                table: "MovieType",
                keyColumn: "MovieTypeID",
                keyValue: new Guid("99b8a1ab-1302-4443-9ef7-95fae52b4938"));

            migrationBuilder.DeleteData(
                table: "MovieType",
                keyColumn: "MovieTypeID",
                keyValue: new Guid("c956e62b-5709-4a4a-85b0-6a3452b48abf"));

            migrationBuilder.DeleteData(
                table: "MovieType",
                keyColumn: "MovieTypeID",
                keyValue: new Guid("ebfd7919-7210-4c54-bb76-fa37dcc191a3"));

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "4be997ee-0208-4c02-a55a-47d1fdb4d27b",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "e4ae11e0-a9ad-4753-9616-d5f569e2b19e");

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    TicketID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    GroupID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IsFromAdmin = table.Column<bool>(type: "bit", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.TicketID);
                    table.ForeignKey(
                        name: "PK_User_One_To_Many_TicketForReceiver",
                        column: x => x.ReceiverId,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "PK_User_One_To_Many_TicketForSender",
                        column: x => x.SenderId,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ReceiverId",
                table: "Ticket",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_SenderId",
                table: "Ticket",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "e4ae11e0-a9ad-4753-9616-d5f569e2b19e",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "4be997ee-0208-4c02-a55a-47d1fdb4d27b");

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
        }
    }
}
