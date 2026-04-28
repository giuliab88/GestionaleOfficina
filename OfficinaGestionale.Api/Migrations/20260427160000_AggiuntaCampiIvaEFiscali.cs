using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficinaGestionale.Api.Migrations
{
    public partial class AggiuntaCampiIvaEFiscali : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // cliente: codice fiscale e partita IVA
            migrationBuilder.AddColumn<string>(
                name: "CodiceFiscale",
                table: "Cliente",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartitaIva",
                table: "Cliente",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            // fattura: aliquota IVA e modalità di pagamento
            migrationBuilder.AddColumn<decimal>(
                name: "AliquotaIva",
                table: "Fattura",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 22m);

            migrationBuilder.AddColumn<string>(
                name: "ModalitaPagamento",
                table: "Fattura",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // preventivo: aliquota IVA e data scadenza validità
            migrationBuilder.AddColumn<decimal>(
                name: "AliquotaIva",
                table: "Preventivo",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 22m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidoFino",
                table: "Preventivo",
                type: "datetime2",
                nullable: true);

            // CF di esempio per i clienti seed
            migrationBuilder.Sql(@"
                UPDATE [Cliente] SET [CodiceFiscale] = 'RSSMRA80A01H501Z' WHERE [ClienteId] = 1;
                UPDATE [Cliente] SET [CodiceFiscale] = 'BNCMLC75M20F205Z' WHERE [ClienteId] = 2;
            ");

            // fattura di esempio con IDENTITY_INSERT (richiesto da SQL Server per ID espliciti)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [Fattura] WHERE [FatturaId] = 1)
                BEGIN
                    SET IDENTITY_INSERT [Fattura] ON;
                    INSERT INTO [Fattura]
                        ([FatturaId],[Codice],[DataEmissione],[DataScadenza],[Note],[Stato],[AliquotaIva],[ModalitaPagamento],[VeicoloRif],[PreventivoRif])
                    VALUES
                        (1,'FAT2026-001','2026-04-01','2026-05-01','Tagliando completo — Fiat Panda 2018','Pagata',22.0,'Bonifico bancario',1,NULL);
                    SET IDENTITY_INSERT [Fattura] OFF;

                    SET IDENTITY_INSERT [RigaFattura] ON;
                    INSERT INTO [RigaFattura] ([RigaFatturaId],[Descrizione],[Quantita],[PrezzoUnitario],[FatturaRif]) VALUES
                        (1,'Manodopera tagliando completo',1.000,80.00,1),
                        (2,'Olio motore 5W-30 (1 litro)',  4.000, 9.50,1),
                        (3,'Filtro olio',                  1.000,12.00,1),
                        (4,'Filtro aria',                  1.000,18.00,1),
                        (5,'Filtro abitacolo',             1.000,15.00,1);
                    SET IDENTITY_INSERT [RigaFattura] OFF;
                END
            ");
            // imponibile 163,00 — IVA 22% = 35,86 — totale 198,86
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [RigaFattura] WHERE [FatturaRif] = 1;
                DELETE FROM [Fattura]    WHERE [FatturaId]  = 1;
            ");

            migrationBuilder.DropColumn(name: "CodiceFiscale",     table: "Cliente");
            migrationBuilder.DropColumn(name: "PartitaIva",        table: "Cliente");
            migrationBuilder.DropColumn(name: "AliquotaIva",       table: "Fattura");
            migrationBuilder.DropColumn(name: "ModalitaPagamento", table: "Fattura");
            migrationBuilder.DropColumn(name: "AliquotaIva",       table: "Preventivo");
            migrationBuilder.DropColumn(name: "ValidoFino",        table: "Preventivo");
        }
    }
}
