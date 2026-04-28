using Microsoft.EntityFrameworkCore;
using OfficinaGestionale.Api.Models;

namespace OfficinaGestionale.Api.Context;

public class OfficinaContext : DbContext
{
    public OfficinaContext(DbContextOptions<OfficinaContext> options) : base(options) { }

    public DbSet<Cliente> Clienti { get; set; } = null!;
    public DbSet<Veicolo> Veicoli { get; set; } = null!;
    public DbSet<Intervento> Interventi { get; set; } = null!;
    public DbSet<Utente> Utenti { get; set; } = null!;
    public DbSet<Preventivo> Preventivi { get; set; } = null!;
    public DbSet<VocePreventivo> VociPreventivo { get; set; } = null!;
    public DbSet<Fattura> Fatture { get; set; } = null!;
    public DbSet<RigaFattura> RigheFattura { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Utente>(entity =>
        {
            entity.ToTable("Utente");
            entity.HasKey(x => x.UtenteId);
            entity.HasIndex(x => x.Email).IsUnique();// email unica per login
            entity.Property(x => x.Email).HasMaxLength(150).IsRequired(); 
            entity.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Ruolo).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");
            entity.HasKey(x => x.ClienteId);
            entity.HasIndex(x => x.Codice).IsUnique(); // codice interno tipo CLI2026-001
            entity.Property(x => x.Codice).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Cognome).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Indirizzo).HasMaxLength(150);
            entity.Property(x => x.Telefono).HasMaxLength(30);
            entity.Property(x => x.Email).HasMaxLength(150);
            entity.Property(x => x.CodiceFiscale).HasMaxLength(20);
            entity.Property(x => x.PartitaIva).HasMaxLength(20);
        });

        modelBuilder.Entity<Veicolo>(entity =>
        {
            entity.ToTable("Veicolo");
            entity.HasKey(x => x.VeicoloId);
            entity.HasIndex(x => x.Targa).IsUnique();
            entity.Property(x => x.Targa).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Marca).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Modello).HasMaxLength(100).IsRequired();

            entity.HasOne(x => x.Cliente)
                  .WithMany(x => x.Veicoli)
                  .HasForeignKey(x => x.ClienteRif)
                  .OnDelete(DeleteBehavior.Restrict);// non cancello il cliente se ha veicoli
        });

        modelBuilder.Entity<Intervento>(entity =>
        {
            entity.ToTable("Intervento");
            entity.HasKey(x => x.InterventoId);
            entity.HasIndex(x => x.Codice).IsUnique();
            entity.Property(x => x.Codice).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Descrizione).HasMaxLength(500);
            entity.Property(x => x.Prezzo).HasColumnType("decimal(10,2)");
            entity.Property(x => x.Stato).HasConversion<string>().HasMaxLength(30).IsRequired();// salvo enum come stringa per leggibilità
            entity.Property(x => x.DataIngresso).HasColumnName("data_ingresso");

            entity.HasOne(x => x.Veicolo)
                  .WithMany(x => x.Interventi)
                  .HasForeignKey(x => x.VeicoloRif)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // dati iniziali per test
        modelBuilder.Entity<Cliente>().HasData(
            new Cliente
            {
                ClienteId = 1,
                Codice = "CLI2026-001",
                Nome = "Mario",
                Cognome = "Rossi",
                Indirizzo = "Via Roma 1",
                Telefono = "3331234567",
                Email = "mario.rossi@example.com"
            },
            new Cliente
            {
                ClienteId = 2,
                Codice = "CLI2026-002",
                Nome = "Luca",
                Cognome = "Bianchi",
                Indirizzo = "Via Milano 20",
                Telefono = "3337654321",
                Email = "luca.bianchi@example.com"
            }
        );
        // veicoli collegati ai clienti di esempio
        modelBuilder.Entity<Veicolo>().HasData(
            new Veicolo
            {
                VeicoloId = 1,
                Codice = "VEI2026-001",
                Targa = "DV552ER",
                Marca = "Fiat",
                Modello = "Panda",
                Anno = 2018,
                ClienteRif = 1
            },
            new Veicolo
            {
                VeicoloId = 2,
                Codice = "VEI2026-002",
                Targa = "EL920GY",
                Marca = "Volkswagen",
                Modello = "Golf",
                Anno = 2020,
                ClienteRif = 2
            }
        );

        modelBuilder.Entity<Preventivo>(entity =>
        {
            entity.ToTable("Preventivo");
            entity.HasKey(x => x.PreventivoId);
            entity.HasIndex(x => x.Codice).IsUnique();
            entity.Property(x => x.Codice).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Note).HasMaxLength(500);
            entity.Property(x => x.Stato).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(x => x.AliquotaIva).HasColumnType("decimal(5,2)").HasDefaultValue(22m);// IVA default 22%

            entity.HasOne(x => x.Veicolo)
                  .WithMany()
                  .HasForeignKey(x => x.VeicoloRif)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<VocePreventivo>(entity =>
        {
            entity.ToTable("VocePreventivo");
            entity.HasKey(x => x.VocePreventivoId);
            entity.Property(x => x.Descrizione).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Quantita).HasColumnType("decimal(10,3)");
            entity.Property(x => x.PrezzoUnitario).HasColumnType("decimal(10,2)");

            entity.HasOne(x => x.Preventivo)
                  .WithMany(x => x.Voci)
                  .HasForeignKey(x => x.PreventivoRif)
                  .OnDelete(DeleteBehavior.Cascade);// se elimino il preventivo elimino anche le voci
        });

        modelBuilder.Entity<Fattura>(entity =>
        {
            entity.ToTable("Fattura");
            entity.HasKey(x => x.FatturaId);
            entity.HasIndex(x => x.Codice).IsUnique();
            entity.Property(x => x.Codice).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Note).HasMaxLength(500);
            entity.Property(x => x.Stato).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(x => x.AliquotaIva).HasColumnType("decimal(5,2)").HasDefaultValue(22m);
            entity.Property(x => x.ModalitaPagamento).HasMaxLength(50);

            entity.HasOne(x => x.Veicolo)
                  .WithMany()
                  .HasForeignKey(x => x.VeicoloRif)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Preventivo)
                  .WithMany()
                  .HasForeignKey(x => x.PreventivoRif)
                  .OnDelete(DeleteBehavior.SetNull)// la fattura può restare anche senza preventivo
                  .IsRequired(false);
        });

        modelBuilder.Entity<RigaFattura>(entity =>
        {
            entity.ToTable("RigaFattura");
            entity.HasKey(x => x.RigaFatturaId);
            entity.Property(x => x.Descrizione).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Quantita).HasColumnType("decimal(10,3)");
            entity.Property(x => x.PrezzoUnitario).HasColumnType("decimal(10,2)");

            entity.HasOne(x => x.Fattura)
                  .WithMany(x => x.Righe)
                  .HasForeignKey(x => x.FatturaRif)
                  .OnDelete(DeleteBehavior.Cascade);// elimino le righe insieme alla fattura
        });
        // intervento di esempio
        modelBuilder.Entity<Intervento>().HasData(
            new Intervento
            {
                InterventoId = 1,
                Codice = "INT2026-001",
                Prezzo = 180.00m,
                Stato = StatoIntervento.Aperto,
                DataIngresso = new DateTime(2026, 4, 1),
                VeicoloRif = 1
            }
        );
    }
}
