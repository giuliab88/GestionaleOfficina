using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficinaGestionale.Api.Migrations
{
    
    public partial class AddUtente : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Utente",
                columns: table => new
                {
                    UtenteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ruolo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utente", x => x.UtenteId);
                });

            migrationBuilder.UpdateData(
                table: "Veicolo",
                keyColumn: "VeicoloId",
                keyValue: 1,
                columns: new[] { "Codice", "Targa" },
                values: new object[] { "VEI001", "DV552ER" });

            migrationBuilder.UpdateData(
                table: "Veicolo",
                keyColumn: "VeicoloId",
                keyValue: 2,
                columns: new[] { "Codice", "Targa" },
                values: new object[] { "VEI002", "EL920GY" });

            migrationBuilder.CreateIndex(
                name: "IX_Utente_Email",
                table: "Utente",
                column: "Email",
                unique: true);
        }

       
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Utente");

            migrationBuilder.UpdateData(
                table: "Veicolo",
                keyColumn: "VeicoloId",
                keyValue: 1,
                columns: new[] { "Codice", "Targa" },
                values: new object[] { "", "AB123CD" });

            migrationBuilder.UpdateData(
                table: "Veicolo",
                keyColumn: "VeicoloId",
                keyValue: 2,
                columns: new[] { "Codice", "Targa" },
                values: new object[] { "", "EF456GH" });
        }
    }
}
