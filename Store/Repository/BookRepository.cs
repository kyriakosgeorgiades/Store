using AutoMapper;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Store.Context;
using Store.Entities;
using Store.Interface;

namespace Store.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context)
        { 
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _context.Books.OrderBy(b => b.BookName).ToList();
        } 
    }
}
