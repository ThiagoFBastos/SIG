using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities;
using Shared.Dtos;

namespace Services.Mappers
{
    public class AlunoProfile: Profile
    {
        public AlunoProfile()
        {
            CreateMap<Aluno, AlunoDto>()
                .ForMember(a => a.AnoEscolar, opt => opt.MapFrom(x => (int)x.AnoEscolar))
                .ForMember(a => a.Status, opt => opt.MapFrom(x => (int)x.Status))
                .ForMember(a => a.Turno, opt => opt.MapFrom(x => (int)x.Turno))
                .ForMember(a => a.Sexo, opt => opt.MapFrom(x => (int)x.Sexo));

            CreateMap<AlunoForCreateDto, Aluno>()
                .ForMember(a => a.Matricula, opt => opt.Ignore())
                .ForMember(a => a.Turmas, opt => opt.Ignore())
                .ForMember(a => a.Endereco, opt => opt.Ignore())
                .ForMember(a => a.DataChegada, opt => opt.Ignore())
                .ForMember(a => a.Media, opt => opt.Ignore());

            CreateMap<AlunoForUpdateDto, Aluno>()
                .ForMember(a => a.Matricula, opt => opt.Ignore())
                .ForMember(a => a.EnderecoId, opt => opt.Ignore())
                .ForMember(a => a.Endereco, opt => opt.Ignore())
                .ForMember(a => a.CPF, opt => opt.Ignore())
                .ForMember(a => a.RG, opt => opt.Ignore())
                .ForMember(a => a.Sexo, opt => opt.Ignore())
                .ForMember(a => a.NomeCompleto, opt => opt.Ignore())
                .ForMember(a => a.DataNascimento, opt => opt.Ignore())
                .ForMember(a => a.DataChegada, opt => opt.Ignore())
                .ForMember(a => a.Media, opt => opt.Ignore());
        }
    }
}