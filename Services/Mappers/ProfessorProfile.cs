using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities;
using Shared.Dtos;

namespace Services.Mappers
{
    public class ProfessorProfile: Profile
    {
        public ProfessorProfile()
        {
            CreateMap<Professor, ProfessorDto>()
                .ForMember(p => p.Sexo, opt => opt.MapFrom(x => (int)x.Sexo))
                .ForMember(p => p.Status, opt => opt.MapFrom(x => (int)x.Status));

            CreateMap<ProfessorForCreateDto, Professor>()
                .ForMember(p => p.Matricula, opt => opt.Ignore())
                .ForMember(p => p.Endereco, opt => opt.Ignore())
                .ForMember(p => p.DataChegada, opt => opt.Ignore())
                .ForMember(p => p.Turmas, opt => opt.Ignore())
                .ForMember(p => p.DataDemissao, opt => opt.Ignore());

            CreateMap<ProfessorForUpdateDto, Professor>()
                .ForMember(p => p.Matricula, opt => opt.Ignore())
                .ForMember(p => p.Endereco, opt => opt.Ignore())
                .ForMember(p => p.EnderecoId, opt => opt.Ignore())
                .ForMember(p => p.DataChegada, opt => opt.Ignore())
                .ForMember(p => p.Turmas, opt => opt.Ignore())
                .ForMember(p => p.CPF, opt => opt.Ignore())
                .ForMember(p => p.RG, opt => opt.Ignore())
                .ForMember(p => p.DataNascimento, opt => opt.Ignore())
                .ForMember(p => p.NomeCompleto, opt => opt.Ignore())
                .ForMember(p => p.Sexo, opt => opt.Ignore());       
        }
    }
}