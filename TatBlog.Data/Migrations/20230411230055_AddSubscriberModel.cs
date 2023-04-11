using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TatBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriberModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscribeEmail = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SubDated = table.Column<DateTime>(type: "datetime", nullable: false),
                    UnSubDated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CancelReason = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    ForceLock = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscribers");
        }
    }
}
