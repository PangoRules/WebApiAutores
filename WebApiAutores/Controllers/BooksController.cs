using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers
{
    //TODO: Pending to re-factor the code so unit-tests may apply
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            return await _context.Books.Include(x => x.Author).FirstOrDefaultAsync(b => b.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult> Add(Book book)
        {
            var existsAuthor = await _context.Authors.AnyAsync(a => a.Id == book.AuthorId);
            if(!existsAuthor)
                return BadRequest($"Author with id: {book.AuthorId} inexistent");

            _context.Add(book);
            await _context.SaveChangesAsync();
            return Ok(book);
        }
    }
}
