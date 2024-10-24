using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities.Abstract;

namespace Shared.Dtos.Abstract
{
    public abstract record class FuncionarioForCreateDto: IntegranteForCreateDto
    {
        [JsonPropertyName("cargo")]
        public required string Cargo { get; set; }
        
        [JsonPropertyName("salario")] 
         public required decimal Salario { get; init; }

         [JsonPropertyName("banco")]
         public required string Banco { get; init; }

         [JsonPropertyName("conta_corrente")]
         public required string ContaCorrente { get; init; }

         [JsonPropertyName("status")]
         public required int Status { get; init; }

         [JsonPropertyName("horario_inicio_expediente")]
         public required DateTime HorarioInicioExpediente { get; init; }

         [JsonPropertyName("horario_fim_expediente")]
         public required DateTime HorarioFimExpediente { get; init; }

         public override bool Match(object? obj)
         {
            var funcionario = obj as Funcionario;

            if(funcionario is null)
                return false;

            return base.Match(obj)
            && Cargo == funcionario.Cargo
            && Salario == funcionario.Salario
            && Banco == funcionario.Banco
            && ContaCorrente == funcionario.ContaCorrente
            && Status == (int)funcionario.Status
            && HorarioInicioExpediente == funcionario.HorarioInicioExpediente
            && HorarioFimExpediente == funcionario.HorarioFimExpediente;
         }
    }
}