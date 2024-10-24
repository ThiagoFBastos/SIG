using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities.Abstract;

namespace Shared.Dtos.Abstract
{
    public abstract record class FuncionarioDto: IntegranteDto
    {
        [JsonPropertyName("matricula")]
        public required Guid Matricula {get; set;}
        
        [JsonPropertyName("cargo")]
        public required string Cargo { get; set; }
        
        [JsonPropertyName("salario")]
         public required decimal Salario { get; set; }

         [JsonPropertyName("banco")]
         public required string Banco { get; set; }

         [JsonPropertyName("conta_corrente")]
         public required string ContaCorrente { get; set; }

         [JsonPropertyName("status")]
         public required int Status { get; set; }

         [JsonPropertyName("data_demissao")]
         public DateTime? DataDemissao { get; set; }

         [JsonPropertyName("horario_inicio_expediente")]
         public required DateTime HorarioInicioExpediente { get; set; }

         [JsonPropertyName("horario_fim_expediente")]
         public required DateTime HorarioFimExpediente { get; set; }

         public override bool Match(object? obj)
         {
            var funcionario = obj as Funcionario;

            if(funcionario is null)
                return false;

            return base.Match(obj) 
            && funcionario.DataDemissao == DataDemissao
            && funcionario.HorarioFimExpediente == HorarioFimExpediente
            && funcionario.HorarioInicioExpediente == HorarioInicioExpediente
            && funcionario.Matricula == Matricula
            && funcionario.Salario == Salario
            && (int)funcionario.Status == Status
            && funcionario.Cargo == Cargo
            && funcionario.Matricula == Matricula
            && funcionario.Banco == Banco
            && funcionario.ContaCorrente == ContaCorrente;
         }
    }
}