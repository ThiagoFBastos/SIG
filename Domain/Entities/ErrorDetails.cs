using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ErrorDetails
    {
        [JsonPropertyName("status")]
        public required int Status { get; set; }

        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("traceId")]
        public required string TraceId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
        public override string ToString() => JsonSerializer.Serialize(this);
    }
}