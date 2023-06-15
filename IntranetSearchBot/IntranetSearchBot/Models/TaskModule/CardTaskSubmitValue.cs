using Newtonsoft.Json;

namespace IntranetSearchBot.Models.Models
{
    public class CardTaskSubmitValue<T>
    {
        [JsonProperty("type")]
        public object Type { get; set; } = "task/fetch";

        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("submitType")]
        public string SubmitType { get; set; }
    }
}
