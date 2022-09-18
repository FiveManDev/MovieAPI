namespace MovieAPI.Data
{
    public class User
    {
        public Guid UserID { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        //Relationship
        public Profile? Profile { get; set; }
        public Guid AuthorizationID { get; set; }
        public Authorization? Authorization { get; set; }
        public Token? Token { get; set; }
        public ICollection<MovieInformation>? MovieInformations { get; set; }
        public ICollection<Review>? Reviews { get; set; }

    }
}
