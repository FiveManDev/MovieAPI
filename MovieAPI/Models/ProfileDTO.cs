using MovieAPI.Data;

namespace MovieAPI.Models
{
    public class ProfileDTO
    {
        public Guid ProfileID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Avatar { get; set; }
        public string? EMail { get; set; }
        //Relationship
        public Guid UserID { get; set; }
        public Guid ClassID { get; set; }

    }
}
