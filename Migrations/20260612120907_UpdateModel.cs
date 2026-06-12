using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiloTakipWebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TelNumarasi",
                table: "Soforler",
                newName: "TelefonNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TelefonNo",
                table: "Soforler",
                newName: "TelNumarasi");
        }
    }
}
