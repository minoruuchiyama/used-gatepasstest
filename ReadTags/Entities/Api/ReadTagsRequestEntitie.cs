using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using ReadTags.Exceptions;

namespace ReadTags.Entities.Api {

    /// <summary>
    /// CheckNewFile(作成日が最も新しいファイルの情報の確認) API リクエスト用エンティティ
    /// </summary>
    class ReadTagsRequestEntitie : BaseEntitie{

        /// <summary>
        /// ファイル名と照合する検索文字列。
        /// このパラメーターには、有効なリテラル パスとワイルドカード (* および ?)
        /// の組み合わせを使用できまが、正規表現はサポートしていません。
        /// 既定のパターンは "*" で、すべてのファイルが返されます。
        /// </summary>

        /// <summary>
        /// ステータスチェック
        /// </summary>
        [JsonProperty("statusCheck")]
        [Required]
        public string StatusCheck { set; get; }

        /// <summary>
        /// 待機秒数
        /// </summary>
        [JsonProperty("sec")]
        [Required]
        public string Sec { set; get; }

        /// <summary>
        /// オリコンNo
        /// </summary>
        [JsonProperty("oriTag")]
        [Required]
        public string OriTag { set; get; }

        /// <summary>
        /// タグ
        /// </summary>
        [JsonProperty("tag")]
        [Required]
        public string Tag { set; get; }

    }
}
