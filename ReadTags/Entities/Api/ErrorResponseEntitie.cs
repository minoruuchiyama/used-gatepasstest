using Newtonsoft.Json;
using System.Collections.Generic;

namespace ReadTags.Entities.Api
{
    /// <summary>
    /// エラー詳細返却用エンティティ
    /// </summary>
    class ErrorResponseEntitie : BaseEntitie{

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
