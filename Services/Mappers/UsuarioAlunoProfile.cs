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
    public class UsuarioAlunoProfile: Profile
    {
        public UsuarioAlunoProfile() 
        {
            CreateMap<UsuarioAluno, UsuarioAlunoDto>();

            CreateMap<UsuarioAlunoForCreateDto, UsuarioAluno>()
                .ForMember(ua => ua.PasswordHash, opt => opt.MapFrom(uac => ""))
                .ForMember(ua => ua.SalString, opt => opt.MapFrom(uac => ""));
        }
    }
}
