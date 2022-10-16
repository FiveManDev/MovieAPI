namespace MovieAPI.Data
{
    public class User
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime CreateAt { get; set; }
        public bool Status { get; set; }
        //Relationship
        public Profile Profile { get; set; }
        public Guid AuthorizationID { get; set; }
        public Authorization Authorization { get; set; }
        public Token Token { get; set; }
        public ICollection<MovieInformation> MovieInformations { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Ticket> TicketForSenders { get; set; }
        public ICollection<Ticket> TicketForReceivers { get; set; }

    }
}
