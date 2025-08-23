using Microsoft.EntityFrameworkCore;
using BizOpsAPI.Models;

namespace BizOpsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Revenue> Revenues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== Core precision (money totals) =====
            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.LineTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Expense>()
                .Property(e => e.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Revenue>()
                .Property(r => r.Amount)
                .HasPrecision(18, 2);

            // ===== Users =====
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ===== Relationships =====
            modelBuilder.Entity<Client>()
                .HasOne(c => c.User)
                .WithMany(u => u.Clients)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Client)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.ClientId);

            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(ii => ii.InvoiceId);

            modelBuilder.Entity<Receipt>()
                .HasOne(r => r.Invoice)
                .WithMany(i => i.Receipts)
                .HasForeignKey(r => r.InvoiceId);

            // If you truly want SET NULL on delete for Revenue → ensure Revenue.InvoiceId is nullable (int?)
            modelBuilder.Entity<Revenue>()
                .HasOne(r => r.Invoice)
                .WithMany()
                .HasForeignKey(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);

            // ===== Receipt: indexes & constraints =====
            modelBuilder.Entity<Receipt>(e =>
            {
                // Indexes for frequent filters / ordering
                e.HasIndex(r => r.InvoiceId);
                e.HasIndex(r => r.UploadedAt);
                e.HasIndex(r => r.Sha256Hex);

                // Composite index to speed up de-dupe checks per invoice
                e.HasIndex(r => new { r.InvoiceId, r.Sha256Hex });

                // Column sizes (avoid unlimited string columns)
                e.Property(r => r.ReceiptUrl)
                    .IsRequired()
                    .HasMaxLength(2048);
                e.Property(r => r.OriginalFileName)
                    .HasMaxLength(512);
                e.Property(r => r.ContentType)
                    .HasMaxLength(255);
                e.Property(r => r.FromAddress)
                    .HasMaxLength(320); // RFC-compliant max for email length

                // ✅ EF Core 8+ way to add a check constraint (Postgres-safe quoting)
                e.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Receipt_SizeBytes_NonNegative",
                        "\"SizeBytes\" IS NULL OR \"SizeBytes\" >= 0"
                    );
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
