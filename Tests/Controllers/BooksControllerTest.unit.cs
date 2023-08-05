using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using Store;
using Store.Context;
using Store.Entities;
using Store.Models.Books.Request;
using Store.Models.Books.Response;
using Store.Models.Users.Request;
using Store.Models.Users.Response;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Tests.Controllers;

public class BooksControllerTest : BaseController
{
    public BooksControllerTest() : base(new WebApplicationFactory<Program>().CreateClient()) { }





    [Test]
    public async Task GetAllBooks_Returns_OK_With_Books()
    {
        using var scope = Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Check if the author exists, if not, add the author
        var authorId = new Guid("E0D55165-2040-4BCA-55AA-08DB9203B00C");
        if (!context.Authors.Any(a => a.AuthorId == authorId))
        {
            var authorToAdd = new Author { AuthorId = authorId, AuthorName = "Giannis" };
            context.Authors.Add(authorToAdd);
        }

        // Check if the book exists based on ISBN, if not, add the book
        var bookISBN = "1234567890123";
        if (!context.Books.Any(b => b.ISBN == bookISBN))
        {
            var bookToAdd = new Book
            {
                BookName = "New Book",
                ISBN = bookISBN,
                BookId = Guid.NewGuid(),
                BookUrl = "test.com",
                AuthId = authorId,
                PublicationYear = new DateTime(DateTime.Now.Year, 1, 1)
            };
            context.Books.Add(bookToAdd);
        }

        await context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync("api/Books");

        // Assert
        response.EnsureSuccessStatusCode();
        var wrapperResult = await response.Content.ReadFromJsonAsync<BooksWrapperResponse>();
        Assert.NotNull(wrapperResult);
        Assert.NotNull(wrapperResult.Books);
        Assert.IsNotEmpty(wrapperResult.Books);

        // Clean up after the test (optional, based on your testing strategy)
        var addedBook = context.Books.SingleOrDefault(b => b.ISBN == bookISBN);
        if (addedBook != null)
        {
            context.Books.Remove(addedBook);
        }

        var addedAuthor = context.Authors.SingleOrDefault(a => a.AuthorId == authorId);
        if (addedAuthor != null)
        {
            context.Authors.Remove(addedAuthor);
        }

        await context.SaveChangesAsync();
    }



    [Test]
    public async Task SaveBook_Creates_New_Book_Returns_Created()
    {
        using var scope = Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var authorId = new Guid("E0D55165-2040-4BCA-55AA-08DB9203B00C");

        // Ensure the author exists
        var author = context.Authors.SingleOrDefault(a => a.AuthorId == authorId);
        if (author == null)
        {
            author = new Author
            {
                AuthorId = authorId,
                AuthorName = "Giannis"
            };
            context.Authors.Add(author);
            await context.SaveChangesAsync();
        }

        // Prepare the request data. BookId should be null for creation
        var bookRequest = new SaveBookRequest
        {
            BookName = "New Book",
            ISBN = "1234567890124",
            BookId = null,
            Author = new AuthorDto
            {
                AuthorId = author.AuthorId,
                AuthorName = author.AuthorName,
            },  
            PublicationYear = new DateTime(DateTime.Now.Year, 1, 1)
        };

        // Execute the API call using the SaveBookRequest DTO
        var token = await GetJwtToken();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await Client.PostAsJsonAsync("/api/Books/Book", bookRequest);



        // Check the response
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        StringAssert.Contains("successfully created", result);

        // Clean up after the test
        var addedBook = context.Books.SingleOrDefault(b => b.ISBN == bookRequest.ISBN);
        if (addedBook != null)
        {
            context.Books.Remove(addedBook);
            await context.SaveChangesAsync();
        }

        // Remove the author if it's not referenced by any book
        if (!context.Books.Any(b => b.AuthId == author.AuthorId))
        {
            context.Authors.Remove(author);
            await context.SaveChangesAsync();
        }
    }

