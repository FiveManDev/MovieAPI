namespace MovieAPI.Data
{
    public class MovieGenreInformation
    {
        public Guid MovieID { get; set; }
        public Guid GenreID { get; set; }
        //Relationship
        public MovieInformation MovieInformation { get; set; }
        public Genre Genre { get; set; }
    }
}
