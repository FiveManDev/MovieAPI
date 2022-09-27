namespace MovieAPI.Data
{
    public class MovieTypeInformation
    {
        public Guid MovieID { get; set; }
        public Guid MovieTypeID { get; set; }
        //Relationship
        public MovieInformation MovieInformation { get; set; }
        public MovieType MovieType { get; set; }
    }
}
