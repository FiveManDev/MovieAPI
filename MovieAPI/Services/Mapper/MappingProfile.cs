using MovieAPI.Data;
using MovieAPI.Models.DTO;

namespace MovieAPI.Services.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            // Mapping (MovieInformation, MovieDTO)
            CreateMap<MovieInformation, MovieDTO>()
                .ForMember(
                    des => des.FirstNameAuthor,
                    opt => opt.MapFrom(src => src.User.Profile.FirstName)
                )
                .ForMember(
                    des => des.LastNameAuthor,
                    opt => opt.MapFrom(src => src.User.Profile.LastName)
                )
                .ForMember(
                    des => des.ClassName,
                    opt => opt.MapFrom(src => src.Classification.ClassName)
                )
                .ForMember(
                    des => des.MovieTypeName,
                    opt => opt.MapFrom(src => src.MovieType.MovieTypeName)
                )
                .ForMember(
                    des => des.Genres,
                    opt => opt.MapFrom(src => src.MovieGenreInformations.Select(movieGenreInfo => movieGenreInfo.GenreID))
                )
                .ReverseMap();

            // Mapping (User, UserDTO)
            CreateMap<User, UserDTO>()
                .ReverseMap();
            // Mapping (Profile, ProfileDTO)
            CreateMap<Profile, ProfileDTO>()
                .ForMember(
                    des => des.ClassName,
                    opt => opt.MapFrom(src => src.Classification.ClassName)
                )
                .ReverseMap();
            // Mapping (Authorization, AuthorizationDTO)
            CreateMap<Authorization, AuthorizationDTO>()
               .ReverseMap();
            // Mapping (Review, ReviewDTO)
            CreateMap<Review, ReviewDTO>()
               .ReverseMap();
            // Mapping (Genre, GenreDTO)
            CreateMap<Genre, GenreDTO>()
               .ReverseMap();
            // Mapping (MovieType, MovieTypeDTO)
            CreateMap<MovieType, MovieTypeDTO>()
               .ReverseMap();
            // Mapping (Classification, ClassificationDTO)
            CreateMap<Classification, ClassificationDTO>()
               .ReverseMap();
        }
    }
}
