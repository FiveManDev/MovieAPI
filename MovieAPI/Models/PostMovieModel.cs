﻿namespace MovieAPI.Models
{
    public class PostMovieModel
    {
        public string MovieName { get; set; }
        public string Description { get; set; }
        public IFormFile Thumbnail { get; set; }
        public string Country { get; set; }
        public List<string> Actor { get; set; }
        public string Director { get; set; }
        public string Language { get; set; }
        public string Subtitle { get; set; }
        public DateTime ReleaseTime { get; set; }
        public DateTime PublicationTime { get; set; }
        public IFormFile CoverImage { get; set; }
        public string Age { get; set; }
        public IFormFile Movie { get; set; }
        public float RunningTime { get; set; }
        public string Quality { get; set; }
        public Guid UserID { get; set; }
        public Guid ClassID { get; set; }
        public Guid MovieTypeID { get; set; }
        public List<Guid> GenreID { get; set; }
    }
}