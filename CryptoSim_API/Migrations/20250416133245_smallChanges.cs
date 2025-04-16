using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoSim_API.Migrations
{
    /// <inheritdoc />
    public partial class smallChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CryptoItems_Cryptos_CryptoId",
                table: "CryptoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CryptoItems_Wallets_WalletId",
                table: "CryptoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserWallets_Users_UserId",
                table: "UserWallets");

            migrationBuilder.DropForeignKey(
                name: "FK_UserWallets_Wallets_WalletId",
                table: "UserWallets");

            migrationBuilder.DropIndex(
                name: "IX_UserWallets_WalletId",
                table: "UserWallets");

            migrationBuilder.DropIndex(
                name: "IX_CryptoItems_CryptoId",
                table: "CryptoItems");

            migrationBuilder.DropIndex(
                name: "IX_CryptoItems_WalletId",
                table: "CryptoItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserWallets_WalletId",
                table: "UserWallets",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoItems_CryptoId",
                table: "CryptoItems",
                column: "CryptoId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoItems_WalletId",
                table: "CryptoItems",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_CryptoItems_Cryptos_CryptoId",
                table: "CryptoItems",
                column: "CryptoId",
                principalTable: "Cryptos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CryptoItems_Wallets_WalletId",
                table: "CryptoItems",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserWallets_Users_UserId",
                table: "UserWallets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserWallets_Wallets_WalletId",
                table: "UserWallets",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
