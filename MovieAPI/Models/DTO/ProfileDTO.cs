using MovieAPI.Data;

namespace MovieAPI.Models.DTO
{
    public class ProfileDTO
    {
        public Guid ProfileID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public Guid UserID { get; set; }
        public ClassificationDTO classification { get; set; }


    }
}
