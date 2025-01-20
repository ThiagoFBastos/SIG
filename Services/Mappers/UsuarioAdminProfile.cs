using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.Users;
using Shared.Dtos;

namespace Services.Mappers
{
    public class UsuarioAdminProfile: Profile
    {
        public UsuarioAdminProfile()
        {
            CreateMap<UsuarioAdmin, UsuarioAdminDto>();

            CreateMap<UsuarioAdminForCreateDto, UsuarioAdmin>()
                .ForMember(ua => ua.PasswordHash, opt => opt.MapFrom(uac => ""))
                .ForMember(ua => ua.SalString, opt => opt.MapFrom(uac => ""));
        }
    }
}
