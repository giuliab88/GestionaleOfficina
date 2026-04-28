using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficinaGestionale.Api.Migrations
{
    public partial class NormalizzaCodici : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // normalizzo i codici al formato nuovo (CLI2026-001, VEI..., INT...)
            // ignoro quelli già corretti
            migrationBuilder.Sql(@"
                UPDATE Cliente
                SET Codice = CONCAT('CLI', YEAR(GETDATE()), '-',
                    RIGHT('000' + CAST(CAST(RIGHT(Codice, LEN(Codice) - 3) AS INT) AS VARCHAR(10)), 3))
                WHERE Codice NOT LIKE 'CLI[0-9][0-9][0-9][0-9]-%';

                UPDATE Veicolo
                SET Codice = CONCAT('VEI', YEAR(GETDATE()), '-',
                    RIGHT('000' + CAST(CAST(RIGHT(Codice, LEN(Codice) - 3) AS INT) AS VARCHAR(10)), 3))
                WHERE Codice NOT LIKE 'VEI[0-9][0-9][0-9][0-9]-%';

                UPDATE Intervento
                SET Codice = CONCAT('INT', YEAR(GETDATE()), '-',
                    RIGHT('000' + CAST(CAST(RIGHT(Codice, LEN(Codice) - 3) AS INT) AS VARCHAR(10)), 3))
                WHERE Codice NOT LIKE 'INT[0-9][0-9][0-9][0-9]-%';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // non gestisco rollback: trasformazione non reversibile
        }
    }
}