using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficinaGestionale.Api.Migrations
{
   
    public partial class AggiuntaCodiceVeicolo : Migration
    {
      
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Codice",
                table: "Veicolo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Veicolo",
                keyColumn: "VeicoloId",
                keyValue: 1,
                column: "Codice",
                value: "VE001");

            migrationBuilder.UpdateData(
                table: "Veicolo",
                keyColumn: "VeicoloId",
                keyValue: 2,
                column: "Codice",
                value: "VE002");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codice",
                table: "Veicolo");
        }
    }
}
