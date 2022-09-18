namespace MovieAPI.Data
{
    public class Profile
    {
        public Guid ProfileID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Avatar { get; set; }
        public string? EMail { get; set; }
        //Relationship
        public Guid UserID { get; set; }
        public User? User { get; set; }
        public Guid ClassID { get; set; }
        public Classification? Classification { get; set; }
    }
}
