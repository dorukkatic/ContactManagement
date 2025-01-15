using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contacts.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDateTimeToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "contacts",
                table: "People",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "contacts",
                table: "People",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "contacts",
                table: "People");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "contacts",
                table: "People");
        }
    }
}
