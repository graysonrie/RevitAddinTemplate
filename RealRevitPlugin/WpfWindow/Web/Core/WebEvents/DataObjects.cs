using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RealRevitPlugin.WpfWindow.Web.Core.WebEvents
{
    public class Result<R>
    {
        public R Output { get; set; }
        public Result(R output) { this.Output = output; }
    }

    public class CommandRequest
    {
        [JsonProperty("command")]
        public string Command { get; set; } = string.Empty;

        [JsonProperty("args")]
        public Dictionary<string, JToken> Args { get; set; } = new Dictionary<string, JToken>();

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class CommandResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
