using System.Text.Json.Serialization;
using Shared.Dtos;
using Domain.Entities.Abstract;

namespace Shared.Dtos.Abstract
{
    public abstract record class IntegranteForUpdateDto
    {
        [JsonPropertyName("email")]
         public required string Email { get; init; }
         
         [JsonPropertyName("celular")]
         public required string Celular { get; init; }

         public virtual bool Match(object? obj)
         {
            var integrante = obj as Integrante;

            if(integrante is null)
                return false;

            return Email == integrante.Email
            && Celular == integrante.Celular;
         }
    }
}