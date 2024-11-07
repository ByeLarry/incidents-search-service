namespace api.Dto
{
    public class CategoryDto
    {
        public int id { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string? color { get; set; }
    }
}
