namespace api.Dto
{
    public class MarkDto
    {
        public int id {  get; set; }
        public decimal lng { get; set; }
        public decimal lat { get; set; }

        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string addressName { get; set; }
        public string addressDescription { get; set; }
    }
}
