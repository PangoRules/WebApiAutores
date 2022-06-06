using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1/books/{bookId:int}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentsController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("{id:int}", Name = "getCommentById")]
        public async Task<ActionResult<CommentDto>> GetSingleCommentById(int commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if(comment == null)
                return NotFound();

            return Ok(_mapper.Map<CommentDto>(comment));
        }

        [HttpGet(Name = "getCommentsByBookId")]
        public async Task<ActionResult<List<CommentDto>>> GetCommentListByBookId(int bookId)
        {
            var book = await _context.Books.AnyAsync(bookDb => bookDb.Id == bookId);

            if(!book)
                return NotFound();

            var comments = await _context.Comments.Where(commentDb => commentDb.BookId == bookId).ToListAsync();
            return _mapper.Map<List<CommentDto>>(comments);
        }

        [HttpPost(Name = "createNewComment")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int bookId, CommentAddDto commentDto)
        {
            var email = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(email);

            var book = await _context.Books.AnyAsync(bookDb => bookDb.Id == bookId);

            if(!book)
                return NotFound();

            var comment = _mapper.Map<Comment>(commentDto);

            comment.BookId = bookId;
            comment.UserId = user.Id;
            _context.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("getCommentById", new { bookId, id = comment.Id }, _mapper.Map<CommentDto>(comment));
        }

        [HttpPut(Name = "updateComment")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int bookId, int id, CommentAddDto commentDto)
        {
            var bookExists = await _context.Books.AnyAsync(bookDb => bookDb.Id == bookId);
            var commentExists = await _context.Comments.AnyAsync(commentDb => commentDb.Id == id);

            if(!bookExists || !commentExists)
                return NotFound();

            var comment = _mapper.Map<Comment>(commentDto);
            comment.Id = id;
            comment.BookId = bookId;

            _context.Update(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}