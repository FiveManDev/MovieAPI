namespace MovieAPI.Models.DTO
{
    public class ClassificationDTO
    {
        public Guid ClassID { get; set; }
        public string ClassName { get; set; }
        public int ClassLevel { get; set; }
        public double ClassPrice { get; set; }
    }
}
