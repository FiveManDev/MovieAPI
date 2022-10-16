using MovieAPI.Data;

namespace MovieAPI.Models.DTO
{
    public class UserDTO
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public DateTime CreateAt { get; set; }
        public bool Status { get; set; }
        //Relationship
        public ProfileDTO Profile { get; set; }
        public AuthorizationDTO Authorization { get; set; }
        public int NumberOfReviews { get; set; }

    }
}
