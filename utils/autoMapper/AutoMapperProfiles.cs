using AutoMapper;
using back.File.Dtos;
using back.models;

using project.roles;
using project.roles.dto;
using project.users;
using project.users.dto;
using project.users.Models;

namespace project.utils.autoMapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<userEntity, userDto>()
            .ForMember(userDtoId => userDtoId.isActive, options => options.MapFrom(src => src.deleteAt == null));
            ;

            CreateMap<rolEntity, rolDto>();
            CreateMap<rolCreationDto, rolEntity>();
            CreateMap<Client, clientDto>();
            CreateMap<clientCreationDto, Client>();
            CreateMap<clientCreationDto, userCreationDto>();
            CreateMap<Files, FileDto>();

        }

    }
}