using Store.Context;
using Store.Entities;
using System.Security.Cryptography;

namespace Store;

public class Seed
{
    private readonly AppDbContext _context;

    public Seed(AppDbContext context)
    {
        _context = context;
    }

    public void SeedData()
    {
        if (!_context.Books.Any())
        {
            var authors = new Author[]
            {
                new Author { AuthorName = "Author1" },
                new Author { AuthorName = "Author2" }
            };

            foreach (Author a in authors)
            {
                _context.Authors.Add(a);
            }
            _context.SaveChanges();

            var users = new User[]
            {
                new User
                {
                    UserName = "User1",
                    Email = "user1@example.com", // added email
                    Password = HashPassword("Password1")
                },
                new User
                {
                    UserName = "User2",
                    Email = "user2@example.com", // added email
                    Password = HashPassword("Password2")
                }
            };

            foreach (User u in users)
            {
                _context.Users.Add(u);
            }
            _context.SaveChanges();

            var books = new Book[]
            {
                new Book
                {
                    BookName = "Book1",
                    BookUrl = "https://plus.unsplash.com/premium_photo-1669652639337-c513cc42ead6?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=687&q=80",
                    AuthId = authors[0].AuthorId,
                    PublicationYear = DateTime.Now,  // added publication year
                    ISBN = "978-3-16-148410-0" // sample ISBN
                },
                new Book
                {
                    BookName = "Book2",
                    BookUrl = "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1470&q=80",
                    AuthId = authors[1].AuthorId,
                    PublicationYear = DateTime.Now, // added publication year
                    ISBN = "978-1-84-149272-7" // sample ISBN
                },
            };

            foreach (Book b in books)
            {
                _context.Books.Add(b);
            }
            _context.SaveChanges();
        }
    }

    private static string HashPassword(string password)
    {
        // Generate a salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

        // Combine password and salt, then hash
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        // Combine salt and hash
        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        // Convert to string and return
        return Convert.ToBase64String(hashBytes);
    }
}
