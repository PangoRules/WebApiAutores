using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class EditAdminDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
