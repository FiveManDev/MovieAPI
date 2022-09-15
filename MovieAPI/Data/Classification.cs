namespace MovieAPI.Data
{
    public class Classification
    {
        public Guid ClassID { get; set; }
        public string? ClassName { get; set; }
        //Relationship
        public Profile? Profile { get; set; }
        public MovieInformation? MovieInformation { get; set; }
    }
}
