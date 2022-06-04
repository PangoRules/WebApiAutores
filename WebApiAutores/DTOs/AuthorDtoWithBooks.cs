namespace WebApiAutores.DTOs
{
    public class AuthorDtoWithBooks : AuthorDto
    {
        public List<BookDto> Books { get; set; }
    }
}
