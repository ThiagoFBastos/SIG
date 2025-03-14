using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Shared.Dtos;
using Domain.Entities;

namespace Services.Mappers
{
    public class TurmaProfile: Profile
    {
        public TurmaProfile()
        {
             CreateMap<Turma, TurmaDto>()
                .ForMember(t => t.AnoEscolar, opt => opt.MapFrom(x => (int)x.AnoEscolar));

            CreateMap<TurmaForCreateDto, Turma>()
                .ForMember(t => t.Codigo, opt => opt.Ignore())
                .ForMember(t => t.Professor, opt => opt.Ignore())
                .ForMember(t => t.Alunos, opt => opt.Ignore());

            CreateMap<TurmaForUpdateDto, Turma>()
                .ForMember(t => t.Codigo, opt => opt.Ignore())
                .ForMember(t => t.Professor, opt => opt.Ignore())
                .ForMember(t => t.Alunos, opt => opt.Ignore());

            CreateMap<Turma, TurmaSemNotaDto>();
        }
    }
}