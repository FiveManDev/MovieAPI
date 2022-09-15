namespace MovieAPI.Data
{
    public class Authorization
    {
        public Guid AuthorizationID { get; set; }
        public string? AuthorizationName { get; set; }
        //Relationship
        public User? Users { get; set; }
    }
}
