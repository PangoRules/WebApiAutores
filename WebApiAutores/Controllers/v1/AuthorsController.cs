using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entities;
using WebApiAutores.Utilities;

namespace WebApiAutores.Controllers.v1
{
    //TODO: Pending to re-factor the code so unit-tests may apply
    [ApiController]
    [Route("api/authors")]
    [HeaderPresentAttribute("x-version", "1")]
    //[Route("api/v1/authors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public AuthorsController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            _context = context;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet(Name = "getAuthorsv1")] //GET: /api/authors
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<List<AuthorDto>>> GetList([FromQuery] PaginationDto paginationDto)
        {
            var queryable = _context.Authors.AsQueryable();
            await HttpContext.InsertParametersPaginationInHeaders(queryable);
            var authors = await queryable.OrderBy(author => author.Name).Paginate(paginationDto).ToListAsync();
            return Ok(_mapper.Map<List<AuthorDto>>(authors));
        }

        [HttpGet("{id:int}", Name = "getAuthorByIdv1")] //GET: /api/authors/{id}
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<AuthorDtoWithBooks>> GetById(int id)
        {
            var autor = await _context.Authors
                .Include(authorDb => authorDb.AuthorsBooks)
                .ThenInclude(authorBooksDb => authorBooksDb.Book)
                .FirstOrDefaultAsync(a => a.Id == id);

            if(autor == null)
                return NotFound();

            var result = _mapper.Map<AuthorDtoWithBooks>(autor);

            return Ok(result);
        }

        [HttpGet("{name}", Name = "getAuthorsByNamev1")] //GET: /api/authors/{name}
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<List<AuthorDto>>> GetByName(string name)
        {
            var authors = await _context.Authors.Where(a => a.Name.Contains(name)).ToListAsync();

            if(authors == null)
                return NotFound();

            var mappedAuthors = _mapper.Map<List<AuthorDto>>(authors);

            return mappedAuthors;
        }

        [HttpPost(Name = "createAuthorv1")] //POST: /api/authors/
        public async Task<ActionResult> Add(AuthorCreationDto authorCreationDTO)
        {
            var authorSameName = await _context.Authors.AnyAsync(x => x.Name == authorCreationDTO.Name);

            if(authorSameName)
                return BadRequest($"There is already an author with the name: {authorCreationDTO.Name}");

            var author = _mapper.Map<Author>(authorCreationDTO);

            _context.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("getAuthorById", new { id = author.Id }, _mapper.Map<AuthorDto>(author));
        }

        [HttpPut("{id:int}", Name = "updateAuthorv1")] //PUT: /api/authors/
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

        /// <summary>
        /// Deletes an author
        /// </summary>
        /// <param name="id">Id of the author to delete</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "deleteAuthorv1")] //DELETE: /api/authors/
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