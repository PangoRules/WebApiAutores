using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Entities
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field {0} is required")]
        [StringLength(maximumLength: 120, ErrorMessage = "Field {0} shouldn't have more than {1} characters")]
        [FirstLetterCap]
        public string Name { get; set; }
    }
}