using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.DTOs
{
    public class BookPatchDto
    {
        [EachWordCap]
        [StringLength(maximumLength: 250)]
        [Required]
        public string Title { get; set; }

        public DateTime DatePublished { get; set; }
    }
}
