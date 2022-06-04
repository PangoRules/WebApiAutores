using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [EachWordCap]
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }

        public DateTime? DatePublished { get; set; }

        public List<Comment> Comments { get; set; }

        public List<AuthorBook> AuthorsBooks { get; set; }
    }
}
