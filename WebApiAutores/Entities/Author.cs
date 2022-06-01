using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Entities
{
    public class Author //: IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field {0} is required")]
        [StringLength(maximumLength: 120, ErrorMessage = "Field {0} shouldn't have more than {1} characters")]
        [FirstLetterCap]
        public string Name { get; set; }

        public List<Book> Books { get; set; }

        #region[Examples]
        //[Range(18, 120)]
        //[NotMapped]
        //public string Age { get; set; }

        //[CreditCard]
        //[NotMapped]
        //public string CreditCard { get; set; }

        //[Url]
        //[NotMapped]
        //public string URL { get; set; }

        //public int Minor { get; set; }

        //public int Mayor { get; set; }
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(!string.IsNullOrEmpty(Name))
        //    {
        //        var firstLetter = Name[0].ToString();
        //        if(firstLetter != firstLetter.ToUpper())
        //            yield return new ValidationResult("First letter must be capitalized",
        //                new string[] { nameof(Name) });
        //    }

        //    if(Minor > Mayor)
        //        yield return new ValidationResult("This value cannot be greater than Minor field",
        //            new string[] { nameof(Minor) });
        //}
        #endregion
    }
}