using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entities;

namespace WebApiAutores.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AuthorCreationDto, Author>();
            CreateMap<Author, AuthorDto>();
            CreateMap<Author, AuthorDtoWithBooks>().ForMember(authorDto => authorDto.Books, options => options.MapFrom(MapAuthorDtoBooks));


            CreateMap<BookCreationDto, Book>().ForMember(book => book.AuthorsBooks, options => options.MapFrom(MapAuthorsBooks));
            CreateMap<Book, BookPatchDto>().ReverseMap();
            CreateMap<Book, BookDto>();
            CreateMap<Book, BookDtoWithAuthors>().ForMember(bookDto => bookDto.Authors, options => options.MapFrom(MapBookDtoAuthors));


            CreateMap<CommentAddDto, Comment>();
            CreateMap<Comment, CommentDto>();


        }

        private List<BookDto> MapAuthorDtoBooks(Author author, AuthorDto authorDto)
        {
            var result = new List<BookDto>();

            if(author.AuthorsBooks == null) { return result;  }

            foreach(var book in author.AuthorsBooks)
            {
                result.Add(new BookDto() {  Id = book.Book.Id, Title = book.Book.Title });
            }

            return result;
        }

        private List<AuthorDto> MapBookDtoAuthors(Book book, BookDto bookDto)
        {
            var result = new List <AuthorDto>();

            if(book.AuthorsBooks == null) { return result; }

            foreach(var authorBook in book.AuthorsBooks)
            {
                result.Add(new AuthorDto() { Id = authorBook.AuthorId, Name = authorBook.Author.Name });
            }

            return result;
        }

        private List<AuthorBook> MapAuthorsBooks(BookCreationDto bookCreateDto, Book book)
        {
            var result = new List<AuthorBook>();

            if(bookCreateDto == null) { return result; }    

            foreach(var authorId in bookCreateDto.AuthorsIds)
            {
                result.Add(new AuthorBook() { AuthorId = authorId });
            }

            return result;
        }
    }
}
