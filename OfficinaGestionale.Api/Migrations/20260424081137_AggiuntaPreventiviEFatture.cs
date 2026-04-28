using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficinaGestionale.Api.Migrations
{
    
    public partial class AggiuntaPreventiviEFatture : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Preventivo",
                columns: table => new
                {
                    PreventivoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codice = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Stato = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VeicoloRif = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preventivo", x => x.PreventivoId);
                    table.ForeignKey(
                        name: "FK_Preventivo_Veicolo_VeicoloRif",
                        column: x => x.VeicoloRif,
                        principalTable: "Veicolo",
                        principalColumn: "VeicoloId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fattura",
                columns: table => new
                {
                    FatturaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codice = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DataEmissione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataScadenza = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Stato = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VeicoloRif = table.Column<int>(type: "int", nullable: false),
                    PreventivoRif = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fattura", x => x.FatturaId);
                    table.ForeignKey(
                        name: "FK_Fattura_Preventivo_PreventivoRif",
                        column: x => x.PreventivoRif,
                        principalTable: "Preventivo",
                        principalColumn: "PreventivoId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Fattura_Veicolo_VeicoloRif",
                        column: x => x.VeicoloRif,
                        principalTable: "Veicolo",
                        principalColumn: "VeicoloId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VocePreventivo",
                columns: table => new
                {
                    VocePreventivoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descrizione = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Quantita = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    PrezzoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PreventivoRif = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocePreventivo", x => x.VocePreventivoId);
                    table.ForeignKey(
                        name: "FK_VocePreventivo_Preventivo_PreventivoRif",
                        column: x => x.PreventivoRif,
                        principalTable: "Preventivo",
                        principalColumn: "PreventivoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RigaFattura",
                columns: table => new
                {
                    RigaFatturaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descrizione = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Quantita = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    PrezzoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FatturaRif = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RigaFattura", x => x.RigaFatturaId);
                    table.ForeignKey(
                        name: "FK_RigaFattura_Fattura_FatturaRif",
                        column: x => x.FatturaRif,
                        principalTable: "Fattura",
                        principalColumn: "FatturaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fattura_Codice",
                table: "Fattura",
                column: "Codice",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fattura_PreventivoRif",
                table: "Fattura",
                column: "PreventivoRif");

            migrationBuilder.CreateIndex(
                name: "IX_Fattura_VeicoloRif",
                table: "Fattura",
                column: "VeicoloRif");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivo_Codice",
                table: "Preventivo",
                column: "Codice",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Preventivo_VeicoloRif",
                table: "Preventivo",
                column: "VeicoloRif");

            migrationBuilder.CreateIndex(
                name: "IX_RigaFattura_FatturaRif",
                table: "RigaFattura",
                column: "FatturaRif");

            migrationBuilder.CreateIndex(
                name: "IX_VocePreventivo_PreventivoRif",
                table: "VocePreventivo",
                column: "PreventivoRif");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RigaFattura");

            migrationBuilder.DropTable(
                name: "VocePreventivo");

            migrationBuilder.DropTable(
                name: "Fattura");

            migrationBuilder.DropTable(
                name: "Preventivo");
        }
    }
}
