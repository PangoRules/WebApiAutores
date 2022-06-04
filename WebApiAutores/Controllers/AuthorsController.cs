using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers
{
    //TODO: Pending to re-factor the code so unit-tests may apply
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthorsController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this._context = context;
            this._mapper = mapper;
            this._configuration = configuration;
        }

        [HttpGet("configs")]
        public ActionResult<string> GetConfigs()
        {
            //return _configuration["apellido"];
            return _configuration["connectionStrings:defaultConnection"];
        }


        [HttpGet] //GET: /api/authors
        public async Task<ActionResult<List<AuthorDto>>> GetList()
        {
            var authors = await _context.Authors.ToListAsync();
            return _mapper.Map<List<AuthorDto>>(authors);
        }

        [HttpGet("{id:int}", Name = "getAuthorById")] //GET: /api/authors/{id}
        public async Task<ActionResult<AuthorDtoWithBooks>> GetById(int id)
        {
            var autor = await _context.Authors
                .Include(authorDb => authorDb.AuthorsBooks)
                .ThenInclude(authorBooksDb => authorBooksDb.Book)
                .FirstOrDefaultAsync(a => a.Id == id);

            if(autor == null)
                return NotFound();

            return _mapper.Map<AuthorDtoWithBooks>(autor);
        }

        [HttpGet("{name}")] //GET: /api/authors/{name}
        public async Task<ActionResult<List<AuthorDto>>> GetByName(string name)
        {
            var authors = await _context.Authors.Where(a => a.Name.Contains(name)).ToListAsync();

            if(authors == null)
                return NotFound();

            return _mapper.Map<List<AuthorDto>>(authors);
        }

        [HttpPost] //POST: /api/authors/
        public async Task<ActionResult> Add(AuthorCreationDto authorCreationDTO)
        {
            var authorSameName = await _context.Authors.AnyAsync(x => x.Name == authorCreationDTO.Name);

            if(authorSameName)
                return BadRequest($"There is already an author with the name: {authorCreationDTO.Name}");

            var author = _mapper.Map<Author>(authorCreationDTO);

            _context.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("getAuthorById", new { id = author.Id }, _mapper.Map<AuthorDto>(author) );
        }

        [HttpPut("{id:int}")] //PUT: /api/authors/
        public async Task<ActionResult> Edit(int id, AuthorCreationDto authorDTO)
        {
            var authorExists = await _context.Authors.AnyAsync(x => x.Id == id);
            if(!authorExists)
                return BadRequest("Author not found");

            var author = _mapper.Map<Author>(authorDTO);

            _context.Update(author);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")] //DELETE: /api/authors/
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