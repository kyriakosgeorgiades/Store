using Microsoft.EntityFrameworkCore;
using Store.Entities;

namespace Store.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Book> Books { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Schemas
        modelBuilder.Entity<Book>().ToTable("Books", "Library");
        modelBuilder.Entity<Author>().ToTable("Authors", "Library");
        modelBuilder.Entity<User>().ToTable("Users", "Security");

        // Relationship between Book and Author
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthId);

        // Indices
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.BookName);

        modelBuilder.Entity<Author>()
            .HasIndex(a => a.AuthorName);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique()
            .HasDatabaseName("IX_Unique_ISBN");


        // Property Configurations
        modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(600);

        modelBuilder.Entity<User>()
            .Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(250);

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(250);

        modelBuilder.Entity<Author>()
            .Property(a => a.AuthorName)
            .IsRequired()
            .HasMaxLength(250);

        modelBuilder.Entity<Book>()
            .Property(b => b.BookName)
            .IsRequired()
            .HasMaxLength(250);

        modelBuilder.Entity<Book>()
            .Property(b => b.BookUrl)
            .IsRequired();

        modelBuilder.Entity<Book>()
            .Property(b => b.ISBN)
            .IsRequired();

        modelBuilder.Entity<Book>()
            .Property(b => b.PublicationYear)
            .IsRequired();
    }
}
