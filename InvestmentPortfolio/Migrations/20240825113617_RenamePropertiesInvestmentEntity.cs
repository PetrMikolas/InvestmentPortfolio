using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class RenamePropertiesInvestmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Investments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Investments",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Investments",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Investments",
                newName: "CreatedDate");
        }
    }
}