    [Test]
    public async Task SaveBook_Updates_Existing_Book_Returns_OK()
    {
        using var scope = Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var authorId = new Guid("E0D55165-2040-4BCA-55AA-08DB9203B00C");
        var author = context.Authors.SingleOrDefault(a => a.AuthorId == authorId);
        if (author == null)
        {
            author = new Author
            {
                AuthorId = authorId,
                AuthorName = "Giannis"
            };
            context.Authors.Add(author);
            await context.SaveChangesAsync();
        }

        // Step 1: Insert a book initially with the name "Initial Book".
        var initialBook = new Book
        {
            BookName = "Initial Book",
            ISBN = "1234567890124",
            AuthId = new Guid("E0D55165-2040-4BCA-55AA-08DB9203B00C"),
            PublicationYear = new DateTime(DateTime.Now.Year, 1, 1),
            BookUrl = "initial-url.com"
        };
        context.Books.Add(initialBook);
        await context.SaveChangesAsync();

        // Step 2: Update the book's name to "Updated Book".
        var updateRequest = new SaveBookRequest
        {
            BookId = initialBook.BookId, // This ensures we're updating and not creating
            BookName = "Updated Book",
            ISBN = initialBook.ISBN,
            Author = new AuthorDto
            {
                AuthorId = initialBook.AuthId,
                AuthorName = author.AuthorName
            },
            PublicationYear = initialBook.PublicationYear
        };

        var token = await GetJwtToken();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await Client.PostAsJsonAsync("/api/Books/Book", updateRequest);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        // Step 3: Fetch the updated book from the database and check if the name has been updated.
        var updatedBook = context.Books.SingleOrDefault(b => b.BookId == initialBook.BookId);

        // Ensure that we fetch the latest data from the database
        context.Entry(updatedBook).Reload();

        Assert.AreEqual("Updated Book", updatedBook.BookName);

        // Cleanup after the test
        context.Books.Remove(updatedBook);
        await context.SaveChangesAsync();
    }




    [Test]
    public async Task GetBookById_Returns_Book()
    {
        using var scope = Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure the author exists
        var authorId = new Guid("E0D55165-2040-4BCA-55AA-08DB9203B00C");
        var author = context.Authors.SingleOrDefault(a => a.AuthorId == authorId);
        if (author == null)
        {
            author = new Author
            {
                AuthorId = authorId,
                AuthorName = "Giannis"
            };
            context.Authors.Add(author);
            await context.SaveChangesAsync();
        }

        // Add a new book
        var bookToAdd = new Book
        {
            BookName = "Another Sample Book",
            ISBN = "1234567890125",
            AuthId = author.AuthorId,
            BookUrl = "test.com"
        };
        context.Books.Add(bookToAdd);
        await context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/Books/Book/{bookToAdd.BookId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Book>();
        Assert.NotNull(result);
        Assert.AreEqual(bookToAdd.BookId, result.BookId);
    }

    [Test]
    public async Task DeleteBook_Returns_OK_When_Deleted()
    {
        using var scope = Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure the author exists
        var authorId = new Guid("E0D55165-2040-4BCA-55AA-08DB9203B00C");
        var author = context.Authors.SingleOrDefault(a => a.AuthorId == authorId);
        if (author == null)
        {
            author = new Author
            {
                AuthorId = authorId,
                AuthorName = "Giannis"
            };
            context.Authors.Add(author);
            await context.SaveChangesAsync();
        }

        // Add a book to delete
        var bookToDelete = new Book
        {
            BookName = "Book to Delete",
            ISBN = GenerateRandomISBN(),
            BookUrl = "test.com",
            AuthId = author.AuthorId
        };
        context.Books.Add(bookToDelete);
        await context.SaveChangesAsync();

        // Act: Delete the book
        var token = await GetJwtToken();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var deleteResponse = await Client.DeleteAsync($"/api/Books/Book/{bookToDelete.BookId}");
        Assert.IsTrue(deleteResponse.IsSuccessStatusCode, "Failed to delete the book.");

        // Get all books
        var getResponse = await Client.GetAsync("/api/Books");
        var wrapperResult = await getResponse.Content.ReadFromJsonAsync<BooksWrapperResponse>();
        var books = wrapperResult?.Books ?? new List<BookSearchResponse>();

        // Assert: Ensure the book is not present in the list
        Assert.IsFalse(books.Any(b => b.BookId == bookToDelete.BookId), "Book was not successfully deleted");
    }

    public string GenerateRandomISBN()
    {
        var rnd = new Random();
        return $"{rnd.Next(100, 999)}-{rnd.Next(1000, 9999)}-{rnd.Next(1000, 9999)}";
    }

    private async Task<string> GetJwtToken()
    {
        var loginRequest = new LoginModelRequest { UserNameOrEmail = "User1", Password = "Password1"}; 
        var response = await Client.PostAsync("/api/Users/Login", new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<LoginModelResponse>(content);
        return tokenResponse.Jwt;
    }


}

