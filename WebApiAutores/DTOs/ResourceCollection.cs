namespace WebApiAutores.DTOs
{
    public class ResourceCollection<T> : Resource where T: Resource
    {
        public List<T> Values { get; set; }
    }
}
