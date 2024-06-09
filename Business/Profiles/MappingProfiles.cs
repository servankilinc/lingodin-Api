using AutoMapper;
using Model.Dtos.CategoryDtos;
using Model.Dtos.RoleDtos;
using Model.Dtos.UserDtos;
using Model.Dtos.WordDtos;
using Model.Entities;

namespace Business.Profiles;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        // CreateMap<source, dest>

        // WordService
        CreateMap<Word, WordResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.Turkish, opt => opt.MapFrom(src => src.Turkish))
            .ForMember(dest => dest.English, opt => opt.MapFrom(src => src.English))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.HasImage, opt => opt.MapFrom(src => src.HasImage));
        
        CreateMap<WordCreateDto, Word>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Turkish, opt => opt.MapFrom(src => src.Turkish))
            .ForMember(dest => dest.English, opt => opt.MapFrom(src => src.English))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));

        CreateMap<WordUpdateDto, Word>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Turkish, opt => opt.MapFrom(src => src.Turkish))
            .ForMember(dest => dest.English, opt => opt.MapFrom(src => src.English))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));

        CreateMap<WordImageUpdateDto, Word>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.HasImage, opt => opt.MapFrom(src => src.HasImage));


        // Category Service
        CreateMap<Category, CategoryResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Turkish, opt => opt.MapFrom(src => src.Turkish))
            .ForMember(dest => dest.English, opt => opt.MapFrom(src => src.English))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.HasImage, opt => opt.MapFrom(src => src.HasImage));

        CreateMap<CategoryCreateDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Turkish, opt => opt.MapFrom(src => src.Turkish))
            .ForMember(dest => dest.English, opt => opt.MapFrom(src => src.English));
        
        CreateMap<CategoryUpdateDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Turkish, opt => opt.MapFrom(src => src.Turkish))
            .ForMember(dest => dest.English, opt => opt.MapFrom(src => src.English));

        CreateMap<CategoryImageUpdateDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.HasImage, opt => opt.MapFrom(src => src.HasImage));


        // Role Service
        CreateMap<Role, RoleResponseDto> ()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<RoleCreateDto, Role>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        // User Service
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.IsVerifiedUser, opt => opt.MapFrom(src => src.IsVerifiedUser))
            .ForMember(dest => dest.AutheticatorType, opt => opt.MapFrom(src => src.AutheticatorType));
         
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.AutheticatorType, opt => opt.MapFrom(src => src.AutheticatorType));

        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}