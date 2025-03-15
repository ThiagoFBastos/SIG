using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Pagination.Abstract
{
    public abstract record class GetOptions
    {
        private int limiteDeResultados = 10;

        [FromQuery(Name = "comeco")]
        [JsonPropertyName("comeco")]
        public int ComecarApartirDe { get; init; } = 0;

        [FromQuery(Name = "limite")]
        [JsonPropertyName("limite")]
        public int LimiteDeResultados 
        { 
            get => limiteDeResultados;
            init
            {
                limiteDeResultados = int.Min(value, 10);
            }
        }

        [FromQuery(Name = "ordenacao")]
        [JsonPropertyName("ordenacao")]
        public string? Ordenacao { get; init; }

        [FromQuery(Name = "crescente")]
        [JsonPropertyName("crescente")]
        public bool Crescente { get; init; } = true;
    }
}