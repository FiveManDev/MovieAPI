using MovieAPI.Data;

namespace MovieAPI.Models.DTO
{
    public class AuthorizationDTO
    {
        public Guid AuthorizationID { get; set; }
        public string AuthorizationName { get; set; }
        public int AuthorizationLevel { get; set; }

    }
}
