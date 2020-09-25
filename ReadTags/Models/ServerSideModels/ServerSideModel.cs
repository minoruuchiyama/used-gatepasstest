using System.Net;

namespace ReadTags.Models.ServerSideModels
{
    /// <summary>
    /// ServerSideModel
    /// </summary>
    public class ServerSideModel{
        /// <summary>
        /// リソースに対するクライアントの要求を表します
        /// </summary>
        protected HttpListenerRequest Request { set; get; }
//        protected HttpListenerRequest Request1 { set; get; }

        /// <summary>
        /// クライアントの要求に対する応答で、クライアントに送信されるオブジェクト
        /// </summary>
        protected HttpListenerResponse Response { set; get; }
//        protected HttpListenerResponse Response1 { set; get; }

        /// <summary>
        /// Response body
        /// </summary>
        public byte[] Content { set; get; }
//        public byte[] Content1 { set; get; }
    }
}
