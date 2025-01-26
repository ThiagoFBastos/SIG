using AutoMapper;
using Domain.Entities.Users;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mappers
{
    public class UsuarioAdministrativoProfile: Profile
    {
        public UsuarioAdministrativoProfile()
        {
            CreateMap<UsuarioAdministrativo, UsuarioAdministrativoDto>();

            CreateMap<UsuarioAdministrativoForCreateDto, UsuarioAdministrativo>()
                 .ForMember(ua => ua.SalString, opt => opt.MapFrom(uac => ""))
                 .ForMember(ua => ua.PasswordHash, opt => opt.MapFrom(uca => ""));
        }
    }
}
