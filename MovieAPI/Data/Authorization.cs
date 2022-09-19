namespace MovieAPI.Data
{
    public class Authorization
    {
        public Guid AuthorizationID { get; set; }
        public string? AuthorizationName { get; set; }
        public int AuthorizationLevel { get; set; }
        //Relationship
        public ICollection<User>? User { get; set; }
    }
}
