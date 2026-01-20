using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiapCloudGamesUsers.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "DateCreated", "Active", "Email", "HashPassword", "IsAdmin", "Name" },
                values: new object[] { new Guid("bbbbbbbb-2222-3333-4444-555555555555"), new DateTime(2025, 10, 24, 0, 0, 0, DateTimeKind.Utc), true, "admin@local", "YWRtaW4=", true, "Administrator" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
