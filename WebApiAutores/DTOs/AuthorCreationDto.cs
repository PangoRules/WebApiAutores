using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.DTOs
{
    public class AuthorCreationDto
    {
        [Required(ErrorMessage = "Field {0} is required")]
        [StringLength(maximumLength: 120, ErrorMessage = "Field {0} shouldn't have more than {1} characters")]
        [EachWordCap]
        public string Name { get; set; }
    }
}
