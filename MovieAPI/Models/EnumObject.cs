using System.ComponentModel;

namespace MovieAPI.Models
{
    public static class EnumObject
    {
        public enum FileType
        {
            Image,
            Video,
            OtherFile = 2
        };
    }
}
