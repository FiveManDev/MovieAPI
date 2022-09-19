namespace MovieAPI.Data
{
    public class MovieType
    {
        public Guid MovieTypeID { get; set; }
        public string? MovieTypeName { get; set; }
        //Relationship
        public ICollection<MovieInformation>? MovieInformations { get; set; }
    }
}
