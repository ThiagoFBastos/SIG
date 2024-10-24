using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Pagination.Abstract
{
    public abstract record class GetOptions
    {
        private int limiteDeResultados = 10;

        [JsonPropertyName("comeco")]
        public int ComecarApartirDe { get; init; } = 0;

        [JsonPropertyName("limite")]
        public int LimiteDeResultados 
        { 
            get => limiteDeResultados;
            init
            {
                limiteDeResultados = int.Min(value, 10);
            }
        }

        [JsonPropertyName("ordenacao")]
        public string? Ordenacao { get; init; }

        [JsonPropertyName("crescente")]
        public bool Crescente { get; init; } = true;
    }
}