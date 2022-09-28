namespace MovieAPI.Data
{
    public class MovieInformation
    {
        public Guid MovieID { get; set; }
        public string MovieName { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string Country { get; set; }
        public string Actor { get; set; }
        public string Director { get; set; }
        public string Language { get; set; }
        public string Subtitle { get; set; }
        public DateTime ReleaseYear { get; set; }
        public DateTime PublicationTime { get; set; }
        public string CoverImage { get; set; }
        public string Age { get; set; }
        public string MovieURL { get; set; }
        public float RunningTime { get; set; }
        public string Quality { get; set; }
        //Relationship
        public Guid UserID { get; set; }
        public User User { get; set; }
        public Guid ClassID { get; set; }
        public Classification Classification { get; set; }
        public Guid MovieTypeID { get; set; }
        public MovieType MovieType { get; set; }
        public ICollection<MovieGenreInformation> MovieGenreInformations { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
