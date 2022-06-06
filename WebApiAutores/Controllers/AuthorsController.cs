using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers
{
    //TODO: Pending to re-factor the code so unit-tests may apply
    [ApiController]
    [Route("api/authors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public AuthorsController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this._context = context;
            this._mapper = mapper;
            this._authorizationService = authorizationService;
        }

        [HttpGet(Name = "getAuthors")] //GET: /api/authors
        [AllowAnonymous]
        public async Task<ActionResult<ResourceCollection<AuthorDto>>> GetList()
        {
            var isAdmin = await _authorizationService.AuthorizeAsync(User, "isAdmin");

            var authors = await _context.Authors.ToListAsync();
            var mappedAuthors = _mapper.Map<List<AuthorDto>>(authors);
            mappedAuthors.ForEach(author => GenerateLinks(author, isAdmin.Succeeded));

            var result = new ResourceCollection<AuthorDto> { Values = mappedAuthors };
            result.Links.Add(new DataHATEOAS(Url.Link("getAuthors", new { }), description: "self", method: "GET"));
            if(isAdmin.Succeeded)
                result.Links.Add(new DataHATEOAS(Url.Link("createAuthor", new { }), description: "create-author", method: "POST"));

            return result;
        }

        [HttpGet("{id:int}", Name = "getAuthorById")] //GET: /api/authors/{id}
        [AllowAnonymous]
        public async Task<ActionResult<AuthorDtoWithBooks>> GetById(int id)
        {
            var isAdmin = await _authorizationService.AuthorizeAsync(User, "isAdmin");

            var autor = await _context.Authors
                .Include(authorDb => authorDb.AuthorsBooks)
                .ThenInclude(authorBooksDb => authorBooksDb.Book)
                .FirstOrDefaultAsync(a => a.Id == id);

            if(autor == null)
                return NotFound();

            var result = _mapper.Map<AuthorDtoWithBooks>(autor);
            GenerateLinks(result, isAdmin.Succeeded);

            return result;
        }

        [HttpGet("{name}", Name = "getAuthorsByName")] //GET: /api/authors/{name}
        [AllowAnonymous]
        public async Task<ActionResult<List<AuthorDto>>> GetByName(string name)
        {
            var isAdmin = await _authorizationService.AuthorizeAsync(User, "isAdmin");

            var authors = await _context.Authors.Where(a => a.Name.Contains(name)).ToListAsync();

            if(authors == null)
                return NotFound();

            var mappedAuthors = _mapper.Map<List<AuthorDto>>(authors);
            mappedAuthors.ForEach(author => GenerateLinks(author, isAdmin.Succeeded));

            return mappedAuthors;
        }

        [HttpPost(Name = "createAuthor")] //POST: /api/authors/
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

        [HttpPut("{id:int}", Name = "updateAuthor")] //PUT: /api/authors/
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

        [HttpDelete("{id:int}", Name = "deleteAuthor")] //DELETE: /api/authors/
        public async Task<ActionResult> Delete(int id)
        {
            var deleteAuthor = await _context.Authors.AnyAsync(a => a.Id == id);
            if(!deleteAuthor)
                return NotFound();

            _context.Remove(new Author() { Id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }

        private void GenerateLinks(AuthorDto authorDto, bool isAdmin)
        {
            authorDto.Links.Add(new DataHATEOAS(Url.Link("getAuthorById", new { id = authorDto.Id }), description: "self", method: "GET"));
            if(isAdmin)
            {
                authorDto.Links.Add(new DataHATEOAS(Url.Link("updateAuthor", new { id = authorDto.Id }), description: "self", method: "PUT"));
                authorDto.Links.Add(new DataHATEOAS(Url.Link("deleteAuthor", new { id = authorDto.Id }), description: "self", method: "DELETE"));
            }
        }
    }
}