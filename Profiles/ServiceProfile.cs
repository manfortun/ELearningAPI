using AutoMapper;
using eLearningApi.DTOs;
using eLearningApi.Models;

namespace eLearningApi.Profiles;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<User, UserReadDto>();
        CreateMap<UserCreateDto, User>();
        CreateMap<User, UserCreateCompleteDto>();
        CreateMap<User, UserLoginSuccessDto>();

        CreateMap<Subject, SubjectReadDto>()
            .ForMember(dest => dest.Owner, option => option.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"));

        CreateMap<Course, CourseReadDto>()
            .ForMember(dest => dest.Author, option => option.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"));

        CreateMap<Module, ModuleReadDto>();

        CreateMap<Content, ContentReadDto>();

        CreateMap<Enrollment, EnrollmentReadDto>();
    }
}
