using WebApiAutores.Validations;

namespace WebApiAutores.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [FirstLetterCap]
        public string Title { get; set; }
    }
}
