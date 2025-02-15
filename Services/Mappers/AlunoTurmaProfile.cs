using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Shared.Dtos;
using Domain.Entities;

namespace Services.Mappers
{
    public class AlunoTurmaProfile: Profile
    {
         public AlunoTurmaProfile()
         {
            CreateMap<AlunoTurma, AlunoTurmaDto>();

            CreateMap<AlunoTurmaForCreateDto, AlunoTurma>()
                .ForMember(at => at.Codigo, opt => opt.Ignore())
                .ForMember(at => at.Aluno, opt => opt.Ignore())
                .ForMember(at => at.Turma, opt => opt.Ignore());

            CreateMap<AlunoTurmaForUpdateDto, AlunoTurma>()
                .ForMember(at => at.Codigo, opt => opt.Ignore())
                .ForMember(at => at.Aluno, opt => opt.Ignore())
                .ForMember(at => at.Turma, opt => opt.Ignore())
                .ForMember(at => at.AlunoMatricula, opt => opt.Ignore());

            CreateMap<AlunoTurmaChangeNotaDto, AlunoTurma>()
                .ForMember(at => at.AlunoMatricula, opt => opt.Ignore())
                .ForMember(at => at.Turma, opt => opt.Ignore())
                .ForMember(at => at.Aluno, opt => opt.Ignore())
                .ForMember(at => at.Codigo, opt => opt.Ignore())
                .ForMember(at => at.TurmaCodigo, opt => opt.Ignore());

            CreateMap<AlunoTurmaChangeTurmaDto, AlunoTurma>()
                 .ForMember(at => at.AlunoMatricula, opt => opt.Ignore())
                 .ForMember(at => at.Turma, opt => opt.Ignore())
                 .ForMember(at => at.Aluno, opt => opt.Ignore())
                 .ForMember(at => at.Codigo, opt => opt.Ignore())
                 .ForMember(at => at.Nota, opt => opt.Ignore());
        }
    }
}