using MovieAPI.Data;

namespace MovieAPI.Models.DTO
{
    public class ReviewDTO
    {
        public Guid ReviewID { get; set; }
        public string Title { get; set; }
        public string ReviewContent { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewTime { get; set; }
        public Guid UserID { get; set; }
        public Guid MovieID { get; set; }

    }
}
