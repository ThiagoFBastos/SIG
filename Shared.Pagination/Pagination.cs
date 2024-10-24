using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Pagination
{
    public record class Pagination<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; init; } = [];

        [JsonPropertyName("page")]
        public required int CurrentPage { get; init; }
    }
}