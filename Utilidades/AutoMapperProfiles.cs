using AutoMapper;
using Seguridad_API.DTO;
using Seguridad_API.DTO.Users;
using Seguridad_API.Models;

namespace Seguridad_API.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //CreateMap<seguridad_usuario, UserDTO>();
            CreateMap<UserDTO, seguridad_usuario>();
            //CreateMap<UsuarioNuevoDTO, seguridad_usuario>();
            //CreateMap<seguridad_usuario, UsuarioNuevoDTO>();
            //CreateMap<UsuarioNuevoDTO, UserCreatedDTO>();
        }
    }
}
