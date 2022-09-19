namespace MovieAPI.Data
{
    public class Classification
    {
        public Guid ClassID { get; set; }
        public string? ClassName { get; set; }
        public int ClassLevel { get; set; }
        public double ClassPrice { get; set; }
        //Relationship
        public ICollection<Profile>? Profiles { get; set; }
        public ICollection<MovieInformation>? MovieInformations { get; set; }
    }
}
