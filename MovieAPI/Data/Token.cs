namespace MovieAPI.Data
{
    public class Token
    {
        public Guid TokenID { get; set; }   
        public string? TokenString { get; set; }
        public string? JwtID { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        //Relationship
        public User? User { get; set; }
    }
}
