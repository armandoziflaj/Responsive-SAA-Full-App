using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Saa_Project_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserContactKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserContacts",
                table: "UserContacts");

            migrationBuilder.DropIndex(
                name: "IX_UserContacts_UserId",
                table: "UserContacts");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "UserContacts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserContacts",
                table: "UserContacts",
                columns: new[] { "UserId", "ContactId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserContacts",
                table: "UserContacts");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "UserContacts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserContacts",
                table: "UserContacts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserContacts_UserId",
                table: "UserContacts",
                column: "UserId");
        }
    }
}
