namespace WebApiAutores.DTOs
{
    public class BookDtoWithAuthors : BookDto
    {
        public List<AuthorDto> Authors { get; set; }
    }
}
