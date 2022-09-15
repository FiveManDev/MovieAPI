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
        public User? User { get; set; }
        public Classification? Classification { get; set; }
    }
}
