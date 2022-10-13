namespace MovieAPI.Models
{
    public class ChatModel
    {
        public Guid GroupID { get; set; }
        public Guid AdminID { get; set; }
        public string Message { get; set; }
    }
}
