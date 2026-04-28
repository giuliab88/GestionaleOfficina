using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 

namespace OfficinaGestionale.Api.Migrations
{
    
    public partial class AggiuntaDescrizioneIntervento : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codice = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cognome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Indirizzo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "Veicolo",
                columns: table => new
                {
                    VeicoloId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Targa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Modello = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Anno = table.Column<int>(type: "int", nullable: false),
                    ClienteRif = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veicolo", x => x.VeicoloId);
                    table.ForeignKey(
                        name: "FK_Veicolo_Cliente_ClienteRif",
                        column: x => x.ClienteRif,
                        principalTable: "Cliente",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Intervento",
                columns: table => new
                {
                    InterventoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codice = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Prezzo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Stato = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    data_ingresso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VeicoloRif = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intervento", x => x.InterventoId);
                    table.ForeignKey(
                        name: "FK_Intervento_Veicolo_VeicoloRif",
                        column: x => x.VeicoloRif,
                        principalTable: "Veicolo",
                        principalColumn: "VeicoloId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cliente",
                columns: new[] { "ClienteId", "Codice", "Cognome", "Email", "Indirizzo", "Nome", "Telefono" },
                values: new object[,]
                {
                    { 1, "CLI001", "Rossi", "mario.rossi@example.com", "Via Roma 1", "Mario", "3331234567" },
                    { 2, "CLI002", "Bianchi", "luca.bianchi@example.com", "Via Milano 20", "Luca", "3337654321" }
                });

            migrationBuilder.InsertData(
                table: "Veicolo",
                columns: new[] { "VeicoloId", "Anno", "ClienteRif", "Marca", "Modello", "Targa" },
                values: new object[,]
                {
                    { 1, 2018, 1, "Fiat", "Panda", "AB123CD" },
                    { 2, 2020, 2, "Volkswagen", "Golf", "EF456GH" }
                });

            migrationBuilder.InsertData(
                table: "Intervento",
                columns: new[] { "InterventoId", "Codice", "data_ingresso", "Descrizione", "Prezzo", "Stato", "VeicoloRif" },
                values: new object[] { 1, "INT001", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 180.00m, "Aperto", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Codice",
                table: "Cliente",
                column: "Codice",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Intervento_Codice",
                table: "Intervento",
                column: "Codice",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Intervento_VeicoloRif",
                table: "Intervento",
                column: "VeicoloRif");

            migrationBuilder.CreateIndex(
                name: "IX_Veicolo_ClienteRif",
                table: "Veicolo",
                column: "ClienteRif");

            migrationBuilder.CreateIndex(
                name: "IX_Veicolo_Targa",
                table: "Veicolo",
                column: "Targa",
                unique: true);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Intervento");

            migrationBuilder.DropTable(
                name: "Veicolo");

            migrationBuilder.DropTable(
                name: "Cliente");
        }
    }
}
