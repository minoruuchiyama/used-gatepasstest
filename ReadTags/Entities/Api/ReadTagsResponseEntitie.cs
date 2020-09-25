using Newtonsoft.Json;

namespace ReadTags.Entities.Api
{
    /// <summary>
    /// ReadTagsResponseEntitie API レスポンス用エンティティ
    /// </summary>
    class ReadTagsResponseEntitie : BaseEntitie{
        /// <summary>
        /// ステータス
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("renban")]
        public string Renban { get; set; }

        [JsonProperty("match")]
        public string[] Match { get; set; }

        [JsonProperty("over")]
        public string[] Over { get; set; }

        [JsonProperty("lack")]
        public string[] Lack { get; set; }

    }
}
