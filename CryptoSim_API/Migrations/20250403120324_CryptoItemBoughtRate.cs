using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoSim_API.Migrations
{
    /// <inheritdoc />
    public partial class CryptoItemBoughtRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BoughtAtRate",
                table: "CryptoItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoughtAtRate",
                table: "CryptoItems");
        }
    }
}
