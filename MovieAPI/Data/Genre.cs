namespace MovieAPI.Data
{
    public class Genre
    {
        public Guid GenreID { get; set; }
        public string GenreName { get; set; }
        //Relationship
        public ICollection<MovieGenreInformation> MovieGenreInformations { get; set; }
    }
}
