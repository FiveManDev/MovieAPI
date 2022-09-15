namespace MovieAPI.Data
{
    public class Review
    {
        public Guid ReviewID { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewTime { get; set; }
        //Relationship
        public User? User { get; set; }
        public MovieInformation? MovieInformation { get; set; }
    }
}
