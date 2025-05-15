using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoSim_API.Migrations
{
    /// <inheritdoc />
    public partial class TransactionFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Fee",
                table: "Transactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fee",
                table: "Transactions");
        }
    }
}
