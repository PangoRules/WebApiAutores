using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers
{
    //TODO: Pending to re-factor the code so unit-tests may apply
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this._mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "getBookById")] //GET: /api/books{id}
        public async Task<ActionResult<BookDtoWithAuthors>> Get(int id)
        {
            if(id == 0)
                return BadRequest("Wrong bookId");

            var book = await _context.Books
                .Include(bookDb => bookDb.AuthorsBooks)
                .ThenInclude(authorBookDb => authorBookDb.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if(book == null)
                return NotFound();

            book.AuthorsBooks = book.AuthorsBooks.OrderBy(ab => ab.Order).ToList();

            return _mapper.Map<BookDtoWithAuthors>(book);
        }

        [HttpPost] //POST: /api/books
        public async Task<ActionResult> Add(BookCreationDto bookDto)
        {
            if(bookDto.AuthorsIds == null)
                return BadRequest("A book must have at least one author to be created.");

            var authorsIds = await _context.Authors.Where(authorBd => bookDto.AuthorsIds.Contains(authorBd.Id)).Select(x => x.Id).ToListAsync();

            if(authorsIds.Count != bookDto.AuthorsIds.Count)
                return BadRequest("One of the authors is not registered in the database.");

            var book = _mapper.Map<Book>(bookDto);
            AssignOrder(book);

            _context.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("getBookById", new { id = book.Id }, _mapper.Map<BookDto>(book));
        }

        [HttpPut("{id:int}")] //PUT: /api/books{id}
        public async Task<ActionResult> Put(int id, BookCreationDto bookDto)
        {
            if(bookDto.AuthorsIds == null)
                return BadRequest("A book must have at least one author to be updated.");

            var authorsIds = await _context.Authors.Where(authorBd => bookDto.AuthorsIds.Contains(authorBd.Id)).Select(x => x.Id).ToListAsync();

            if(authorsIds.Count != bookDto.AuthorsIds.Count)
                return BadRequest("One of the authors is not registered in the database.");

            var bookDb = await _context.Books.Include(b => b.AuthorsBooks).FirstOrDefaultAsync(b => b.Id == id);

            if(bookDb == null)
                return NotFound();

            bookDb = _mapper.Map(bookDto, bookDb);

            AssignOrder(bookDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")] //PATCH: /api/books{id}
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDto> patchDocument)
        {
            if(patchDocument == null)
                return BadRequest();

            var bookDb = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);

            if(bookDb == null)
                return NotFound();

            var bookPatchDto = _mapper.Map<BookPatchDto>(bookDb);

            patchDocument.ApplyTo(bookPatchDto, ModelState);

            var valid = TryValidateModel(bookPatchDto);
            if(!valid)
                return BadRequest(ModelState);

            _mapper.Map(bookPatchDto, bookDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")] //DELETE: /api/books{id}
        public async Task<ActionResult> Delete(int id)
        {
            var deleteBook = await _context.Books.AnyAsync(a => a.Id == id);
            if(!deleteBook)
                return NotFound();

            _context.Remove(new Book() { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static void AssignOrder(Book book)
        {
            if(book.AuthorsBooks != null)
            {
                for(int i = 0; i < book.AuthorsBooks.Count; i++)
                {
                    book.AuthorsBooks[i].Order = i;
                }
            }
        }
    }
}
