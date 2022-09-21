namespace MovieAPI.Data
{
    public class Token
    {
        public Guid TokenID { get; set; }   
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        //Relationship
        public Guid UserID { get; set; }
        public User? User { get; set; }
    }
}
