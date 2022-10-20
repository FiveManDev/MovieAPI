namespace MovieAPI.Models
{
    public class PostReviewModel
    {
        public string Title { get; set; }
        public string ReviewContent { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewTime { get; set; }
        public Guid UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid MovieID { get; set; }
        public string MovieName { get; set; }
    }
}
