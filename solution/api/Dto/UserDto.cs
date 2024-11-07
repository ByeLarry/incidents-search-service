namespace api.Dto
{
    public class UserDto
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public bool activated { get; set; }
        public bool isBlocked { get; set; }
        public string[] roles { get; set; }
        public string provider { get; set; }
        public string password { get; set; }
        public string id { get; set; }

        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }

        public string? phone_number { get; set; }
    }
}
