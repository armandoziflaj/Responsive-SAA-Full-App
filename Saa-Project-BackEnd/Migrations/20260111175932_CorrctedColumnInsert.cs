using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saa_Project_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class CorrctedColumnInsert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedId",
                table: "ChatMessages");

            migrationBuilder.AddColumn<long>(
                name: "RelatedId",
                table: "Notifications",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedId",
                table: "Notifications");

            migrationBuilder.AddColumn<long>(
                name: "RelatedId",
                table: "ChatMessages",
                type: "bigint",
                nullable: true);
        }
    }
}
