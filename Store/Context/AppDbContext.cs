using Microsoft.EntityFrameworkCore;
using Store.Entities;

namespace Store.Context;

/// <summary>
/// Represents the application's database context which is used for database operations.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">Configuration options for the database context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    /// <summary>
    /// Gets or sets the database set representing the books in the database.
    /// </summary>
    public DbSet<Book> Books { get; set; }

    /// <summary>
    /// Gets or sets the database set representing the users in the database.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the database set representing the authors in the database.
    /// </summary>
    public DbSet<Author> Authors { get; set; }

    /// <summary>
    /// Configures entities, relationships, and additional settings when the model is being created.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define table schemas
        modelBuilder.Entity<Book>().ToTable("Books", "Library");
        modelBuilder.Entity<Author>().ToTable("Authors", "Library");
        modelBuilder.Entity<User>().ToTable("Users", "Security");

        // Define relationship between Book and Author
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthId);

        // Define indices for optimization and unique constraints
        modelBuilder.Entity<Book>().HasIndex(b => b.BookName);
        modelBuilder.Entity<Author>().HasIndex(a => a.AuthorName);
        modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Book>().HasIndex(b => b.ISBN).IsUnique().HasDatabaseName("IX_Unique_ISBN");

        // Define property configurations such as required fields and length constraints
        modelBuilder.Entity<User>().Property(u => u.Password).IsRequired().HasMaxLength(600);
        modelBuilder.Entity<User>().Property(u => u.UserName).IsRequired().HasMaxLength(250);
        modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(250);
        modelBuilder.Entity<Author>().Property(a => a.AuthorName).IsRequired().HasMaxLength(250);
        modelBuilder.Entity<Book>().Property(b => b.BookName).IsRequired().HasMaxLength(250);
        modelBuilder.Entity<Book>().Property(b => b.BookUrl).IsRequired();
        modelBuilder.Entity<Book>().Property(b => b.ISBN).IsRequired();
        modelBuilder.Entity<Book>().Property(b => b.PublicationYear).IsRequired();
    }
}
