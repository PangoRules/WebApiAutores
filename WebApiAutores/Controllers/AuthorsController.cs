using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entities;
using WebApiAutores.Filters;

namespace WebApiAutores.Controllers
{
    //TODO: Pending to re-factor the code so unit-tests may apply
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet] //GET: /api/authors
        public async Task<ActionResult<List<Author>>> Get()
        {
            return await _context.Authors.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> Get(int id)
        {
            var autor = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if(autor == null)
                return NotFound();

            return autor;
        }
        
        [HttpGet("{name}")]
        public async Task<ActionResult<Author>> Get(string name)
        {
            var autor = await _context.Authors.FirstOrDefaultAsync(a => a.Name.Contains(name));

            if(autor == null)
                return NotFound();

            return autor;
        }

        [HttpPost]
        public async Task<ActionResult> Add(Author author)
        {
            var authorSameName = await _context.Authors.AnyAsync(x => x.Name == author.Name);
            if(authorSameName)
                return BadRequest($"There is already an author with the name: {author.Name}");

            _context.Add(author);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Edit(int id, Author author)
        {
            if(author.Id != id || id == 0 )
                return BadRequest("Author not found");

            _context.Update(author);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleteAuthor = await _context.Authors.AnyAsync(a => a.Id == id);
            if(!deleteAuthor)
                return NotFound();

            _context.Remove(new Author() { Id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
