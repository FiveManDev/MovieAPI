namespace MovieAPI.Data
{
    public class Token
    {
        public Guid TokenID { get; set; }   
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        //Relationship
        public Guid UserID { get; set; }
        public User? User { get; set; }
    }
}
