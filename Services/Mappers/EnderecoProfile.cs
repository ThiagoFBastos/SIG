using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Shared.Dtos;
using Domain.Entities;

namespace Services.Mappers
{
    public class EnderecoProfile: Profile
    {
        public EnderecoProfile()
        {
            CreateMap<Endereco, EnderecoDto>()
                .ForMember(e => e.Estado, opt => opt.MapFrom(x => (int)x.Estado));

            CreateMap<EnderecoForCreateDto, Endereco>()
                .ForMember(e => e.Id, opt => opt.Ignore());

            CreateMap<EnderecoForUpdateDto, Endereco>()
                .ForMember(e => e.Id, opt => opt.Ignore());
        }
    }
}