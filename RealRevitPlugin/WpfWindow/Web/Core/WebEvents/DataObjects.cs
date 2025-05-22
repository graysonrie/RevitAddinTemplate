using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections.Generic;

namespace RealRevitPlugin.WpfWindow.Web.Core.WebEvents {
    public class Result<R> { 
        public R Output { get; set; }
        public Result(R output) { this.Output = output; }
    }

    public class CommandRequest {
        [JsonPropertyName("command")]
        public string Command { get; set; } = string.Empty;

        [JsonPropertyName("args")]
        public Dictionary<string, JsonElement> Args { get; set; } = new Dictionary<string, JsonElement>();

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class CommandResponse {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
